    $(document).ready(function() {
        // Funzione per aggiungere i campi passeggero
        function addPassengerFields(passengerIndex) {
            return `
                <fieldset class="passenger-fieldset">
                    <legend class="passenger-legend">Passeggero ${passengerIndex + 1}</legend>
                    <div class="form-row">
                        <label for="firstName_${passengerIndex}">Nome:</label>
                        <input type="text" id="firstName_${passengerIndex}" name="firstName_${passengerIndex}" required>
                    </div>
                    <div class="form-row">
                        <label for="lastName_${passengerIndex}">Cognome:</label>
                        <input type="text" id="lastName_${passengerIndex}" name="lastName_${passengerIndex}" required>
                    </div>
                    <div class="form-row">
                        <label for="tourOperator_${passengerIndex}">Tour Operator:</label>
                        <input type="text" id="tourOperator_${passengerIndex}" name="tourOperator_${passengerIndex}" required>
                    </div>
                    <div class="form-row">
                        <label for="baggageWeight_${passengerIndex}">Peso Bagaglio:</label>
                        <input type="number" id="baggageWeight_${passengerIndex}" name="baggageWeight_${passengerIndex}" required>
                    </div>
                </fieldset>
            `;

        }

        // Aggiungi i campi per i passeggeri
        $('#passengerCount').on('input', function() {
            const count = $(this).val();
            let passengerFields = '';
            for (let i = 0; i < count; i++) {
                passengerFields += addPassengerFields(i);
            }
            $('#passengerSection').html(passengerFields);
        });

        // Invia la richiesta via AJAX
        $('#flightForm').on('submit', function(e) {
            e.preventDefault();

            const flightData = {
                flightNumber: $('#flightNumber').val(),
                generalInfo: $('#flightName').val(),
                flightDate: $('#flightDate').val(),
                route: $('#route').val(),
                passengerCount: $('#passengerCount').val(),
                passengers: []
            };

            // Aggiungi i dettagli dei passeggeri
            const passengerCount = $('#passengerCount').val();
            for (let i = 0; i < passengerCount; i++) {
                const passenger = {
                    firstName: $(`#firstName_${i}`).val(),
                    lastName: $(`#lastName_${i}`).val(),
                    tourOperator: $(`#tourOperator_${i}`).val(),
                    baggage: [{
                        type: "BAGS",
                        status: "HK1",
                        quantity: "01",
                        weight: $(`#baggageWeight_${i}`).val()
                    }]
                };
                flightData.passengers.push(passenger);
            }

            // Effettua la richiesta AJAX per inviare i dati
            $.ajax({
                url: 'http://localhost:5003/api/Flight/generate-pnl',  // Cambia con l'URL corretto
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(flightData),
                success: function(response) {
                    // Sostituisci i ritorni a capo con <br> per ogni nuova riga
                    var formattedResponse = response.replace(/\n/g, '<br>');

                    // Imposta il contenuto HTML dell'elemento #jsonResponse
                    $('#jsonResponse').html(formattedResponse);  // Usa .html() per interpretare i <br>
                    //$('#jsonResponse').text(JSON.stringify(response, null, 4));
                },
                error: function(xhr, status, error) {
                    $('#jsonResponse').text('Errore nella richiesta: ' + error);
                }
            });
        });
    });
