namespace PNL_ADL_Parser.Models;

public class PassengerDetails
{
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string ?TourOperator { get; set; }
    public string ?PNR { get; set; } // Passenger Name Record (PNR), rappresenta il codice univoco della prenotazione.
    public string ?PassengerType { get; set; } // e.g., MR, MRS, CHLD
    public Ticket ?ElectronicTicket { get; set; }
    public List<AdditionalInfo> ?SpecialRequests { get; set; }  = new List<AdditionalInfo>();
    public List<BaggageDetails> ?Baggage { get; set; } = new List<BaggageDetails>();
}
public class Ticket
{
    public string Code { get; set; } // Il numero del biglietto elettronico
    public string Status { get; set; } // Lo stato del biglietto (es. HK1)
}
public class AdditionalInfo
{
    public string ?Type { get; set; } // Tipo di informazione, es. CHLD, RQST
    public string ?Status { get; set; } // Stato, es. HK1
    public string ?SeatRequest { get; set; } // Dettagli aggiuntivi, es. posto 12A
}