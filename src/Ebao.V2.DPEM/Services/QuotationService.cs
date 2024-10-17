using System.Globalization;
using System.Text.RegularExpressions;
using Ebao.V2.DPEM.Api.Payment.Responses;
using Ebao.V2.DPEM.Api.Quotation;
using Ebao.V2.DPEM.Api.Quotation.Requests;
using Ebao.V2.DPEM.Api.Quotation.Responses;
using Ebao.V2.DPEM.Helpers;
using Ebao.V2.DPEM.Helpers.Extensions;
using Ebao.V2.DPEM.Helpers.Templates;
using Ebao.V2.DPEM.Models;
using Ebao.V2.DPEM.Models.Template;
using Ebao.V2.DPEM.Models.ViewModels.Quotation.Requests;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ebao.V2.DPEM.Services;

/// <summary>
/// Responsável por manipular as cotações no V3.
/// </summary>
public class QuotationService : ServiceBase<QuotationService>
{
    private readonly IMemoryCache _cache;
    private readonly UserService _userService;
    private readonly BrokerService _brokerService;
    private readonly IQuotationApi _quotationApi;
    private readonly ClientService _clientService;
    private readonly CoverageService _coverageService;

    public QuotationService(ILogger<QuotationService> logger, IMemoryCache cache, RequestInfo requestInfo,
        UserService userService, IQuotationApi quotationApi, BrokerService brokerService,
        ClientService clientService, CoverageService coverageService) : base(
        logger, cache, requestInfo)
    {
        _cache = cache;
        _userService = userService;
        _quotationApi = quotationApi;
        _brokerService = brokerService;
        _clientService = clientService;
        _coverageService = coverageService;
    }

    /// <summary>
    /// Faz a criação da cotação.
    /// </summary>
    /// <param name="policy">Informações da cotação a ser criada.</param>
    /// <returns></returns>
    public async ValueTask<V3QuotationResponse> CreateQuotationAsync(PolicyRequest policy)
    {
        using var _ = Logger.BeginScope("Policy", policy.Certificate);

        Logger.LogInformation("Criando cotação para a apólice.");

        var quotation = await CreateEmptyQuotationAsync();

        using var __ = Logger.BeginScope("Quotation", quotation.Id);
        Logger.LogInformation("Cotação {Quotation} criada para a apólice {Policy}", quotation.Id, policy.Certificate);

        //O formulário de salvamento do V3, exige uma cobertura padrão selecionada.
        var defaultCoverageId = policy.Coverages.First().Id;
        quotation.CtId = defaultCoverageId;
        quotation.CtType = defaultCoverageId;
        quotation.PlatformCerti = policy.Certificate;
        quotation.PolicyProtocolFinalizedDateStr = policy.ProtocolDate.ToString("dd/MM/yyyy HH:mm");
        quotation.ReinsuranceReferenceDateStr = policy.EffectiveDate.ToString("dd/MM/yyyy");
        quotation.PropDate = DateTime.Now.ToString("dd/MM/yyyy");
        quotation.AgreementId = "177857";
        quotation.CommRateCharger = "agreement";
        quotation.OperId = "NBDeModifyProductDetail";
        quotation.NeedSave = "1";
        quotation.SalesPlatform = 268;
        quotation.Xml = await PrepareXmlAsync(quotation, policy);
        quotation.PrePaymentFlag = "1"; //Pré pagamento.
        quotation.NextPageType = "3";
        quotation.Error3 = 1.000000;

        //O valor addCT, informa que "clicamos" no botão de adicionar cobertura.
        //Isso é necessário para que ao chamar o UpdateQuotationAsync, seja retornado o HTML da página de adição de cobertura.
        //Tornando possível obter o pageId.
        quotation.SActionType = "addCT";

        Logger.LogInformation("Atualizando informações da cotação.");

        //Atualiza as informações da cotação e retorna o html da página de cobertura.
        var html = await _quotationApi.UpdateQuotationAsync(quotation);

        //Obtém o pageId da cotação.
        //TODO: Este trecho deve ser motivo para o CoverageService, combina mais ele lá.
        var pageId = GetPageId(html);
        var reader = HtmlReader.CreateReader(html);
        var sysKeyPage = long.Parse(reader.GetValueFromXPathOrThrow("//*[@name='syskey_page_token']"));

        foreach (var coverage in policy.Coverages)
            sysKeyPage = await _coverageService.AddCoverageAsync(coverage, quotation.PolicyId, sysKeyPage,
                pageId);

        await _clientService.AssignOrCreateUserQuotationAsync(policy.Insured);

        await _brokerService.SetBrokerComissionAsync(policy.CommissionRate);

        sysKeyPage = await CalculateAsync(quotation, policy.ExpectedPremium, sysKeyPage.ToString());

        // await CheckQuotationStatusAsync(quotation.PolicyId);
        await AssignQuotationAsync(sysKeyPage);
        await SendToUnderwritingAsync(quotation.PolicyId);

        Logger.LogInformation("Apólice emitida com sucesso.");

        return quotation;
    }

