namespace PNL_ADL_Parser.Models;

public class PassengerDetails
{
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string PassengerType { get; set; } // e.g., MR, MRS, CHLD
    public List<string> SpecialRequests { get; set; } = new List<string>();
    public List<BaggageDetails> Baggage { get; set; } = new List<BaggageDetails>();
}
