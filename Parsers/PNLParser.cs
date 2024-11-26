using PNL_ADL_Parser.Models;

namespace PNL_ADL_Parser.Parsers;

public class PNLParser
{
    public FlightDetails Parse(string[] fileLines)
    {
        var flightDetails = new FlightDetails();

        foreach (var line in fileLines)
        {
            // Esempio di parsing base
            if (line.StartsWith("PNL"))
            {
                var parts = line.Split(' ');
                flightDetails.FlightNumber = parts[1];
                flightDetails.FlightDate = DateTime.ParseExact(parts[2], "ddMMM", null);
                flightDetails.Route = parts[3];
            }
            else if (line.StartsWith("1")) // Dettagli passeggeri
            {
                var passenger = ParsePassenger(line);
                flightDetails.Passengers.Add(passenger);
            }
        }

        flightDetails.PassengerCount = flightDetails.Passengers.Count;
        return flightDetails;
    }

    private PassengerDetails ParsePassenger(string line)
    {
        // Esempio semplice di parsing passeggeri
        var parts = line.Split('/');
        return new PassengerDetails
        {
            LastName = parts[0].Substring(1), // Esclude il primo carattere (es. "1")
            FirstName = parts[1].Split('-')[0],
            PassengerType = parts[1].Split('-')[1]
        };
    }
}
