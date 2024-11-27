namespace PNL_ADL_Parser.Models;

public class BaggageDetails
{
    public string Type { get; set; } // e.g., BAGS, XBAG
    public string Status { get; set; } // e.g., HK1, 7032015262740
    public string ?Quantity { get; set; } 
    public double ?Weight { get; set; } // in KG
}
