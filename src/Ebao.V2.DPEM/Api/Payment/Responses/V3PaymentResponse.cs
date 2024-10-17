namespace Ebao.V2.DPEM.Api.Payment.Responses;

public class V3PaymentResponse
{
    public decimal Premium { get; set; }
    public string PayorCode { get; set; }
    public string PolicyId { get; set; }
    public string ProposalNo { get; set; }
    public long SyskeyPageToken { get; set; }
    public string ReceiptNo { get; set; }

    public V3PaymentResponse(decimal premium, string payorCode, string policyId, string proposalNo,
        long syskeyPageToken)
    {
        Premium = premium;
        PayorCode = payorCode;
        PolicyId = policyId;
        ProposalNo = proposalNo;
        SyskeyPageToken = syskeyPageToken;
    }
}