namespace Ebao.V2.DPEM.Api.Binding.Responses;

public class BatchResponse
{
    public ulong Id { get; set; }
    public ulong ProposalId { get; set; }
    public string XmlMessage { get; set; }
}