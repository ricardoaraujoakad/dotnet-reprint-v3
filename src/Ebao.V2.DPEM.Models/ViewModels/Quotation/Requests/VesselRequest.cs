namespace Ebao.V2.DPEM.Models.ViewModels.Quotation.Requests;

public class VesselRequest
{
    public string Name { get; set; }
    public string Number { get; set; }
    public int PropulsionId { get; set; }
    public int TypeId { get; set; }
    public int NavigationTypeId { get; set; }
    public int UseOfTheVesselId { get; set; }
    public int SegmentId { get; set; }
    public int TransportTypeId { get; set; }
    public string ActivityId { get; set; }
    public int Crews { get; set; }
    public int NumberOfPassengers { get; set; }
}