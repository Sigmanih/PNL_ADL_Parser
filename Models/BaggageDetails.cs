namespace PNL_ADL_Parser.Models;

public class BaggageDetails
{
    public string Type { get; set; } // e.g., BAGS, XBAG
    public string Status { get; set; } // e.g., HK1, 7032015262740
    public double Weight { get; set; } // in KG
    public double ExtraWeight { get; set; } // in KG (optional, for XBAG)
}
