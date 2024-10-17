using System.Xml.Linq;
using Ebao.V2.DPEM.Api.Coverage;
using Ebao.V2.DPEM.Api.Coverage.Requests;
using Ebao.V2.DPEM.Helpers;
using Ebao.V2.DPEM.Helpers.Templates;
using Ebao.V2.DPEM.Models;
using Ebao.V2.DPEM.Models.Template;
using Ebao.V2.DPEM.Models.ViewModels.Quotation.Requests;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Ebao.V2.DPEM.Helpers.Extensions;

namespace Ebao.V2.DPEM.Services;

public class CoverageService : ServiceBase<CoverageService>
{
    private readonly UserService _userService;
    private readonly ICoverageApi _coverageApi;
    private string? _insuredId;

    public CoverageService(ILogger<CoverageService> logger, IMemoryCache cache, RequestInfo requestInfo,
        ICoverageApi coverageApi, UserService userService) : base(logger, cache, requestInfo)
    {
        _coverageApi = coverageApi;
        _userService = userService;
    }

    public async ValueTask<long> AddCoverageAsync(CoverageRequest coverage, string policyId, long syskeyPageToken,
        string pageId)
    {
        using var _ = Logger.BeginScope("CoverageId", coverage.Id);

        Logger.LogInformation("Iniciando adição das coberturas.");
        var insuredId = await GetInsuredIdAsync(pageId);

        var modelTemplate = new CoverageTemplate()
        {
            InsertTime = DateTime.Now,
            InsuredId = insuredId,
            Lmi = coverage.Lmi
        };

        var writer = new TemplateWriter(new TemplateInfo("", "Coverage"));
        var xml = await writer.GenerateAsync(modelTemplate);

        var request = new AddCoverageRequest(coverage.Id, policyId, xml, _userService.GetSyskeyRequestToken(),
            syskeyPageToken);

        Logger.LogInformation("Enviando request para adicionar cobertura.");
        var html = await _coverageApi.AddCoverageAsync(request);

        var reader = HtmlReader.CreateReader(html);
        var sysKeyPage = long.Parse(reader.GetValueFromXPathOrThrow("//*[@name='syskey_page_token']"));

        Logger.LogInformation("Coberuras adicionadas com sucesso. Novo syskey_page_token: {SysKeyPage}", sysKeyPage);
        return sysKeyPage;
    }

    private async ValueTask<string> GetInsuredIdAsync(string pageId)
    {
        Logger.LogInformation("Preparando para obter o InsuredId.");

        if (!string.IsNullOrEmpty(_insuredId))
        {
            Logger.LogInformation("InsuredId já foi obtido, re-utilizando.");
            return _insuredId;
        }

        var modelTemplate = new CoverageTemplate()
        {
            InsertTime = DateTime.Now
        };

        var writer = new TemplateWriter(new TemplateInfo("", "PreCoverage"));
        var templateXml = await writer.GenerateAsync(modelTemplate);

        Logger.LogInformation("Enviando request para obter o InsuredId.");
        var request = new PrepareXMLRequest(templateXml, pageId);
        var xml = await _coverageApi.PrepareXMLAsync(request);
        var xdoc = XDocument.Parse(xml);

        _insuredId = xdoc.Descendants("InsuredId").FirstOrDefault()?.Value;

        if (string.IsNullOrEmpty(_insuredId))
            throw new InvalidOperationException("Não foi possível obter o InsuredId.");

        Logger.LogInformation("InsuredId obtido com sucesso. Valor: {InsuredId}", _insuredId);
        return _insuredId;
    }
}