    private async ValueTask<long> CalculateAsync(V3QuotationResponse quotation, decimal expectedPremium,
        string syskeyPageToken)
    {
        const int delay = 1000;
        const int maxRetry = 10;
        var retry = 0;
        decimal premiumEbao;

        var prevXml = quotation.Xml;
        var currentOperId = quotation.OperId;

        quotation.Xml = null;
        quotation.OperId = "DEProcessCaculatePremium";
        quotation.SyskeyPageToken = syskeyPageToken;

        do
        {
            ++retry;

            Logger.LogInformation("Calculando prêmio da cotação. Tentativa {Retry}.", retry);

            var html = await _quotationApi.UpdateQuotationAsync(quotation);
            var reader = HtmlReader.CreateReader(html);

            premiumEbao = decimal.Parse(reader.GetValueByNameOrThrow("premiumPayable"));

            var sysKeyPage = long.Parse(reader.GetValueFromXPathOrThrow("//*[@name='syskey_page_token']"));
            quotation.SyskeyPageToken = sysKeyPage.ToString();

            if (premiumEbao == expectedPremium)
            {
                Logger.LogInformation("Prêmio calculado com sucesso. Prêmio Ebao: {premiumEbao} Esperado: {ExpectedPremium}.",
                    premiumEbao, expectedPremium);
                break;
            }

            Logger.LogWarning("Prêmio calculado divergente. Prêmio Ebao: {premiumEbao} Esperado: {ExpectedPremium}.",
                premiumEbao, expectedPremium);

            Logger.LogInformation("Aguardando {Delay}ms para nova tentativa.", delay);
            await Task.Delay(delay);
        } while (retry < maxRetry);

        if (retry >= maxRetry)
            throw new InvalidOperationException($"Não foi possível calcular o prêmio da cotação. OperationSetup: {expectedPremium}; Ebao: {premiumEbao}");

        quotation.Xml = prevXml;
        quotation.OperId = currentOperId;

        return long.Parse(quotation.SyskeyPageToken);
    }

    /// <summary>
    /// Obtém o pageId da cotação.
    /// </summary>
    /// <param name="html">HTML da página de criação da cotação.</param>
    /// <remarks>
    /// Este campo é necessário no envio de coberturas.
    /// Não sabemos exatamente o motivo do porque ele é necessário.
    /// </remarks>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private string GetPageId(string html)
    {
        var pattern = @"pageId:""(\d+)""";
        var match = Regex.Match(html, pattern);

        if (!match.Success)
            throw new InvalidOperationException("PageId não encontrado.");

        return match.Groups[1].Value;
    }

