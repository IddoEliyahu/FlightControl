// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

let expandedFlightsMap = new Map();
let selectedFlightId = "";

var map;
var dragBox;

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
    debugger
}, 1000);

function toggleFlight(element) {
    let flightId = element.innerText ? element.innerText : element;

    let flightDetailsSelector = $("#" + flightId + " .details");

    // TODO: fix conditions to toggle only on the last one selected if it's expanded
    if (expandedFlightsMap.get(flightId)) {
        // Shrink
        flightDetailsSelector.hide();
        expandedFlightsMap.set(flightId.toString(), false);
        $("#" + selectedFlightId).removeClass("selected");
        $("#" + flightId).removeClass("selected");
        $("#" + selectedFlightId).addClass("not-selected");
        $("#" + flightId).addClass("not-selected");
        selectedFlightId = "";
    } else {
        $("#" + flightId).addClass("selected");
        // Expand
        $("#" + flightId).removeClass("not-selected");
        if (!(selectedFlightId === "")) {
            let previousId = $("#" + selectedFlightId + " .details");

            previousId.hide();
            previousId.removeClass("selected");
            previousId.addClass("not-selected");
            resetDetails();
        }
        selectedFlightId = flightId;
        flightDetailsSelector.show();
        expandedFlightsMap.set(flightId.toString(), true);
        updateDetails();
    }
}

function updateFlightsListHtml(flight) {
    return `<li id=${flight.flight_id} class="list-group-item ${selectedFlightId === flight.flight_id ? 'selected' : 'not-selected'} " >
                    <img src="css/x.png" id="x-button" class="x-button x-hover"  onclick='removeFlight("${flight.flight_id}")' style="display: ${!flight.is_external ? 'block' : 'none'}"/> 
                    <div onclick='toggleFlight(this)' class='flight-id'>
                        ${flight.flight_id}
                    </div>
                    <div style="display: ${expandedFlightsMap.get(flight.flight_id) ? 'block' : 'none'}"
                         class="${flight.is_external ? 'external-flight' : 'internal-flight '} details
                                ${selectedFlightId === flight.flight_id ? 'selected' : ''}">
                        <div class="flight-detail"> Passengers: ${flight.passengers} </div>
                        <div> Company Name: ${flight.company_name} </div>
                        <div> Location: (${flight.longitude.toFixed(8)}, ${flight.latitude.toFixed(8)} )</div>
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
            icon: new google.maps.MarkerImage("https://www.shareicon.net/data/48x48/2017/02/01/877360_paper_512x512.png"),
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

// Change it to red on click.
function changeMarkerColor(flight) {
    flightsMarkers.get(flight.flight_id).icon.fontcolor();
}

function resetDetails() {
    let emptyGrid = `<tr>
                        <td>
                        Passengers: 
                        </td>
                        <td>
                        Takeoff Location: 
                        </td>
                        <td>
                        Landing Location: 
                        </td>
                        <td>
                        Airline Company: 
                        </td>
                        <td>
                        Takeoff Time: 
                        </td>
                        <td>
                        Landing Time:
                        </td>
                        </tr>`;
    $("#flight-details").html(emptyGrid);
}

function returnDetailsHtml(flightPlan) {
    let lastSegment = flightPlan.segments[Math.max(0, flightPlan.segments.length - 1)];
    let dateToMoment = moment(flightPlan.initial_location.date_time);
    flightPlan.segments.forEach(segment => {
        dateToMoment.add(segment.timespan_seconds, 'seconds');
    });
    let momentToDate = dateToMoment.toDate().toString();
    let s = `<tr>
    <td>
    Passengers: ${flightPlan.passengers}
    </td>
    <td>
    Takeoff Location: ${flightPlan.initial_location.longitude}, ${flightPlan.initial_location.latitude}
    </td>
    <td>
    Landing Location: ${lastSegment.longitude}, ${lastSegment.latitude}
    </td>
    <td>
    Airline Company: ${flightPlan.company_name}
    </td>
    <td>
    Takeoff Time: ${flightPlan.initial_location.date_time}
    </td>
    <td>
    Landing Time: ${momentToDate}
    </td>
    </tr>`;
    return s;
}

// TODO: Fix the server and see if this actually works.
function updateDetails() {
    $.ajax({
        url: `http://localhost:5000/api/FlightPlan/${selectedFlightId}`
    }).done((data) => {
        let flightPlan = JSON.parse(data);
        $("#flight-details").html(returnDetailsHtml(flightPlan));
        calculatePolyline(flightPlan);
    });
}

// let polylineMap = new Map();
let currentPolyline = null;

function calculatePolyline(flightPlan) {
    if (currentPolyline != null) {
        currentPolyline.setMap(null);
        currentPolyline = null;
    }
    let segments = [{lat: flightPlan.initial_location.latitude, lon: flightPlan.initial_location.longitude}];
    segments = segments.concat(flightPlan.segments.map(segment => ({lat: segment.latitude, lng: segment.longitude})));
    let flightPath = new google.maps.Polyline({
        path: segments,
        geodesic: true,
        strokeColor: '#FF0000',
        strokeOpacity: 1.0,
        strokeWeight: 2
        , map: map
    });
    // polylineMap.set(flightPlan.flight_id, flightPath);

    // flightPath.setMap(map);


    currentPolyline = flightPath;
}
let fileReader = new FileReader();

fileReader.onload = function (event) {
    debugger
    console.log(event.target.result);
    $.ajax({
        url: `/api/FlightPlan`,
        type: 'POST',
        body: event.target.result
    });
    
    return event.target.result;
};

function dropHandler(ev) {
    ev.preventDefault();

    let fileData = ev.target.files || ev.dataTransfer.files;
    //Data should contain .json files.
    Array.prototype.forEach.call(fileData, flightPlan => {
        let fileText = fileReader.readAsText(flightPlan);
        console.log(fileText);
    });
}

function allowDrop(ev) {
    ev.preventDefault();
}

function removeFlight(flight_id) {

    console.log("removeFlight has been called");
    $.ajax({
        url: `/api/Flights/${flight_id}`,
        type: 'DELETE'
    });
}
