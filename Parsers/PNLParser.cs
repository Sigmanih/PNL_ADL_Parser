using System;
using System.Globalization;
using System.Text;
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
        //salva header dell'aereo
        if (fileLines[0].StartsWith("PNL", StringComparison.OrdinalIgnoreCase))
        {
            ParseFlightHeader(fileLines[0],fileLines[1], flightDetails);
        }

        List<string> currentPassengerLines = new List<string>();
        
        // Itera dalle righe successive, partendo dalla quarta
        for (int i = 3; i < fileLines.Length; i++)
        {
            if (fileLines[i].StartsWith("1") && fileLines[i] != ""){
                try
                {
                    if(currentPassengerLines.Count == 0){
                        currentPassengerLines.Add(fileLines[i]);
                    }else{
                    //inserisci il vecchio passeggero 
                    var passenger = ParsePassenger(currentPassengerLines);
                    flightDetails.Passengers.Add(passenger);
                    currentPassengerLines.Clear();
                    currentPassengerLines.Add(fileLines[i]);
                    }
                }
                catch (Exception ex)
                {
                    throw new FormatException($"Errore nel parsing della riga del passeggero: {fileLines[i]}", ex);
                }
            }else if(fileLines[i].StartsWith("ENDPNL")){
                var passenger = ParsePassenger(currentPassengerLines);
                flightDetails.Passengers.Add(passenger);
                currentPassengerLines.Clear();
            }
            else{
                currentPassengerLines.Add(fileLines[i]);
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
    private PassengerDetails ParsePassenger(List<string> lines)
    {
        // Inizializza la lista delle richieste speciali e dei bagagli
        PassengerDetails        passengerDetails    = new PassengerDetails();
        List<AdditionalInfo>    AdditionalInfo      = new List<AdditionalInfo>();
        List<BaggageDetails>    baggageDetails      = new List<BaggageDetails>();
        //var specialRequests = new List<string>();
        passengerDetails.SpecialRequests = AdditionalInfo;
        passengerDetails.Baggage = baggageDetails;
        // Verifica che la riga del nome e cognome contenga almeno 2 parti
        string line = lines[0];
        var parts = line.Split('/');
        if (parts.Length < 2)
            throw new FormatException($"Formato nome e cognome del passeggero non valido: {line}");

        // Estrai il cognome rimuovendo il prefisso "1"
        passengerDetails.LastName = parts[0].Substring(1); 

        // Estrai il nome e il tipo passeggero separati da un trattino
        var firstNameAndType = parts[1].Split('-', StringSplitOptions.RemoveEmptyEntries);

        // Verifica che ci siano almeno 2 parti (nome e tipo passeggero)
        if (firstNameAndType.Length < 2)
            throw new FormatException($"Nome o tipo di passeggero non valido: {parts[1]}");

        passengerDetails.FirstName       = firstNameAndType[0];
        passengerDetails.PassengerType   = firstNameAndType[1];
        var status = "";
        var type = "";
        var quantity = 0;
        Ticket ticket;
        for (int i = 1; i < lines.Count; i++){
            var currentLine = lines[i];
            switch (currentLine)
                {
                    case var _ when currentLine.StartsWith(".L"):
                        passengerDetails.PNR = currentLine;
                        break;

                    case var _ when currentLine.StartsWith(".R/TOP"):
                        passengerDetails.TourOperator = currentLine.Replace(".R/TOP", "").Trim(); // Rimuove ".R/TOP" e pulisce gli spazi
                        break;

                    case var _ when currentLine.StartsWith(".R/PDBG"):
                        // Rimuove il prefisso e separa i componenti
                        var pdbgDetails = currentLine.Replace(".R/PDBG", "").Trim();
                        var pdbgParts = pdbgDetails.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (pdbgParts.Length < 3)
                            throw new FormatException($"Formato non valido per la riga bagagli: {currentLine}");

                        // Estrae i dettagli
                        status = pdbgParts[0]; // HK1
                        type = pdbgParts[1];   // BAGS
                        quantity = int.TryParse(pdbgParts[2], out var count) ? count : 0; // 01 -> 1

                        // Aggiunge i dettagli del bagaglio
                        baggageDetails.Add(new BaggageDetails
                        {
                            Type = type,
                            Status = status,
                            Weight = 0, // Default (peso non specificato in questa riga)
                            ExtraWeight = 0, // Default
                        });
                        break;

                    case var _ when currentLine.StartsWith(".R/XBAG"):
                        // Rimuove il prefisso e separa i componenti
                        var xbagDetails = currentLine.Replace(".R/XBAG", "").Trim();
                        var xbagParts = xbagDetails.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (xbagParts.Length < 3)
                            throw new FormatException($"Formato non valido per la riga bagaglio extra: {currentLine}");

                        // Estrae i dettagli
                        status = xbagParts[0]; // HK1
                        var weight = double.TryParse(xbagParts[1].Replace("KG", ""), out var kg) ? kg : 0; // 15KG -> 15
                        var extraInfo = xbagParts[2]; // FREE

                        // Aggiunge i dettagli del bagaglio extra
                        baggageDetails.Add(new BaggageDetails
                        {
                            Type = "XBAG",
                            Status = status,
                            Weight = weight, // Peso del bagaglio
                            ExtraWeight = weight, // Peso extra coincide per XBAG
                        });
                        break;

                    case var _ when currentLine.StartsWith(".R/TKNE"):
                        // Rimuove il prefisso e separa i componenti
                        var tkneDetails = currentLine.Replace(".R/TKNE", "").Trim();
                        var tkneParts = tkneDetails.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (tkneParts.Length < 2)
                            throw new FormatException($"Formato non valido per il numero del biglietto elettronico: {currentLine}");

                        var ticketStatus = tkneParts[0]; // HK1
                        var ticketNumber = tkneParts[1].Split('/')[0]; // 7032015262740 (estrae solo il numero)

                        // Salva il biglietto elettronico come informazione addizionale
                        passengerDetails.ElectronicTicket = new Ticket
                        {
                            Code = ticketNumber,
                            Status = ticketStatus
                        };

                        // Salva l'oggetto Ticket nel passeggero
                        break;

                    case var _ when currentLine.StartsWith(".R/CHLD"):
                        var childDetails = currentLine.Replace(".R/CHLD", "").Trim();
                        var childStatus = childDetails.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]; // HK1

                        // Aggiungi l'informazione alla lista AdditionalInfo
                        AdditionalInfo.Add(new AdditionalInfo
                        {
                            Type = "CHILD",
                            Status = childStatus
                        });
                        break;

                    case var _ when currentLine.StartsWith(".R/RQST"):
                        // Rimuove il prefisso
                        var rqstDetails = currentLine.Replace(".R/RQST", "").Trim();
                        var rqstParts = rqstDetails.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (rqstParts.Length < 2)
                            throw new FormatException($"Formato non valido per la richiesta del passeggero: {currentLine}");
                        
                        var requestStatus = rqstParts[0]; // HK1
                        var requestSeat = rqstParts[1]; // Es. 12A
                        // Aggiunge la richiesta specifica
                        AdditionalInfo.Add(new AdditionalInfo
                            {
                                Type = "REQUEST",
                                Status = requestStatus,
                                SeatRequest = requestSeat
                            });
                        break;

                }
        }



        // Restituisci un oggetto PassengerDetails popolato
        return passengerDetails;
    }

public string GeneratePNLFile(FlightDetails flightDetails)
{
    StringBuilder pnlContent = new StringBuilder();

    // Aggiungi i dettagli di volo (header)
    pnlContent.AppendLine($"PNL {flightDetails.FlightNumber} {flightDetails.Route} {flightDetails.FlightDate:yyyy-MM-dd}");

    // Aggiungi ogni passeggero
    foreach (var passenger in flightDetails.Passengers)
    {
        // Header del passeggero (una riga per ogni passeggero)
        pnlContent.AppendLine($"1{passenger.LastName}/{passenger.FirstName}{passenger.PassengerType}");

        // PNR
        if (!string.IsNullOrEmpty(passenger.PNR))
        {
            pnlContent.AppendLine($" .L/{passenger.PNR}");
        }

        // Dettagli del biglietto elettronico
        if (passenger.ElectronicTicket != null)
        {
            pnlContent.AppendLine($" .R/TKNE {passenger.ElectronicTicket.Code} {passenger.ElectronicTicket.Status}");
        }

        // Baggage (bagagli)
        foreach (var baggage in passenger.Baggage)
        {
            pnlContent.AppendLine($" .R/{baggage.Type} {baggage.Status} {baggage.Weight}KG {baggage.ExtraWeight}KG");
        }

        // Informazioni aggiuntive
        foreach (var info in passenger.SpecialRequests)
        {
            if (info.Type == "CHLD")
            {
                pnlContent.AppendLine($" .R/CHLD {info.Status}");
            }
            else if (info.Type == "RQST")
            {
                pnlContent.AppendLine($" .R/RQST {info.Status} {info.SeatRequest}");
            }
        }

        // Aggiungi una linea vuota per separare ogni passeggero
        pnlContent.AppendLine();
    }

    // Restituisci il contenuto del file PNL
    return pnlContent.ToString();
}


}