    /// <summary>
    /// Atribui a cotação ao usuário do bot.
    /// </summary>
    /// <param name="syskeyPageToken"></param>
    private async ValueTask AssignQuotationAsync(long syskeyPageToken)
    {
        Logger.LogInformation("Atribuindo cotação para o usuário de subscrição. {Username}.", Config.Username);

        var (actorId, newSyskeyToken) = await GetActorIdAsync(Config.Username);

        if (newSyskeyToken > 0)
            syskeyPageToken = newSyskeyToken;

        var body = new V3AssignQuotationRequest(actorId, syskeyPageToken, _userService.GetSyskeyRequestToken());

        Logger.LogInformation("Enviando request de atribuição.");
        await _quotationApi.AssignQuotationAsync(body);
        Logger.LogInformation("Atribuição feita com sucesso..");
    }

    /// <summary>
    /// Envia uma cotação para underwriting.
    /// </summary>
    /// <param name="policyId">PolicyId da cotação a ser enviada.</param>
    private async ValueTask SendToUnderwritingAsync(string policyId)
    {
        Logger.LogInformation("Enviando apólice para subscrição.");

        var request = new V3PrepareDeTransactionBody(policyId, _userService.GetSyskeyRequestToken());
        await _quotationApi.SendToUnderwritingAsync(request);

        Logger.LogInformation("Apólice enviada para subscrição com sucesso..");
    }

    /// <summary>
    /// Obtém o actorId a partir do username.
    /// </summary>
    /// <param name="username">Nome de usuário.</param>
    /// <returns>ActorId do usuário.</returns>
    /// <exception cref="NullReferenceException"></exception>
    private async ValueTask<(string actorId, long syskeyPageToken)> GetActorIdAsync(string username)
    {
        Logger.LogInformation("Obtendo o ActorId do usuário {Username}.", username);
        // if (_cache.TryGetValue<string>(username, out var actorId))
        // {
        //     Logger.LogInformation("ActorId {ActorId} encontrado no cache.", actorId);
        //     return (actorId, -1)!;
        // }

        string actorId = "";
        var body = new V3PageDecisionRequest(_userService.GetSyskeyRequestToken());

        Logger.LogInformation("Carregando página de decisão.");
        var pageHtml = await _quotationApi.LoadPageDecisionAsync(body);

        var reader = new HtmlReader();
        reader.Load(pageHtml);

        foreach (var option in reader.ReadSelectOptions("actorId"))
        {
            _cache.Set(option.Value, option.Key);

            if (option.Value == username)
                actorId = option.Key;
        }

        if (actorId == null)
            throw new NullReferenceException($"Actor {username} not found.");

        var syskeyPage = long.Parse(reader.GetValueByNameOrThrow("syskey_page_token"));

        Logger.LogInformation("ActorId obtido com sucesso.");
        return (actorId, syskeyPage);
    }

    /// <summary>
    /// Monta o XML a ser enviado na cotação.
    /// </summary>
    /// <param name="quotation">Cotação criada pelo V3.</param>
    /// <param name="policy">Informações da apólice.</param>
    /// <returns>XML criado.</returns>
    private static async ValueTask<string> PrepareXmlAsync(V3QuotationResponse quotation, PolicyRequest policy)
    {
        var templateModel = new QuotationTemplate()
        {
            Activity = policy.Vessel.ActivityId,
            Crews = policy.Vessel.Crews,
            Propulsion = policy.Vessel.PropulsionId,
            NavigationType = policy.Vessel.NavigationTypeId,
            VesselName = policy.Vessel.Name,
            VesselNumber = policy.Vessel.Number,
            TypeOfVessel = policy.Vessel.TypeId,
            UseOfTheVessel = policy.Vessel.UseOfTheVesselId,
            Lmi = policy.Lmi,
            EffectiveDate = policy.EffectiveDate,
            ExpiryDate = policy.ExpiryDate,
            InsertTime = DateTime.Now,
            OperatorId = ulong.Parse(quotation.OperatorId),
            OperatorName = quotation.OperatorName,
            PolicyId = quotation.PolicyId,
            QuotationId = quotation.Id,
            ProposalDate = DateTime.Now,
            NumberOfPassengers = policy.Vessel.NumberOfPassengers,
            Segment = policy.Vessel.SegmentId
        };

        var writer = new TemplateWriter(new TemplateInfo("QuotationTemplate", "Policy"));
        var xml = await writer.GenerateAsync(templateModel);

        return xml;
    }

