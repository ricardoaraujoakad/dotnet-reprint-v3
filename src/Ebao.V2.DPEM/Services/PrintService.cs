using System.Text.RegularExpressions;
using Ebao.V2.DPEM.Api.Print;
using Ebao.V2.DPEM.Api.Print.Requests;
using Ebao.V2.DPEM.Helpers;
using Ebao.V2.DPEM.Models;
using Ebao.V2.DPEM.Models.ViewModels.Auth.Requests;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ebao.V2.DPEM.Services;

/// <summary>
/// Responsável por manipular a impressão de apólices no V3.
/// </summary>
public class PrintService : ServiceBase<PrintService>
{
    private readonly IMemoryCache _cache;
    private readonly UserService _userService;
    private readonly IPrintApi _printApi;
    private readonly AuthService _authService;

    public PrintService(ILogger<PrintService> logger, IMemoryCache cache, RequestInfo requestInfo,
        UserService userService, IPrintApi printApi, AuthService authService) : base(logger, cache, requestInfo)
    {
        _cache = cache;
        _userService = userService;
        _printApi = printApi;
        _authService = authService;
    }

    /// <summary>
    /// Prints the policy, processes the email, and sends the email document.
    /// </summary>
    /// <param name="policyNo">The policy number.</param>
    /// <returns>The final response after completing all steps.</returns>
    public async Task<string> PrintProcessAndSendEmailAsync(string policyNo)
    {
        using var _ = Logger.BeginScope("PolicyNo", policyNo);

        Logger.LogInformation("Starting policy printing process for policy number: {PolicyNo}", policyNo);

        var printResponse = await PrintPolicyAsync(policyNo);

        var (policyId, productCode) = ExtractPolicyIdAndProductCode(printResponse);

        var isPrintPreviewAvailable = await IsPrintPreviewAvailableAsync(productCode);

        var isDigitalSignatureAvailable = await IsDigitalSignatureAvailableAsync(policyId);

        var needCallNewZenvia = await NeedCallNewZenviaAsync();

        if (!string.IsNullOrEmpty(policyId))
        {
            await RequestEmailProcessAsync(policyId, policyNo);

            var emailDocumentResponse = await SendEmailDocumentAsync(policyNo, policyId);
            return emailDocumentResponse;
        }

        return printResponse;
    }

    /// <summary>
    /// Imprime a apólice.
    /// </summary>
    /// <param name="policyNo">Número da apólice a ser impressa.</param>
    /// <returns>O conteúdo HTML da apólice impressa.</returns>
    public async ValueTask<string> PrintPolicyAsync(string policyNo)
    {
        using var _ = Logger.BeginScope("PolicyNo", policyNo);

        Logger.LogInformation("Iniciando impressão da apólice {PolicyNo}.", policyNo);

        var propDateTo = DateTime.Now.ToString("dd/MM/yyyy");
        var request = new V3PrintRequest(policyNo, propDateTo, _userService.GetSyskeyRequestToken());

        Logger.LogInformation("Enviando requisição de impressão para a API.");
        var response = await _printApi.PrintPolicyAsync(request);

        return response;
    }

    /// <summary>
    /// Autentica o usuário antes de realizar operações.
    /// </summary>
    private async ValueTask AuthenticateAsync()
    {
        Logger.LogInformation("Autenticando usuário para impressão.");

        var authRequest = new AuthRequest()
        {
            Username = Config.IntegrationConfig.EbaoLogin,
            Password = Config.IntegrationConfig.EbaoPassword
        };

        await _authService.LoginAsync(authRequest);

        Logger.LogInformation("Autenticação concluída com sucesso.");
    }

    /// <summary>
    /// Extrai o número do certificado do HTML da apólice impressa.
    /// </summary>
    /// <param name="policyHtml">O conteúdo HTML da apólice impressa.</param>
    /// <returns>O número do certificado extraído.</returns>
    /// <exception cref="InvalidOperationException">Lançada quando o número do certificado não é encontrado.</exception>
    public string ExtractCertificateNumber(string policyHtml)
    {
        Logger.LogInformation("Extraindo número do certificado do HTML da apólice.");

        var pattern = @"Certificado\s*Nº:\s*(\d+)";
        var match = Regex.Match(policyHtml, pattern, RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            Logger.LogError("Não foi possível encontrar o número do certificado no HTML da apólice.");
            throw new InvalidOperationException("Número do certificado não encontrado no HTML da apólice.");
        }

        var certificateNumber = match.Groups[1].Value;
        Logger.LogInformation("Número do certificado extraído com sucesso: {CertificateNumber}", certificateNumber);

        return certificateNumber;
    }

    /// <summary>
    /// Salva o HTML da apólice impressa em um arquivo.
    /// </summary>
    /// <param name="policyHtml">O conteúdo HTML da apólice impressa.</param>
    /// <param name="policyNo">O número da apólice.</param>
    /// <returns>O caminho do arquivo salvo.</returns>
    public async ValueTask<string> SavePolicyHtmlAsync(string policyHtml, string policyNo)
    {
        var fileName = $"policy_{policyNo}_{DateTime.Now:yyyyMMddHHmmss}.html";
        var filePath = Path.Combine(Path.GetTempPath(), fileName);

        Logger.LogInformation("Salvando HTML da apólice {PolicyNo} em {FilePath}.", policyNo, filePath);

        await File.WriteAllTextAsync(filePath, policyHtml);

        Logger.LogInformation("HTML da apólice {PolicyNo} salvo com sucesso.", policyNo);

        return filePath;
    }

