namespace Ebao.V2.DPEM.Models.Template;

public class QuotationTemplate
{
    public ulong OperatorId { get; set; }
    public string OperatorName { get; set; }
    public string PolicyId { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public long QuotationId { get; set; }
    public DateTime ProposalDate { get; set; }
    public DateTime InsertTime { get; set; }
    public string VesselName { get; set; }
    public int Crews { get; set; }
    public long UseOfTheVessel { get; set; }
    public long TypeOfVessel { get; set; }
    public int Segment { get; set; }
    public int NumberOfPassengers { get; set; }
    public long NavigationType { get; set; }
    public long Propulsion { get; set; }
    public string VesselNumber { get; set; }
    public string Activity { get; set; }
    public decimal Lmi { get; set; }
}