    /// <summary>
    /// Faz a emissão de uma cotação.
    /// </summary>
    /// <param name="payment">Informações de pagamento.</param>
    /// <param name="policyRequest">Informações da apólice.</param>
    public async ValueTask<string> IssueQuotationAsync(V3PaymentResponse payment, PolicyRequest policyRequest)
    {
        Logger.LogInformation("Emitindo cotação.");

        var quotationNo = payment.ProposalNo;
        var taskId = await GetTaskIdFromQuotationAsync(quotationNo);

        var loadRequest = new V3SearchQuotationRequest("", _userService.GetSyskeyRequestToken());
        await _quotationApi.LoadQuotationAsync(loadRequest, taskId);

        await SetPaymentDataAsync(payment, policyRequest);
        await SendToBoundAsync(payment.PolicyId);
        var certificate = await IssueAsync();

        Logger.LogInformation("Cotação emitida com sucesso.");

        return certificate;
    }

    /// <summary>
    /// Atualiza as inforamções de pagamento da apólice.
    /// </summary>
    /// <param name="payment">Informações de pagamento.</param>
    /// <param name="policyRequest">Informações da apólice.</param>
    private async ValueTask SetPaymentDataAsync(V3PaymentResponse payment, PolicyRequest policyRequest)
    {
        //Necessário carregar a página para obter o valor de bpRadio.
        var html = await _quotationApi.LoadPaymentInfoAsync();
        var reader = HtmlReader.CreateReader(html);

        //O formulário exige envio do campo bpRadio. Acredito que seja o Id relacionado ao pagamento.
        var bpId = reader.GetValueByNameOrThrow("bpRadio");

        var request = new V3UpdatePaymentRequest(payment.PayorCode, payment.Premium, bpId, policyRequest.PaymentDate,
            _userService.GetSyskeyRequestToken(), policyRequest.Insured.Identity.Length == 14);
        await _quotationApi.UpdatePaymentInfoAsync(request);
    }

    /// <summary>
    /// Transforma a cotação em apólice.
    /// </summary>
    /// <param name="policyId">Número da apólice da cotação.</param>
    private async ValueTask SendToBoundAsync(string policyId)
    {
        Logger.LogInformation("Enviando cotatação para proposta.");

        Logger.LogInformation("Efetuando a operação Quote Bound.");
        var boundRequest = new V3QuoteBoundRequest(_userService.GetSyskeyRequestToken());
        await _quotationApi.SendToBoundAsync(boundRequest);

        Logger.LogInformation("Efetuando o dispatch da cotação.");
        var dispatchRequest = new V3DispatchRequest(policyId, _userService.GetSyskeyRequestToken());
        await _quotationApi.DispatchBoundAsync(dispatchRequest);

        Logger.LogInformation("Cotação enviada para proposta com sucesso.");
    }

    /// <summary>
    /// Faz a emissão da apólice.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private async ValueTask<string> IssueAsync()
    {
        Logger.LogInformation("Efetuando emissão da proposta.");
        var request = new V3IssueRequest(_userService.GetSyskeyRequestToken());
        var html = await _quotationApi.IssueAsync(request);

        var pattern = @"Policy No\. is\s+(\d+)";
        var match = Regex.Match(html, pattern);

        if (!match.Success)
            throw new InvalidOperationException("Certificado não encontrado.");

        var certificate = match.Groups[1].Value;
        Logger.LogInformation("Apólice emitida com sucesso. Certificado: {Certificate}", certificate);
        return certificate;
    }

