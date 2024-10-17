using Refit;

namespace Ebao.V2.DPEM.Api.Quotation.Requests
{
    public record V3AssignQuotationRequest
    {
        [AliasAs("actorId")]
        public string ActorId { get; set; }

        [AliasAs("compareTo")]
        public string CompareTo { get; set; } = "1";

        [AliasAs("digitalEmail")]
        public string DigitalEmail { get; set; } = "";
        
        [AliasAs("docDecision")]
        public string DocDecision { get; set; } = "";
        
        [AliasAs("estartLoseContent")]
        public string EstartLoseContent { get; set; } = "";
        
        [AliasAs("rejDesc")]
        public string RejDesc { get; set; } = "";
        
        [AliasAs("transaction")]
        public string Transaction { get; set; } = "";
        
        [AliasAs("transactionType")]
        public string TransactionType { get; set; } = "";
        
        [AliasAs("operId")]
        public string OperId => "QQ_TO_MP";

        [AliasAs("policyCate")]
        public int PolicyCate { get; } = 1;

        [AliasAs("rejCode")]
        public int RejCode { get; } = 3;

        [AliasAs("generatePolicyDocFlagValue")]
        public int GeneratePolicyDocFlagValue { get; } = 1;

        [AliasAs("syskey_page_token")]
        public long SyskeyPageToken { get; }

        [AliasAs("syskey_request_token")]
        public string SyskeyRequestToken { get; }

        [AliasAs("transition")]
        public string Transition { get; set; } = "QQ_TO_MP";

        public V3AssignQuotationRequest(string actorId, long syskeyPageToken, string syskeyRequestToken)
        {
            ActorId = actorId;
            SyskeyPageToken = syskeyPageToken;
            SyskeyRequestToken = syskeyRequestToken;
        }
    }
}
