// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

let expandedFlightsMap = new Map();
let selectedFlightId = "";

var map;
var dragBox;

document.addEventListener('click', event => {
    if (selectedFlightId !== "" && (event.target.className !== "flight-id" && event.target.src !== PLANE_IMAGE_UNSELECTED && event.target.src !== PLANE_IMAGE_SELECTED)) {
        hideSelectedFlight(selectedFlightId);
    }
});

function initMap() {
    map = new google.maps.Map(document.getElementById('map'),
        {
            center: {lat: 31, lng: 34},
            zoom: 8
        });
}

let intervalMs = 1000;

let initialDateTime = null;
let relativeDateTime = null;

let previousFlights = [];

setInterval(() => {
    if (initialDateTime == null) {
        initialDateTime = new Date();
        relativeDateTime = initialDateTime;
    } else {
        relativeDateTime = moment(relativeDateTime).add(intervalMs, "ms").toDate();
    }
    $.ajax({
        url: `http://localhost:5000/api/flights?relative_to=${relativeDateTime.toISOString()}`,
    }).done((data) => {
        data.sort((firstFlight, secondFlight) =>
            firstFlight.flight_id > secondFlight.flight_id ? 1 : -1
        );

        let flightListHtml = "";
        data.forEach(flight => {
            flightListHtml += updateFlightsListHtml(flight);
            updateFlightInMap(flight);
        });

        previousFlights = previousFlights.filter(previousFlight => {
            return !data.find(flight => previousFlight.flight_id === flight.flight_id);
        });

        previousFlights.forEach(flight => removeFlight(flight.flight_id));

        previousFlights = data;
        $("#flight-list").html(flightListHtml);
    });
}, intervalMs);

function hideSelectedFlight(flightId) {
    if (currentPolyline) {
        currentPolyline.setMap(null);
    }
    if (flightsMarkers.get(flightId)) {
        flightsMarkers.get(flightId).setIcon(PLANE_IMAGE_UNSELECTED);
    }
    if (selectedFlightId !== "") {
        flightsMarkers.get(selectedFlightId).setIcon(PLANE_IMAGE_UNSELECTED);
        $("#" + flightId + " .details").hide();
        $("#" + selectedFlightId + " .details").hide();
        $("#" + selectedFlightId).removeClass("selected");
        $("#" + flightId).removeClass("selected");
        $("#" + selectedFlightId).addClass("not-selected");
        $("#" + flightId).addClass("not-selected");
    }
    selectedFlightId = "";
    resetDetails();
}

function toggleFlight(element) {
    let flightId = element.innerText ? element.innerText : element;
    if (currentPolyline) {
        currentPolyline.setMap(null);
    }
    let flag = flightId !== selectedFlightId;
    if (selectedFlightId !== "") {
        hideSelectedFlight(selectedFlightId);
    }
    if (flag) {
        $("#" + flightId).addClass("selected");
        // Expand
        $("#" + flightId).removeClass("not-selected");
        if (!(selectedFlightId === "")) {
            let previousId = $("#" + selectedFlightId + " .details");

            previousId.hide();
            previousId.removeClass("selected");
            previousId.addClass("not-selected");
        }
        if (flightsMarkers.get(flightId)) {
            flightsMarkers.get(flightId).setIcon(PLANE_IMAGE_SELECTED);
        }
        selectedFlightId = flightId;
        $("#" + flightId + " .details").show();
        updateDetails();
    }
}


function updateFlightsListHtml(flight) {
    return `<li id=${flight.flight_id} class="list-group-item ${selectedFlightId === flight.flight_id ? 'selected' : 'not-selected'} " >
                    <img src="css/x.png" id="x-button" class="x-button x-hover"  onclick='removeFlight("${flight.flight_id}")' style="display: ${!flight.is_external ? 'block' : 'none'}"/> 
                    <div onclick='toggleFlight(this)' class='flight-id'>
                        ${flight.flight_id}
                    </div>
                    <div style="display: ${selectedFlightId === flight.flight_id ? 'block' : 'none'}"
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
    return new google.maps.LatLng(latitude, longitude);
}

const PLANE_IMAGE_UNSELECTED = "https://www.shareicon.net/data/48x48/2017/02/01/877360_paper_512x512.png";
const PLANE_IMAGE_SELECTED = "css/airplane2.svg";

function updateFlightInMap(flight) {
    if (!flightsMarkers.get(flight.flight_id)) {
        let marker = new google.maps.Marker({
            icon: new google.maps.MarkerImage(PLANE_IMAGE_UNSELECTED),
            map: map,
            title: flight.flight_id,
            anchor: new google.maps.Point(0, 32)
        });
        marker.setPosition(getPosition(flight.latitude, flight.longitude));
        marker.addListener('click', () => {
            if (currentPolyline) {
                currentPolyline.setMap(null);
                currentPolyline = null;
            }
            toggleFlight(flight.flight_id);

        });
        flightsMarkers.set(flight.flight_id, marker);
        
        
    } else {
        let marker = flightsMarkers.get(flight.flight_id);
        marker.setPosition(getPosition(flight.latitude, flight.longitude));
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
    <h6>Passengers:</h6> ${flightPlan.passengers}
    </td>
    <td>
    <h6>Takeoff Location:</h6>
    ${flightPlan.initial_location.longitude}, ${flightPlan.initial_location.latitude}
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
        // let flightPlan = JSON.parse(data);
        $("#flight-details").html(returnDetailsHtml(data));
        calculatePolyline(data);
    });
}

// let polylineMap = new Map();
let currentPolyline = null;

function calculatePolyline(flightPlan) {
    if (currentPolyline != null) {
        currentPolyline.setMap(null);
        currentPolyline = null;
    }

    let oldSegments = [flightPlan.initial_location];
    oldSegments = oldSegments.concat(flightPlan.segments);
    let segments = [];
    let initialTime = moment.utc(oldSegments[0].date_time);
    let i = 0;
    for (; i < oldSegments.length - 1; i++) {
        let x = moment(relativeDateTime).subtract(oldSegments[i + 1].timespan_seconds, "seconds");
        if (initialTime > x) {
            for (; i < oldSegments.length; i++) {
                segments.push(getPosition(oldSegments[i].latitude, oldSegments[i].longitude));
            }
        } else {
            initialTime.add(oldSegments[i].timespan_seconds, "seconds");
        }
    }


    let flightPath = new google.maps.Polyline({
        path: segments,
        geodesic: true,
        strokeColor: '#FF0000',
        strokeOpacity: 1.0,
        strokeWeight: 2,
        map: map
    });


    currentPolyline = flightPath;
}

let fileReader = new FileReader();

fileReader.onload = function (event) {
    console.log(event.target.result);
    $.ajax({
        url: `/api/FlightPlan/`,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: event.target.result
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

    hideDropStyle();
}

function allowDrop(ev) {
    $("#drop-text").show();
    $("#dragbox").addClass('droppable');
    ev.preventDefault();
}

function hideDropStyle(event) {
    $("#drop-text").hide();
    $("#dragbox").removeClass('droppable');
}

function removeFlight(flight_id) {
    let markerToDelete = flightsMarkers.get(flight_id);
    if (markerToDelete) {
        console.log("removeFlight has been called");
        $.ajax({
            url: `/api/Flights/${flight_id}`,
            type: 'DELETE'
        });

        if (currentPolyline) {
            currentPolyline.setMap(null);
            currentPolyline = null;
        }

        markerToDelete.setMap(null);
        flightsMarkers.delete(flight_id);
    }
}