    /// <summary>
    /// Obtém o número da task a partir do número da cotação.
    /// </summary>
    /// <remarks>
    /// No eBao, as API's não acessam a cotação pelo seu Id, mas sim pelo número da Task associado a ela.
    /// Não sabemos exatamente o porque desta implementação, mas acredito que seja algo ligado ao workflow do v3.
    /// </remarks>
    /// <param name="quotationNo">Número da cotação a ser obtido o número da task.</param>
    /// <returns>TaskId da cotação.</returns>
    /// <exception cref="NullReferenceException"></exception>
    private async ValueTask<string> GetTaskIdFromQuotationAsync(string quotationNo)
    {
        Logger.LogInformation("Obtendo TaskId da cotação {QuotationNo}.", quotationNo);

        var templateModel = new SearchQuotationTemplate(quotationNo);
        var writer = new TemplateWriter(new TemplateInfo("", "SearchQuotation"));
        var xml = await writer.GenerateAsync(templateModel);

        var request = new V3SearchQuotationRequest(xml, _userService.GetSyskeyRequestToken());
        var html = await _quotationApi.SearchQuotationAsync(request);
        var reader = HtmlReader.CreateReader(html);

        var quoteNode = reader.Document.DocumentNode
            .SelectNodes("//quono").FirstOrDefault(w => w.InnerText == quotationNo && w.ParentNode.Name == "map");

        var taskIdNode = quoteNode.ParentNode.ChildNodes.FirstOrDefault(w => w.Name == "wftaskid");

        if (taskIdNode == null)
            throw new NullReferenceException("TaskId não encontrado.");

        var taskId = taskIdNode!.InnerText;

        Logger.LogInformation("TaskId {TaskId} encontrado para a cotação {QuotationNo}.", taskId, quotationNo);

        return taskId;
    }

    /// <summary>
    /// Faz a criação da cotação a ser populada.
    /// </summary>
    /// <remarks>
    /// O usuário logado, não pode simplesmente chamar a API de cotação e criar uma cotação.
    /// O token dele precisa ir até a "pagina de criação" de cotação.
    /// Apenas apartir daí que é possível criar cotaçõo.
    /// </remarks>
    /// <returns></returns>
    private async ValueTask<V3QuotationResponse> CreateEmptyQuotationAsync()
    {
        Logger.LogInformation("Criando cotação vazia.");

        await PrepareSystokenAsync();
        var pageHtml = await _quotationApi.LoadPageQuickQuotationAsync();

        Logger.LogInformation("Cotação criando com sucesso. Obtendo informações da cotação...");
        return ReadQuotationInfo(pageHtml);
    }

    /// <summary>
    /// Prepara a sessão atual para criação da cotação.
    /// </summary>
    /// <remarks>
    /// O usuário logado, não pode simplesmente chamar a API de cotação e criar uma cotação.
    /// O token dele precisa ir até a "pagina de criação" de cotação.
    /// Apenas apartir daí que é possível criar cotaçõo.
    /// </remarks>
    private async ValueTask PrepareSystokenAsync()
    {
        Logger.LogInformation("Preparando sessão para criação da cotação.");
        var syskey = _userService.GetSyskeyRequestToken();
        var worklistRequest = new V3WorklistRequest(syskey);

        Logger.LogInformation("Carregando página de cotação.");
        var pageHtml = await _quotationApi.GetWorkListAsync(worklistRequest);

        var reader = new HtmlReader();
        reader.Load(pageHtml);

        var sysKeyPage = long.Parse(reader.GetValueFromXPathOrThrow("//*[@name='syskey_page_token']"));

        Logger.LogInformation("Efetuando QuickQuote.");
        var quickQuoteRequest = new V3QuickQuoteRequest(sysKeyPage, syskey);
        await _quotationApi.QuickQuoteAsync(quickQuoteRequest);

        Logger.LogInformation("Sessão preparada com sucesso.");
    }

