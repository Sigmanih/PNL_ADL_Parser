using Microsoft.AspNetCore.Mvc;
using PNL_ADL_Parser.Models;
using PNL_ADL_Parser.Parsers;
using PNL_ADL_Parser.Validators;
using FluentValidation;
using FluentValidation.Results;
using System.Text.Json;

namespace PNL_ADL_Parser.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : ControllerBase
    {
        private readonly PNLParser _parser;
        private readonly IValidator<FlightDetails> _validator;

        // Inietta il servizio PNLParser e il validatore
        public FlightController(PNLParser parser, IValidator<FlightDetails> validator)
        {
            _parser = parser;
            _validator = validator;
        }

        /// <summary>
        /// Endpoint per caricare e analizzare un file PNL/ADL.
        /// </summary>
        [HttpPost("parse")]
        public IActionResult ParsePNL([FromBody] string[] fileLines)
        {
            try
            {
                // Esegui il parsing del file
                var flightDetails = _parser.Parse(fileLines);
                // Salva il file di risposta in un file locale di Test
                SaveResponseToFile(JsonSerializer.Serialize(flightDetails, new JsonSerializerOptions
                {
                    WriteIndented = true // Per formattare il JSON con indentazione leggibile
                }));  
                return Ok(flightDetails); // Restituisce i dettagli del volo in caso di successo
            }
            catch (Exception ex)
            {
                // Restituisce un errore se il parsing fallisce
                return Problem($"Errore nel parsing del file: {ex.Message}");
            }
        }

        /// <summary>
        /// Endpoint per caricare e analizzare un file PNL/ADL.
        /// </summary>
        [HttpPost("parse-file")]
        public async Task<IActionResult> ParsePNLFile(IFormFile file)
        {
            try
            {
                // Verifica se il file è stato caricato
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Nessun file caricato.");
                }

                // Legge il contenuto del file
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    // Leggi tutte le righe del file e le converte in un array di stringhe
                    var fileLines = new List<string>();
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        fileLines.Add(line);
                    }

                    // Esegui il parsing del contenuto del file
                    var flightDetails = _parser.Parse(fileLines.ToArray());

                    SaveResponseToFile(JsonSerializer.Serialize(flightDetails, new JsonSerializerOptions
                    {
                        WriteIndented = true // Per formattare il JSON con indentazione leggibile
                    }));  
                    // Restituisci i dettagli del volo in caso di successo
                    return Ok(flightDetails);
                }
            }
            catch (Exception ex)
            {
                // Restituisce un errore se il parsing fallisce
                return Problem($"Errore nel parsing del file: {ex.Message}");
            }
        }


        /// <summary>
        /// Endpoint per generare il file PNL dal FlightDetails.
        /// </summary>
        [HttpPost("generate-pnl")]
        public IActionResult GeneratePNLFile([FromBody] FlightDetails flightDetails)
        {
            try
            {
                // Usa il parser per generare il contenuto del file PNL
                var pnlContent = _parser.GeneratePNLFile(flightDetails);

                // Salva il file di risposta in un file locale di Test
                SaveResponseToFile(pnlContent);
                // Restituisci solo il contenuto delle righe, senza "pnlContent" come chiave
                return Content(pnlContent, "text/plain");
            }
            catch (Exception ex)
            {
                // Restituisci un errore se la generazione fallisce
                return Problem($"Errore durante la generazione del file PNL: {ex.Message}");
            }
        }

        /// <summary>
        /// Salva la risposta API in un file nella cartella Tests con nome incrementale.
        /// </summary>
        private void SaveResponseToFile(string content)
        {
            try
            {
                // Definisci il percorso della directory Tests
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Tests");
                
                // Crea la directory se non esiste
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Trova l'ultimo numero di file nella directory
                var existingFiles = Directory.GetFiles(directoryPath, "*.txt");
                var maxIndex = existingFiles
                    .Select(file => Path.GetFileNameWithoutExtension(file)) // Prende solo il nome del file senza estensione
                    .Where(name => int.TryParse(name, out _))              // Considera solo file con nomi numerici
                    .Select(int.Parse)                                    // Converte in interi
                    .DefaultIfEmpty(0)                                    // Imposta 0 se non ci sono file
                    .Max();                                               // Trova il massimo

                // Nome del nuovo file incrementale
                var newFileName = Path.Combine(directoryPath, $"{maxIndex + 1}.txt");

                // Scrivi il contenuto nel nuovo file
                System.IO.File.WriteAllText(newFileName, content);
            }
            catch (Exception ex)
            {
                // Logga eventuali errori nel salvataggio del file
                Console.WriteLine($"Errore durante il salvataggio del file di risposta: {ex.Message}");
            }
        }
    }
}
