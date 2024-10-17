using Ebao.V2.DPEM.Api.Broker.Requests;
using Refit;

namespace Ebao.V2.DPEM.Api.Broker;

public interface IBrokerApi
{
    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> LoadPageBrokerAsync([Body(BodySerializationMethod.UrlEncoded)] LoadPageBrokerRequest body,
        [Query] string forwardToAnotherOperId = "LoaderDECommInfo");

    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> UpdateBrokerAsync([Body(BodySerializationMethod.UrlEncoded)] UpdateBrokerRequest body);
}