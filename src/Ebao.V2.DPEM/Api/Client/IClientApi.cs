using Ebao.V2.DPEM.Api.Client.Requests;
using Refit;

namespace Ebao.V2.DPEM.Api.Client;

public interface IClientApi
{
    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.CustomerWebAction")]
    Task<string> FindClientAsync([Body(BodySerializationMethod.UrlEncoded)] V3SearchByIdentityRequest body);

    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> AssignClientQuotationAsync([Body(BodySerializationMethod.UrlEncoded)] V3AssignClientRequest body);

    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.CustomerWebAction")]
    Task<string> CreateClientAsync([Body(BodySerializationMethod.UrlEncoded)] object body);

    [Post("/insurance/party/PartyRoleQueryAction.do")]
    Task<string> CheckCompanyPartyAsync([Body(BodySerializationMethod.UrlEncoded)] V3CompanyQueryRequest body);
}