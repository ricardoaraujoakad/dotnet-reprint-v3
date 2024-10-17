using Refit;

namespace Ebao.V2.DPEM.Api.Print.Requests
{
    public class EmailPreviewRequest
    {
        [AliasAs("syskey_request_token")]
        public string SyskeyRequestToken { get; set; }

        [AliasAs("actionType")]
        public string ActionType { get; set; } = "preview";

        [AliasAs("docDecision")]
        public string DocDecision { get; set; } = "email";

        [AliasAs("transactionType")]
        public string TransactionType { get; set; } = "Issue";

        [AliasAs("transaction")]
        public string Transaction { get; set; } = "SP Policy issuance";

        [AliasAs("agpi")]
        public string Agpi { get; set; } = "emailProcess";

        [AliasAs("transactionNo")]
        public string TransactionNo { get; set; }

        [AliasAs("policyId")]
        public string PolicyId { get; set; }

        [AliasAs("policyNo")]
        public string PolicyNo { get; set; }

        [AliasAs("fromProcess")]
        public string FromProcess { get; set; } = "IssuancePrintingProcess";

        [AliasAs("_")]
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
