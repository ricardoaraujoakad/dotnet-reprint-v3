using Ebao.V2.DPEM.Api.Payment.Responses;
using Refit;

namespace Ebao.V2.DPEM.Api.Payment.Requests
{
    public class V3PaymentRequest
    {
        [AliasAs("syskey_page_token")]
        public long SyskeyPageToken { get; set; }

        [AliasAs("amount")]
        public decimal Amount { get; set; }

        [AliasAs("currencyType")]
        public int CurrencyType => 28;

        [AliasAs("proposalNo")]
        public string ProposalNo { get; set; }

        [AliasAs("payMode")]
        public int PayMode => 300;

        [AliasAs("payor")]
        public int Payor => 2;

        [AliasAs("receiptAmount")]
        public decimal ReceiptAmount => Amount;

        [AliasAs("bankAccountId")]
        public int BankAccountId => 100341;

        [AliasAs("receiptDate")]
        public string ReceiptDate => DateTime.Now.ToString("dd/MM/yyyy");

        [AliasAs("receiveAmount")]
        public decimal ReceiveAmount => Amount;

        [AliasAs("pageno")]
        public int PageNo => 1;

        [AliasAs("sumPayAmount")]
        public decimal SumPayAmount => Amount;

        [AliasAs("accExchgRate")]
        public decimal AccExchgRate => 1;

        [AliasAs("proposalNoArray")]
        public string ProposalNoArray => $"{ProposalNo};";

        [AliasAs("covernoteNoArray")]
        public string CovernoteNoArray => ";";

        [AliasAs("agentCodeArray")]
        public string AgentCodeArray => "0011378;";

        [AliasAs("prepaidDepositArray")]
        public string PrepaidDepositArray => "0;";

        [AliasAs("receiveAmountArray")]
        public string ReceiveAmountArray => $"{Amount};";

        [AliasAs("payorCode")]
        public string PayorCode { get; }

        [AliasAs("policyId")]
        public string PolicyId { get; }

        [AliasAs("searchCurrency")]
        public int SearchCurrency => 28;

        [AliasAs("syskey_request_token")]
        public string SyskeyRequestToken { get; set; }

        public V3PaymentRequest(V3PaymentResponse paymentResponse, string syskeyRequestToken)
        {
            SyskeyPageToken = paymentResponse.SyskeyPageToken;
            Amount = paymentResponse.Premium;
            ProposalNo = paymentResponse.ProposalNo;
            PayorCode = paymentResponse.PayorCode;
            PolicyId = paymentResponse.PolicyId;
            SyskeyRequestToken = syskeyRequestToken;
        }
    }
}