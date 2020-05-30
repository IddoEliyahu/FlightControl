// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

let toggleMap = new Map();

setInterval(() => {
    $.ajax({
        url: "http://localhost:5000/api/flights",
    }).done((data) => {
        console.log(data);
        let flightListHtml = "";
        data.forEach(flight => {
            let flightId = flight.flight_id;
            flightListHtml +=
                `<li class="list-group-item"> 
                    <div onclick='toggleFlight(this)'>
                        ${flightId}
                    </div>
                    <div id=${flightId} style="display: ${toggleMap.get(flightId) ? 'block' : 'none'}" class="${flight.is_external ? 'external-flight' : 'internal-flight '} details">
                        <div class="flight-detail"> Passengers: ${flight.passengers} </div>
                        <div> Company Name: ${flight.company_name} </div>
                        <div> Location: (${flight.longitude}, ${flight.latitude} )</div>
                        <div> Time until landing: 30m </div>
                    </div>
                 </li>`
        });
        $("#flight-list").html(flightListHtml);
    });
}, 1000);

function toggleFlight(element) {
    let flightId = element.innerText;
    if (toggleMap.get(flightId)) {
        $("#" + flightId).hide();
        toggleMap.set(flightId.toString(), false);
    } else {
        $("#" + flightId).show();
        toggleMap.set(flightId.toString(), true);
    }
}