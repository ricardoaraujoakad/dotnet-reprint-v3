using Refit;

namespace Ebao.V2.DPEM.Api.Print.Requests
{
    public class EmailActionDecisionRequest
    {
        [AliasAs("syskey_request_token")]
        public string SyskeyRequestToken { get; set; }

        [AliasAs("actionType")]
        public string ActionType { get; set; } = "decision";

        [AliasAs("localPrint")]
        public bool LocalPrint { get; set; } = true;
    }
}
