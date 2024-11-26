using Microsoft.AspNetCore.Mvc;
using PNL_ADL_Parser.Models;
using PNL_ADL_Parser.Parsers;
using PNL_ADL_Parser.Validators;
using FluentValidation;
using FluentValidation.Results;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi al contenitore
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Iniezione delle dipendenze
builder.Services.AddSingleton<PNLParser>();                                     // Aggiungi il parser per l'analisi del file PNL
builder.Services.AddSingleton<IValidator<FlightDetails>, FlightValidator>();    // Aggiungi il validatore per FlightDetails

var app = builder.Build();

// Configura la pipeline delle richieste HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                               // Abilita Swagger in modalità sviluppo
    app.UseSwaggerUI();                             // Interfaccia utente di Swagger per esplorare le API
}

app.UseHttpsRedirection();                          // Abilita il reindirizzamento su HTTPS

// API Endpoints

/// <summary>
/// Endpoint per caricare e analizzare un file PNL/ADL.
/// </summary>
/// <remarks>Questo endpoint riceve un array di stringhe che rappresentano il contenuto di un file PNL e restituisce i dettagli del volo e i passeggeri.</remarks>
app.MapPost("/api/parse", ([FromServices] PNLParser parser, [FromBody] string[] fileLines) =>
{
    try
    {
        // Esegui il parsing del file
        var flightDetails = parser.Parse(fileLines);
        return Results.Ok(flightDetails); // Restituisce i dettagli del volo in caso di successo
    }
    catch (Exception ex)
    {
        // Restituisce un errore se il parsing fallisce
        return Results.Problem($"Errore nel parsing del file: {ex.Message}");
    }
})
.WithName("ParseFile") // Assegna un nome all'endpoint
.WithOpenApi(); // Abilita OpenAPI per l'endpoint

/// <summary>
/// Endpoint per validare un oggetto FlightDetails.
/// </summary>
/// <remarks>Questo endpoint riceve un oggetto FlightDetails e restituisce il risultato della validazione.</remarks>
app.MapPost("/api/validate", ([FromServices] IValidator<FlightDetails> validator, [FromBody] FlightDetails flightDetails) =>
{
    // Esegui la validazione
    ValidationResult results = validator.Validate(flightDetails);

    if (!results.IsValid)
    {
        // Restituisce errori di validazione se non validato correttamente
        var errors = results.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
        return Results.BadRequest(errors);
    }

    // Restituisce un messaggio di successo se la validazione è riuscita
    return Results.Ok("Validazione riuscita");
})
.WithName("ValidateFlightDetails") // Assegna un nome all'endpoint
.WithOpenApi(); // Abilita OpenAPI per l'endpoint

/// <summary>
/// Endpoint per ottenere un esempio di formato JSON per FlightDetails.
/// </summary>
/// <remarks>Questo endpoint restituisce un esempio di un oggetto FlightDetails in formato JSON ben formattato.</remarks>
app.MapGet("/api/example", () =>
{
    // Crea un esempio di FlightDetails
    var example = new FlightDetails
    {
        FlightNumber = "NO6149",
        Route = "RHO-MXP",
        FlightDate = DateTime.Parse("2023-09-07"),
        PassengerCount = 1,
        Passengers = new List<PassengerDetails>
        {
            new PassengerDetails
            {
                LastName = "Albanesi",
                FirstName = "Marcello",
                PassengerType = "MR",
                SpecialRequests = new List<string> { "Seat 12A", "Vegetarian Meal" },
                Baggage = new List<BaggageDetails>
                {
                    new BaggageDetails { Type = "BAGS", Status = "HK1", Weight = 15.0 }
                }
            }
        }
    };

    // Restituisce l'esempio in formato JSON ben formattato
    return Results.Ok(JsonSerializer.Serialize(example, new JsonSerializerOptions { WriteIndented = true }));
})
.WithName("GetExample") // Assegna un nome all'endpoint
.WithOpenApi(); // Abilita OpenAPI per l'endpoint

// Avvia l'applicazione
app.Run();
