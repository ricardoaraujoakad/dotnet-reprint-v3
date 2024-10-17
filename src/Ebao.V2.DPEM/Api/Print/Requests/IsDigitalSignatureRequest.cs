using Refit;

namespace Ebao.V2.DPEM.Api.Print.Requests
{
    public class IsDigitalSignatureRequest
    {
        [AliasAs("syskey_request_token")]
        public string SyskeyRequestToken { get; set; } = string.Empty;

        [AliasAs("actionType")]
        public string ActionType { get; set; } = "isDigitalSignature";

        [AliasAs("policyId")]
        public string PolicyId { get; set; } = string.Empty;
    }
}