    /// <summary>
    /// Requests the email process for the policy.
    /// </summary>
    /// <param name="policyId">The extracted policy ID.</param>
    /// <param name="transactionNo">The transaction number (policy number).</param>
    /// <returns>The response content from the email process request.</returns>
    public async Task<string> RequestEmailProcessAsync(string policyId, string transactionNo)
    {
        Logger.LogInformation("Requesting email process for policy ID: {PolicyId}", policyId);

        var request = new EmailProcessRequest
        {
            SyskeyRequestToken = _userService.GetSyskeyRequestToken(),
            PolicyId = policyId,
            TransactionNo = transactionNo
        };

        try
        {
            var response = await _printApi.RequestEmailProcessAsync(request);
            Logger.LogInformation("Email process request successful for policy ID: {PolicyId}", policyId);
            return response;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error requesting email process for policy ID: {PolicyId}", policyId);
            throw;
        }
    }

    /// <summary>
    /// Sends the email document for the policy.
    /// </summary>
    /// <param name="policyNo">The policy number.</param>
    /// <param name="policyId">The policy ID.</param>
    /// <returns>The response content from the email document request.</returns>
    public async Task<string> SendEmailDocumentAsync(string policyNo, string policyId)
    {
        Logger.LogInformation("Sending email document for policy number: {PolicyNo}", policyNo);

        var request = new EmailDocumentRequest
        {
            SyskeyRequestToken = _userService.GetSyskeyRequestToken(),
            PolicyNo = policyNo,
            PolicyId = policyId,
            TransactionNo = policyNo
        };

        try
        {
            var response = await _printApi.SendEmailDocumentAsync(request);
            Logger.LogInformation("Email document sent successfully for policy number: {PolicyNo}", policyNo);
            return response;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error sending email document for policy number: {PolicyNo}", policyNo);
            throw;
        }
    }

    public (string PolicyId, string ProductCode) ExtractPolicyIdAndProductCode(string response)
    {
        string policyId = string.Empty;
        string productCode = string.Empty;

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(response);

        var policyIdInput = htmlDocument.DocumentNode.SelectSingleNode("//input[@name='policyIDs']");

        if (policyIdInput != null)
        {
            policyId = policyIdInput.GetAttributeValue("value", string.Empty);
        }

        var productCodeMatch = Regex.Match(response, @"<input\s+value=""(\w+)""\s+type=""hidden""\s+name=""policy_\d+_prdCode""");
        if (productCodeMatch.Success)
        {
            productCode = productCodeMatch.Groups[1].Value;
        }

        return (PolicyId: policyId, ProductCode: productCode);
    }

    /// <summary>
    /// Checks if print preview is available for the given product code.
    /// </summary>
    /// <param name="productCode">The product code to check.</param>
    /// <returns>The response from the print preview check.</returns>
    public async Task<string> IsPrintPreviewAvailableAsync(string productCode)
    {
        Logger.LogInformation("Checking print preview availability for product code: {ProductCode}", productCode);

        var request = new IsPrintPreviewRequest
        {
            SyskeyRequestToken = _userService.GetSyskeyRequestToken(),
            ProductCode = productCode
        };

        try
        {
            var response = await _printApi.IsPrintPreviewAsync(request);
            Logger.LogInformation("Print preview check successful for product code: {ProductCode}", productCode);
            return response;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error checking print preview for product code: {ProductCode}", productCode);
            throw;
        }
    }

    /// <summary>
    /// Checks if digital signature is available for the given policy ID.
    /// </summary>
    /// <param name="policyId">The policy ID to check.</param>
    /// <returns>The response from the digital signature check.</returns>
    public async Task<string> IsDigitalSignatureAvailableAsync(string policyId)
    {
        Logger.LogInformation("Checking digital signature availability for policy ID: {PolicyId}", policyId);

        var request = new IsDigitalSignatureRequest
        {
            SyskeyRequestToken = _userService.GetSyskeyRequestToken(),
            PolicyId = policyId
        };

        try
        {
            var response = await _printApi.IsDigitalSignatureAsync(request);
            Logger.LogInformation("Digital signature check successful for policy ID: {PolicyId}", policyId);
            return response;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error checking digital signature for policy ID: {PolicyId}", policyId);
            throw;
        }
    }

    /// <summary>
    /// Checks if a new Zenvia call is needed.
    /// </summary>
    /// <returns>The response from the Zenvia call check.</returns>
    public async Task<string> NeedCallNewZenviaAsync()
    {
        Logger.LogInformation("Checking if new Zenvia call is needed");

        var request = new NeedCallNewZenviaRequest
        {
            SyskeyRequestToken = _userService.GetSyskeyRequestToken()
        };

        try
        {
            var response = await _printApi.NeedCallNewZenviaAsync(request);
            Logger.LogInformation("Zenvia call check successful");
            return response;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error checking if new Zenvia call is needed");
            throw;
        }
    }
}