    /// <summary>
    /// Faz a leitura das informações da cotação do HTML.
    /// </summary>
    /// <param name="pageHtml">HTML da página de criação da cotação.</param>
    /// <returns></returns>
    private V3QuotationResponse ReadQuotationInfo(string pageHtml)
    {
        var reader = HtmlReader.CreateReader(pageHtml);

        var selectedCommercialPrdt = reader.ReadSelectOptions("commercialPrdt").FirstOrDefault(w => w.IsSelected);

        var quotation = new V3QuotationResponse()
        {
            OperId = reader.GetValueByName("operId"),
            CtId = reader.GetValueByName("_ctId"),
            Id2 = reader.GetValueByName("_id"),
            CtType = reader.GetValueByName("_ctType"),
            InsuredCateCode = int.Parse(reader.GetValueByName("_insuredCateCode")),
            AdjustedNetPremium = double.Parse(reader.GetValueByName("adjustedNetPremium", "0")),
            AgreementId = reader.GetValueByName("agreementId"),
            AnnualPremium = double.Parse(reader.GetValueByName("annualPremium")),
            BackUrl = reader.GetValueByName("backUrl"),
            BankName = reader.GetValueByName("bankName"),
            CommCurType = int.Parse(reader.GetValueByName("commCurType")),
            CommercialPrdt = selectedCommercialPrdt != null ? selectedCommercialPrdt.Key.ToString() : "",
            CommRateCharger = reader.GetValueByName("commRateCharger", "agreement"),
            ContactorEmail = reader.GetValueByName("contactorEmail"),
            ContactorName = reader.GetValueByName("contactorName"),
            ContactorPartyId = reader.GetValueByName("contactorPartyId"),
            CoverNoteNo = reader.GetValueByName("coverNoteNo"),
            CoverNoteType = reader.GetValueByName("coverNoteType"),
            CtxBizProductCode = reader.GetValueByName("ctx.bizProductCode"),
            PartnerShip = reader.GetValueByName("partnership"),
            CtxCtCode = reader.GetValueByName("ctx.ctCode", "null"),
            CtxCtId = reader.GetValueByName("ctx.ctId", "0"),
            CtxInsuredCateCode = reader.GetValueByName("ctx.insuredCateCode", "0"),
            CtxInsuredId = long.Parse(reader.GetValueByName("ctx.insuredId", "0")),
            CtxIsEndorsement = bool.Parse(reader.GetValueByName("ctx.isEndorsement", "false")),
            CtxIsInDataEntry = bool.Parse(reader.GetValueByName("ctx.isInDataEntry", "true")),
            CtxIsInManualPricing = bool.Parse(reader.GetValueByName("ctx.isInManualPricing", "false")),
            CtxIsInQuotation = bool.Parse(reader.GetValueByName("ctx.isInQuotation", "false")),
            CtxIsNewBiz = bool.Parse(reader.GetValueByName("ctx.isNewBiz", "true")),
            CtxIsRenewal = bool.Parse(reader.GetValueByName("ctx.isRenewal", "false")),
            CtxPolicyCtId = reader.GetValueByName("ctx.policyCtId"),
            CtxPolicyId = long.Parse(reader.GetValueByNameOrThrow("ctx.policyId")),
            CtxProductCode = reader.GetValueByName("ctx.productCode"),
            CtxProductId = long.Parse(reader.GetValueByName("ctx.productId")),
            DescritorId = reader.GetValueByName("descritorId"),
            Edit = reader.GetValueByName("edit"),
            EndoId = reader.GetValueByName("endoId"),
            ErrorMessage = reader.GetValueByName("er_message"),
            ErrorResult = reader.GetValueByName("er_result"),
            Error1 = double.Parse(reader.GetValueByName("er1", "1.000000")),
            Error2 = double.Parse(reader.GetValueByName("er2", "1.000000"), new CultureInfo("pt-Br")),
            Error3 = double.Parse(reader.GetValueByName("er3", "1.000000")),
            InitCommRateCharger = reader.GetValueByName("initCommRateCharger", "null"),
            InnerMemo = reader.GetValueByName("innerMemo"),
            InstalNo = reader.GetValueByName("instalNo", "4"),
            InsuredId = reader.GetValueByName("insuredId"),
            IntroducerPtyrId = reader.GetValueByName("introducerPtyrId", "0"),
            MasterPolicyCopy = reader.GetValueByName("masterPolicyCopy", "0"),
            ModelName = reader.GetValueByName("modelName"),
            NeedSave = reader.GetValueByName("needSave", "1"),
            NettPremium = double.Parse(reader.GetValueByName("nettPremium", "0.00")),
            NextPageType = reader.GetValueByName("nextPageType"),
            OldAgreementId = reader.GetValueByName("oldAgreementId"),
            PageType = reader.GetValueByName("pageType", "1"),
            PayMode = reader.GetValueByName("payMode", "300"),
            PlanId = reader.GetValueByName("planId", "0"),
            PlatformCerti = reader.GetValueByName("platformCerti"),
            PolicyId = reader.GetValueByNameOrThrow("policyId"),
            PolicyProtocolFinalizedDateStr = reader.GetValueByName("policyProtocolFinalizedDateStr"),
            PremCurrencyCode = reader.GetValueByName("premCurrencyCode", "28"),
            PremiumPayable = double.Parse(reader.GetValueByName("premiumPayable", "0.00")),
            PrePageType = reader.GetValueByName("prePageType"),
            PreviousPolicyNo = reader.GetValueByName("previousPolicyNo"),
            ProductId = long.Parse(reader.GetValueByNameOrThrow("productId")),
            PropDate = reader.GetValueByName("propDate"),
            QuoDate = reader.GetValueByName("quoDate"),
            Id = long.Parse(reader.GetValueByNameOrThrow("quoteNo")),
            ReferenceDateEditableFlag = reader.GetValueByName("referenceDateEditableFlag", "N"),
            ReferenceDateType = reader.GetValueByName("referenceDateType", "1"),
            ReinsuranceReferenceDateStr = reader.GetValueByName("reinsuranceReferenceDateStr"),
            RenewalFlag = bool.Parse(reader.GetValueByName("renewalFlag", "true")),
            RenewalMemo = reader.GetValueByName("renewalMemo"),
            RewriteType = reader.GetValueByName("rewriteType"),
            SActionType = reader.GetValueByName("sActionType"),
            SalesPlatform = int.Parse(reader.GetValueByName("salesPlatform", "268")),
            SelectInstallmentMethod = reader.GetValueByName("selectInstallmentMethod", "null"),
            SelectInstallmentNumber = reader.GetValueByName("selectInstallmentNumber", "null"),
            SelectOptionId = reader.GetValueByName("selectOptionId", "-1"),
            SelectPaymentPlan = reader.GetValueByName("selectPaymentPlan", "null"),
            SelectPrepayFlag = reader.GetValueByName("selectPrepayFlag", "null"),
            SiCurrencyCode = reader.GetValueByName("siCurrencyCode", "28"),
            SpecRule = reader.GetValueByName("specRule"),
            SpecRuleForSchedule = reader.GetValueByName("specRuleForSchedule"),
            SubNo = reader.GetValueByName("subNo"),
            SyskeyPageToken = reader.GetValueByNameOrThrow("syskey_page_token"),
            SyskeyRequestToken = reader.GetValueByName("syskey_request_token"),
            TableName = reader.GetValueByName("tableName"),
            TariffPremium = double.Parse(reader.GetValueByName("tariffPremium", "0.00")),
        };

        var regex = new Regex(@"<OperatorId>(\d+)</OperatorId>");
        var match = regex.Match(pageHtml);

        if (match.Success)
            quotation.OperatorId = match.Groups[1].Value;
        else
            throw new NullReferenceException("Não foi possível extrair o operator id.");

        regex = new Regex(@"<OperatorName>([^<]+)</OperatorName>");
        match = regex.Match(pageHtml);

        if (match.Success)
            quotation.OperatorName = match.Groups[1].Value;
        else
            throw new NullReferenceException("Não foi possível extrair o operator name.");

        return quotation;
    }
}