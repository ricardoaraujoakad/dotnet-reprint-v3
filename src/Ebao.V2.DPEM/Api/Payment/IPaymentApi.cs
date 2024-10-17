using Ebao.V2.DPEM.Api.Payment.Requests;
using Ebao.V2.DPEM.Models.Contants;
using Refit;

namespace Ebao.V2.DPEM.Api.Payment;

public interface IPaymentApi
{
    [Post("/insurance/gs/servlet/API.gs.bcp.ctrl.arap.prepaid.SearchPremiumAction")]
    Task<string> SearchPaymentAsync([Body(BodySerializationMethod.UrlEncoded)] V3SearchPremiumActionRequest body);

    [Get("/insurance/gs/servlet/API.gs.bcp.ctrl.arap.prepaid.ViewPremiumPreAction")]
    Task<string> LoadPrePaymentPageAsync([Query] [AliasAs("current_module_id")] int currentModuleId = 100115,
        [Query] [AliasAs(Parameters.Syskey)] string sysKey = "");
    
    [Post("/insurance/gs/servlet/API.gs.bcp.ctrl.arap.prepaid.PremiumPrepaidAction")]
    Task<string> SetPaymentAsync([Body(BodySerializationMethod.UrlEncoded)] V3PaymentRequest body);
}