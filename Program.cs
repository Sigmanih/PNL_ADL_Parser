using Microsoft.AspNetCore.Mvc;
using PNL_ADL_Parser.Models;
using PNL_ADL_Parser.Parsers;
using PNL_ADL_Parser.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi al contenitore
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Iniezione delle dipendenze
builder.Services.AddSingleton<PNLParser>();                                     // Aggiungi il parser per l'analisi del file PNL
builder.Services.AddSingleton<IValidator<FlightDetails>, FlightValidator>();    // Aggiungi il validatore per FlightDetails

// Aggiungi il supporto per i controller
builder.Services.AddControllers();

var app = builder.Build();

// Configura la pipeline delle richieste HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                               // Abilita Swagger in modalità sviluppo
    app.UseSwaggerUI();                             // Interfaccia utente di Swagger per esplorare le API
}

app.UseHttpsRedirection();                          // Abilita il reindirizzamento su HTTPS

// Configura i controller per gestire le richieste
app.MapControllers();  // Questo è il punto in cui il routing dei controller viene attivato

// Avvia l'applicazione
app.Run();
