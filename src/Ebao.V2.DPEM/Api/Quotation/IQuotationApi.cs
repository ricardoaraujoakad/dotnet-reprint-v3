using Ebao.V2.DPEM.Api.Quotation.Requests;
using Ebao.V2.DPEM.Api.Quotation.Responses;
using Ebao.V2.DPEM.Models.Contants;
using Refit;

namespace Ebao.V2.DPEM.Api.Quotation;

public interface IQuotationApi
{
    [Post("/insurance/gs/servlet/com.ebao.gs.pol.nb.action.NewBizAction")]
    Task<string> GetWorkListAsync([Body(BodySerializationMethod.UrlEncoded)] V3WorklistRequest worklist,
        [Query] string moduleRadio = "QuickQuote", [Query] string operId = "AddNBTask");

    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> QuickQuoteAsync([Body(BodySerializationMethod.UrlEncoded)] V3QuickQuoteRequest quickQuote);

    [Get("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> LoadPageQuickQuotationAsync([Query] string operId = "LoaderDataEntryTemplet",
        [Query] [AliasAs(Parameters.Syskey)] string sysKey = "");

    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> UpdateQuotationAsync([Body(BodySerializationMethod.UrlEncoded)] V3QuotationResponse quotation);

    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> LoadPageDecisionAsync(
        [Body(BodySerializationMethod.UrlEncoded)] V3PageDecisionRequest body,
        [Query] int isFrameSumbit = 1,
        [Query] string forwardToAnotherOperId = "LoaderDEDecision",
        [Query] int selVersion = 1
    );

    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> AssignQuotationAsync([Body(BodySerializationMethod.UrlEncoded)] V3AssignQuotationRequest body);

    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> SendToUnderwritingAsync([Body(BodySerializationMethod.UrlEncoded)] V3PrepareDeTransactionBody body);

    [Post("/insurance/pub/workflow/GetProcWorkList.do")]
    Task<string> SearchQuotationAsync([Body(BodySerializationMethod.UrlEncoded)] V3SearchQuotationRequest body);

    [Post("/insurance/pub/workflow/TaskOperateUICAction.do")]
    Task<string> LoadQuotationAsync(
        [Body(BodySerializationMethod.UrlEncoded)] V3SearchQuotationRequest body,
        [Query] string taskId);

    [Get("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> LoadPaymentInfoAsync([Query] string operId = "LoaderMPPayment",
        [Query] [AliasAs(Parameters.Syskey)] string sysKey = "");

    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> UpdatePaymentInfoAsync([Body(BodySerializationMethod.UrlEncoded)] V3UpdatePaymentRequest request);

    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> SendToBoundAsync([Body(BodySerializationMethod.UrlEncoded)] V3QuoteBoundRequest request);

    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> DispatchBoundAsync([Body(BodySerializationMethod.UrlEncoded)] V3DispatchRequest request);

    [Post("/insurance/gs/servlet/API.gs.pol.nb.action.NewBizAction")]
    Task<string> IssueAsync([Body(BodySerializationMethod.UrlEncoded)] V3IssueRequest request);
}