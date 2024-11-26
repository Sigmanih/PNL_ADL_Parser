namespace PNL_ADL_Parser.Models;

public class FlightDetails
{
    public string FlightNumber { get; set; }
    public string Route { get; set; }
    public DateTime FlightDate { get; set; }
    public int PassengerCount { get; set; }
    public List<PassengerDetails> Passengers { get; set; } = new List<PassengerDetails>();
}
