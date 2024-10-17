using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ebao.V2.DPEM.Api.Print.Requests;
using Refit;

namespace Ebao.V2.DPEM.Api.Print
{
    public interface IPrintApi
    {
        [Post("/insurance/gs/servlet/API.gs.pol.issue.newbiz.ctrl.NewbizPrintAction")]
        Task<string> PrintPolicyAsync([Body(BodySerializationMethod.UrlEncoded)] V3PrintRequest request);

        [Get("/insurance/gs/servlet/com.ebao.lisig.integration.agpi.web.action.AgpiPrintAction")]
        Task<string> RequestEmailProcessAsync([Query] EmailProcessRequest request);

        [Get("/insurance/gs/servlet/com.ebao.lisig.integration.agpi.web.action.AgpiEmailAction")]
        Task<string> SendEmailDocumentAsync([Query] EmailDocumentRequest request);

        [Get("/insurance/gs/servlet/com.ebao.lisig.integration.agpi.web.action.AgpiPrintAction")]
        Task<string> IsPrintPreviewAsync([Query] IsPrintPreviewRequest request);

        [Get("/insurance/gs/servlet/com.ebao.lisig.integration.agpi.web.action.AgpiPrintAction")]
        Task<string> IsDigitalSignatureAsync([Query] IsDigitalSignatureRequest request);

        [Get("/insurance/gs/servlet/com.ebao.lisig.integration.agpi.web.action.AgpiPrintAction")]
        Task<string> NeedCallNewZenviaAsync([Query] NeedCallNewZenviaRequest request);
    }
}
