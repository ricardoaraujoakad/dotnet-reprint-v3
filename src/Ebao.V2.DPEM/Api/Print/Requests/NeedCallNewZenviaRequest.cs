using Refit;

namespace Ebao.V2.DPEM.Api.Print.Requests
{
    public class NeedCallNewZenviaRequest
    {
        [AliasAs("syskey_request_token")]
        public string SyskeyRequestToken { get; set; } = string.Empty;

        [AliasAs("actionType")]
        public string ActionType { get; set; } = "needCallNewZenvia";
    }
}
