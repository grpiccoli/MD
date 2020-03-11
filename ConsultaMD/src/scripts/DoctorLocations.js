function initPlaceSelector() {
    var map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: -42.1410401, lng: -84.2488564 },
        zoom: 3,
        streetViewControl: false,
        zoomControl: false,
        fullscreenControl: false,
        mapTypeControl: false
    });
    document.getElementById('add')
        .addEventListener('click', function () {
        $("#Input_More").val("true");
        $("form").submit();
    });
    var end = document.getElementById('end');
    if (end != null) {
        end.addEventListener('click', function () {
            $("#Input_More").val("false");
            $("form").submit();
        });
    }
    var input = document.getElementById('pac-input');
    var options = {
        componentRestrictions: { country: 'cl' }
    };
    var autocomplete = new google.maps.places.Autocomplete(input, options);
    autocomplete.bindTo('bounds', map);
    autocomplete.setFields(['place_id', 'geometry', 'name', 'formatted_address', 'photos']);
    var infowindow = new google.maps.InfoWindow();
    var infowindowContent = document.getElementById('infowindow-content');
    infowindow.setContent(infowindowContent);
    var marker = new google.maps.Marker({ map: map });
    marker.addListener('click', function (_) {
        infowindow.open(map, marker);
    });
    autocomplete.addListener('place_changed', function (_) {
        infowindow.close();
        var place = autocomplete.getPlace();
        if (!place.geometry) {
            return;
        }
        if (place.geometry.viewport) {
            map.fitBounds(place.geometry.viewport);
        }
        else {
            map.setCenter(place.geometry.location);
            map.setZoom(17);
        }
        marker.setPlace({
            placeId: place.place_id,
            location: place.geometry.location
        });
        marker.setVisible(true);
        infowindowContent.querySelector('#place-image').src = place.photos[0].getUrl({ 'maxWidth': 200 });
        infowindowContent.querySelector('#place-name').textContent = place.name;
        document.getElementById('Input_PlaceId').value = place.place_id;
        infowindowContent.querySelector('#place-address').textContent = place.formatted_address;
        infowindow.open(map, marker);
        document.getElementById('add').attributes.removeNamedItem('disabled');
    });
}
//# sourceMappingURL=DoctorLocations.js.map