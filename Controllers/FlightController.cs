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

                // Restituisci solo il contenuto delle righe, senza "pnlContent" come chiave
                return Content(pnlContent, "text/plain");
            }
            catch (Exception ex)
            {
                // Restituisci un errore se la generazione fallisce
                return Problem($"Errore durante la generazione del file PNL: {ex.Message}");
            }
        }

    }
}
