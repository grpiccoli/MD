function initPlaceSelector() {
    var map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: -42.1410401, lng: -84.2488564 },
        zoom: 3,
        streetViewControl: false,
        zoomControl: false,
        fullscreenControl: false,
        mapTypeControl: false
    });

    var input = <HTMLInputElement>document.getElementById('pac-input');

    var options = {
        //types: ['address|health|establishment|doctor|hospital'],
        componentRestrictions: {country: 'cl'}
    };

    var autocomplete = new google.maps.places.Autocomplete(input, options);
    autocomplete.bindTo('bounds', map);

    // Specify just the place data fields that you need.
    autocomplete.setFields(['place_id', 'geometry', 'name', 'formatted_address', 'photos']);

    //map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

    var infowindow = new google.maps.InfoWindow();
    var infowindowContent = document.getElementById('infowindow-content');
    infowindow.setContent(infowindowContent);

    var marker = new google.maps.Marker({ map: map });

    marker.addListener('click', _=> {
        infowindow.open(map, marker);
    });

    autocomplete.addListener('place_changed', _=> {
        infowindow.close();

        var place = autocomplete.getPlace();

        if (!place.geometry) {
            return;
        }

        if (place.geometry.viewport) {
            map.fitBounds(place.geometry.viewport);
        } else {
            map.setCenter(place.geometry.location);
            map.setZoom(17);
        }

        // Set the position of the marker using the place ID and location.
        marker.setPlace({
            placeId: place.place_id,
            location: place.geometry.location
        });

        marker.setVisible(true);

        (infowindowContent.querySelector('#place-image') as HTMLImageElement).src = place.photos[0].getUrl({ 'maxWidth': 200 });
        infowindowContent.querySelector('#place-name').textContent = place.name;
        //infowindowContent.querySelector('#place-id').textContent = place.place_id;
        $('#Input_PlaceId').val(place.place_id);
        infowindowContent.querySelector('#place-address').textContent = place.formatted_address;
        infowindow.open(map, marker);
        $("#add").removeAttr('disabled');
    });
}