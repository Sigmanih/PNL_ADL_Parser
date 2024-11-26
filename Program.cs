using Microsoft.AspNetCore.Mvc;
using PNL_ADL_Parser.Models;
using PNL_ADL_Parser.Parsers;
using PNL_ADL_Parser.Validators;
using FluentValidation;
using FluentValidation.Results;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency injection
builder.Services.AddSingleton<PNLParser>();
builder.Services.AddSingleton<IValidator<FlightDetails>, FlightValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// API Endpoints

// Endpoint per caricare e analizzare un file PNL/ADL
app.MapPost("/api/parse", ([FromServices] PNLParser parser, [FromBody] string[] fileLines) =>
{
    try
    {
        var flightDetails = parser.Parse(fileLines);
        return Results.Ok(flightDetails);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error parsing file: {ex.Message}");
    }
})
.WithName("ParseFile")
.WithOpenApi();

// Endpoint per validare un oggetto FlightDetails
app.MapPost("/api/validate", ([FromServices] IValidator<FlightDetails> validator, [FromBody] FlightDetails flightDetails) =>
{
    ValidationResult results = validator.Validate(flightDetails);

    if (!results.IsValid)
    {
        var errors = results.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
        return Results.BadRequest(errors);
    }

    return Results.Ok("Validation successful");
})
.WithName("ValidateFlightDetails")
.WithOpenApi();

// Endpoint per ottenere un esempio di formato JSON
app.MapGet("/api/example", () =>
{
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

    return Results.Ok(JsonSerializer.Serialize(example, new JsonSerializerOptions { WriteIndented = true }));
})
.WithName("GetExample")
.WithOpenApi();

app.Run();
