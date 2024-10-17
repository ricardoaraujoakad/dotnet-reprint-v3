namespace Ebao.V2.DPEM.Models.ViewModels.Quotation.Requests;

public class PolicyRequest
{
    public string Certificate { get; set; }
    public decimal ExpectedPremium { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime ProtocolDate { get; set; }
    public decimal Lmi { get; set; }
    public VesselRequest Vessel { get; set; }
    public InsuredRequest Insured { get; set; }
    public IReadOnlyList<CoverageRequest> Coverages { get; set; }
    public double CommissionRate { get; set; }
}