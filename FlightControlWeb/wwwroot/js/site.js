// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

let expandedFlightsMap = new Map();
let selectedFlightId = "";

var map;

function initMap() {
    map = new google.maps.Map(document.getElementById('map'),
        {
            center: {lat: 31, lng: 34},
            zoom: 8
        });
}

setInterval(() => {
    $.ajax({
        url: "http://localhost:5000/api/flights",
    }).done((data) => {
        data.sort((firstFlight, secondFlight) =>
            firstFlight.flight_id > secondFlight.flight_id ? 1 : -1
        );

        let flightListHtml = "";
        data.forEach(flight => {
            flightListHtml += updateFlightsListHtml(flight);
            addFlightToMap(flight);
        });
        
        $("#flight-list").html(flightListHtml);
    });
}, 1000);

function toggleFlight(element) {
    let flightId = element.innerText ? element.innerText : element;

    let flightDetailsSelector = $("#" + flightId + " .details");

    // TODO: fix conditions to toggle only on the last one selected if it's expanded
    if (expandedFlightsMap.get(flightId)) {
        flightDetailsSelector.hide();
        expandedFlightsMap.set(flightId.toString(), false);
        $("#" + selectedFlightId).toggleClass("selected");
        selectedFlightId = null;
        $("#" + flightId).toggleClass("selected");

    } else {
        expandedFlightsMap.forEach(flightFlag => {
            $("#" + selectedFlightId).toggleClass("not-selected");
        });
        $("#" + flightId).toggleClass("selected");
        selectedFlightId = flightId;
        flightDetailsSelector.show();
        expandedFlightsMap.set(flightId.toString(), true);
    }
}

function updateFlightsListHtml(flight) {
    return `<li id=${flight.flight_id} class="list-group-item ${selectedFlightId === flight.flight_id ? 'selected' : ''}" >
                    <div onclick='toggleFlight(this)' class='flight-id'>
                        ${flight.flight_id}
                    </div>
                    <div style="display: ${expandedFlightsMap.get(flight.flight_id) ? 'block' : 'none'}"
                         class="${flight.is_external ? 'external-flight' : 'internal-flight '} details
                                ${selectedFlightId === flight.flight_id ? 'selected' : ''}">
                        <div class="flight-detail"> Passengers: ${flight.passengers} </div>
                        <div> Company Name: ${flight.company_name} </div>
                        <div> Location: (${flight.longitude}, ${flight.latitude} )</div>
                        <div> Time until landing: 30m </div>
                    </div>
             </li>`;
}

flightsMarkers = new Map();

function getPosition(latitude, longitude) {
    return {lat: Number(latitude), lng: Number(longitude)};
}

function addFlightToMap(flight) {
    if (!flightsMarkers.get(flight.flight_id)) {
        let marker = new google.maps.Marker({
            position: getPosition(flight.latitude, flight.longitude),
            icon: "https://www.shareicon.net/data/48x48/2017/02/01/877360_paper_512x512.png",
            map: map
        });

        marker.addListener('click', () => toggleFlight(flight.flight_id));
        flightsMarkers.set(flight.flight_id, marker);
    } else {
        flightsMarkers.get(flight.flight_id).setPosition({
                position: getPosition(flight.latitude, flight.longitude)
            }
        )
    }
}

function initDetails() {
    
}

function resetDetails() {
    
}

function updateDetails() {

}

