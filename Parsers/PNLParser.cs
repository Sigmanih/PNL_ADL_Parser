using System;
using System.Globalization;
using PNL_ADL_Parser.Models;

namespace PNL_ADL_Parser.Parsers;

public class PNLParser
{
    /// <summary>
    /// Esegue il parsing di un array di stringhe che rappresentano un file PNL (Passenger Name List) e restituisce un oggetto <see cref="FlightDetails"/>.
    /// </summary>
    /// <param name="fileLines">Un array di stringhe contenente il contenuto del file PNL.</param>
    /// <returns>Un oggetto <see cref="FlightDetails"/> contenente i dettagli del volo e le informazioni sui passeggeri.</returns>
    /// <exception cref="ArgumentException">Viene generata quando il file di input è vuoto o nullo.</exception>
    /// <exception cref="FormatException">Viene generata quando si verifica un errore durante il parsing di una riga del passeggero.</exception>
    public FlightDetails Parse(string[] fileLines)
    {
        if (fileLines == null || fileLines.Length == 0)
            throw new ArgumentException("Il file di input è vuoto o nullo");
        //dettagli del volo. Ha List<PassengerDetails>
        var flightDetails = new FlightDetails
        {
            Passengers = new List<PassengerDetails>()
        };

        foreach (var line in fileLines)
        {
            // Parsing dell'intestazione del volo
            if (line.StartsWith("PNL", StringComparison.OrdinalIgnoreCase))
            {
                ParseFlightHeader(line,fileLines[1], flightDetails);
            }
            // Parsing dei dettagli del passeggero
            else if (line.StartsWith("1"))
            {
                int index = Array.IndexOf(fileLines, line);
                try
                {
                    var passenger = ParsePassenger(line);
                    flightDetails.Passengers.Add(passenger);
                }
                catch (Exception ex)
                {
                    throw new FormatException($"Errore nel parsing della riga del passeggero: {line}", ex);
                }
            }
        }

        // Calcola il numero di passeggeri
        flightDetails.PassengerCount = flightDetails.Passengers.Count;
        return flightDetails;
    }

    /// <summary>
    /// Esegue il parsing delle informazioni sull'intestazione del volo dalla riga fornita e popola l'oggetto <see cref="FlightDetails"/>.
    /// </summary>
    /// <param name="line">Una stringa contenente le informazioni sull'intestazione del volo. Dovrebbe iniziare con "PNL" e contenere numero di volo, data e rotta.</param>
    /// <param name="flightDetails">L'oggetto <see cref="FlightDetails"/> da popolare con le informazioni estratte.</param>
    /// <exception cref="FormatException">Viene generata quando il formato della riga è invalido o non contiene abbastanza informazioni.</exception>
    private void ParseFlightHeader(string line,string line1, FlightDetails flightDetails)
    {
        // Verifica che la riga inizi con "PNL"
        if (!line.StartsWith("PNL", StringComparison.OrdinalIgnoreCase))
        {
            throw new FormatException($"Formato dell'intestazione del volo non valido: {line}");
        }

        // Dividi la riga con separatori misti (spazi e slash)
        var parts = line1.Split(new[] { ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);

        // Verifica che ci siano abbastanza parti
        if (parts.Length < 3)
        {
            throw new FormatException($"Formato dell'intestazione del volo non valido: {line}");
        }

        // Assegna i valori alle proprietà dell'oggetto flightDetails
        flightDetails.FlightNumber = parts[0]; // Es: NO6149
        flightDetails.FlightDate = ParseFlightDate(parts[1]); // Es: 07SEP
        flightDetails.Route = parts[2]; // Es: RHO
    }

    /// <summary>
    /// Esegue il parsing della data del volo a partire da una stringa nel formato "ddMMM".
    /// </summary>
    /// <param name="dateRaw">La stringa contenente la data del volo, es. "07SEP".</param>
    /// <returns>Un oggetto <see cref="DateTime"/> che rappresenta la data del volo.</returns>
    /// <exception cref="FormatException">Viene generata quando non è possibile parsare la data del volo.</exception>
    private DateTime ParseFlightDate(string dateRaw)
    {
        var format = "ddMMM"; // Formato della data: 07SEP
        var year = DateTime.Now.Year; // Utilizza l'anno corrente

        if (!DateTime.TryParseExact(dateRaw + year, format + "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            throw new FormatException($"Impossibile parsare la data del volo: {dateRaw}");
        }

        return parsedDate;
    }

    /// <summary>
    /// Esegue il parsing di una riga che rappresenta un passeggero e restituisce un oggetto <see cref="PassengerDetails"/>.
    /// </summary>
    /// <param name="line">La stringa contenente le informazioni del passeggero. Es: "1ALBANESI/MARCELLOMR.R/TOP AL.L/655F43.R/PDBG HK1 BAGS 01".</param>
    /// <returns>Un oggetto <see cref="PassengerDetails"/> contenente le informazioni del passeggero.</returns>
    /// <exception cref="FormatException">Viene generata quando il formato della riga del passeggero è invalido.</exception>
    private PassengerDetails ParsePassenger(string line)
    {
        // Esempio di riga: "1ALBANESI/MARCELLOMR.R/TOP AL.L/655F43.R/PDBG HK1 BAGS 01"
        var parts = line.Split('/');
        
        // Verifica che la riga contenga almeno 2 parti
        if (parts.Length < 2)
            throw new FormatException($"Formato del passeggero non valido: {line}");

        // Estrai il cognome rimuovendo il prefisso "1"
        var lastName = parts[0].Substring(1); 

        // Estrai il nome e il tipo passeggero separati da un trattino
        var firstNameAndType = parts[1].Split('-', StringSplitOptions.RemoveEmptyEntries);

        // Verifica che ci siano almeno 2 parti (nome e tipo passeggero)
        if (firstNameAndType.Length < 2)
            throw new FormatException($"Nome o tipo di passeggero non valido: {parts[1]}");

        var firstName       = firstNameAndType[0];
        var passengerType   = firstNameAndType[1];
        var pnr             = firstNameAndType[1];
        // Restituisci un oggetto PassengerDetails popolato
        return new PassengerDetails
        {
            LastName = lastName,
            FirstName = firstName,
            PassengerType = passengerType,
            SpecialRequests = new List<string>(), // Placeholder per richieste speciali
            Baggage = new List<BaggageDetails>()  // Placeholder per dettagli bagagli
        };
    }
}
