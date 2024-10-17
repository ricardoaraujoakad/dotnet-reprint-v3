using Ebao.V2.DPEM.Api.Coverage.Requests;
using Ebao.V2.DPEM.Models.Contants;
using Refit;

namespace Ebao.V2.DPEM.Api.Coverage;

public interface ICoverageApi
{
    [Post("/insurance/gs/servlet/API.gs.pol.core.pc.ctrl.CtAcceAction")]
    Task<string> AddCoverageAsync([Body(BodySerializationMethod.UrlEncoded)] AddCoverageRequest request);

    [Post("/insurance/gs/servlet/API.lisig.gs.pol.core.pc.ctrl.LisigShareSIAjaxAction")]
    Task<string> PrepareXMLAsync([Body(BodySerializationMethod.UrlEncoded)] PrepareXMLRequest request,
        [Query] [AliasAs(Parameters.Syskey)] string sysKey = "");
}