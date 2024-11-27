# PNL_ADL_Parser

### Descrizione
**PNL_ADL_Parser** è un'applicazione scritta in C# con .NET progettata per analizzare e gestire documenti PNL (Passenger Name List) e ADL (Addition and Deletion List). L'applicativo consente di:
- Leggere e interpretare i documenti PNL/ADL.
- Estrarre informazioni strutturate (dettagli volo, passeggeri, bagagli, richieste speciali).
- Validare i dati estratti.
- Generare un output in formato JSON.
- (Opzionale) Convertire i dati JSON nuovamente in formato PNL/ADL.
- (Opzionale) Confrontare documenti generati e originali.

--------------------------------------------------------------------------------------

### Funzionalità
1. **Parsing dei documenti PNL/ADL**:
   - Analisi della struttura del volo (numero volo, data, rotta).
   - Estrazione dei dettagli passeggeri e bagagli.
2. **Validazione dei dati**:
   - Regole di validazione per formati, coerenza dei dati e campi richiesti.
3. **Generazione JSON**:
   - Conversione dei dati estratti in un formato JSON ben strutturato.
4. **Opzioni aggiuntive (facoltative)**:
   - Conversione JSON → PNL/ADL.
   - Interfaccia utente (WPF) per la gestione dei voli.
   - Web API per generazione PNL tramite input JSON.

--------------------------------------------------------------------------------------

### Requisiti di Sistema
- **.NET SDK**: 7.0 o superiore (preferibilmente 8.0).
- **Librerie aggiuntive**:
  - `Newtonsoft.Json`
  - `FluentValidation`

--------------------------------------------------------------------------------------

### Installazione
1. Clona il repository:
   ```bash
   git clone https://github.com/Sigmanih/PNL_ADL_Parser
   cd PNL_ADL_Parser


### Ripristina i pacchetti necessari:
dotnet restore


### Compila il progetto:
dotnet build


### Esegui l'applicazione:
dotnet run

--------------------------------------------------------------------------------------
### Struttura del Progetto

PNL_ADL_Parser
├── Services/          # Classi per il parsing dei documenti PNL/ADL.
├── Models/            # Modelli dati per i dettagli volo e passeggeri.
├── Controllers/       # Controllers per servire le API 
├── Utils/             # Utilità comuni (es. helper per file I/O).
├── Tests/             # Test unitari per verificare la correttezza del codice.
├── Program.cs         # Entry point dell'applicazione.
├── README.md          # Documentazione del progetto.
└── .gitignore         # File per ignorare file e directory non necessari.

--------------------------------------------------------------------------------------
### Licenza

Questo progetto è distribuito sotto la licenza MIT. Consulta il file LICENSE per ulteriori dettagli.

--------------------------------------------------------------------------------------

Contatti

    Autore: DS
    GitHub: Sigmanih

