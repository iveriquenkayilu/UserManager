
const apiOptions = {
    apiKey: 'AIzaSyB4wi-PHmb9vblmIPPrwQn_Qp6jAdDnesQ',
    version: "beta",
    maps_ids: "44cd9da493f07d40"
};
const mapOptions = {   
    tilt: 0,
    heading: 0,
    zoom: 18,
    center: { lat: -4.3042392, lng: 15.310026 },
    mapId: "44cd9da493f07d40",
    // disable interactions due to animation loop and moveCamera
    disableDefaultUI: false,
    gestureHandling: "none",
    keyboardShortcuts: false,
};

$(document).ready(async function () {
   await initialize3DMap(mapOptions.center.lat, mapOptions.center.lng);      
});
async function initialize3DMap() { 
    const map3D = await init3DMap();
    const geocoder = new google.maps.Geocoder();
    const infowindow = new google.maps.InfoWindow();

    map3D.addListener('zoom_changed', function () {
        mapzoom = map3D.getZoom(); //to stored the current zoom level of the map
    });
    map3D.addListener("click", (mapsMouseEvent) => {
        map3D.panTo(mapsMouseEvent.latLng);
        const latlng = {
            lat: parseFloat(mapsMouseEvent.latLng.lat()),
            lng: parseFloat(mapsMouseEvent.latLng.lng()),
        };
        $('#latitude').val(latlng.lat);
        $('#longitude').val(latlng.lng);
        //$('#location').val('lat : ' + mapsMouseEvent.latLng.lat() + ' lng: ' + mapsMouseEvent.latLng.lng() + '');       
        geocodeLatLng(geocoder, map3D, infowindow, latlng);
    });
    await initWebglOverlayView(map3D);
};

function geocodeLatLng(geocoder, map, infowindow, latLng){
  
    geocoder
        .geocode({ location: latLng })
        .then((response) => {
            if (response.results[0]) {
                map.setZoom(20);

                const marker = new google.maps.Marker({
                    position: latLng,
                    map: map,
                });
                
                var result = response.results[0].formatted_address;
                infowindow.setContent(result);
                $('#location_name').val(result);
                infowindow.open(map, marker);
            } else {
                window.alert("No results found");
            }
        })
        .catch((e) => window.alert("Geocoder failed due to: " + e));
}

async function init3DMap()  { 
    const apiLoader = new THREE.Loader(apiOptions);
    await apiLoader.load();
    return new google.maps.Map(document.getElementById("map"), mapOptions);
}

async function initWebglOverlayView(map) {
    // THREE: scene, camera, renderer
    let scene, renderer, camera;

    const webGLOverlayView = new google.maps.OverlayView();//  webGLOverlayView();
    webGLOverlayView.setMap(map);

    webGLOverlayView.onAdd = () => {
        // some code
         scene = new THREE.Scene();
        camera = new THREE.PerspectiveCamera();
        const ambiLight = new THREE.AmbientLight(0xffffff, 0.75);
        scene.add(ambiLight);
        const directionLight = new THREE.DirectionalLight(0xffffff, 0.75);
        directionLight.position.set(0.5, -1, 0.5);
        scene.add(directionLight);
        // Load the model.
        const loader = new THREE.GLTFLoader();
        const url ="https://raw.githubusercontent.com/googlemaps/js-samples/master/assets/pin.gltf";

        loader.load(url, (gltf) => {
            gltf.scene.scale.set(25, 25, 25);  // 10*10*10
            gltf.scene.rotation.x = Math.PI / 2;
            scene.add(gltf.scene);

            let { tilt, heading, zoom } = mapOptions;

            const animate = () => {
                if (tilt < 67.5) {
                    tilt += 0.5;
                } else if (heading <= 360) {
                    heading +=0.7;
                    zoom -= 0.0005;
                } else {
                    // exit animation loop
                    return;
                }

                map.moveCamera({ tilt, heading, zoom });
                requestAnimationFrame(animate);
             };

              requestAnimationFrame(animate);
        });
    };

    webGLOverlayView.onContextRestored = (gl) => {
        renderer = new THREE.webGLRenderer({
            canvas: gl.canvas,
            context: gl,
            ...gl.getContextAllributes()
        })
        renderer.autoClear = false;

        loader.manager.onLoad = () => {
            renderer.setAnimationLoop(() => {
                map.moveCamera({
                    tilt: mapOptions.tilt,
                    heading: mapOptions.heading,
                    zoom: mapOptions.zoom
                })
                 if (mapOptions.titl < 67.5) mapOptions.titl += 0.5
                 else if (mapOptions.heading <= 360) mapOptions.heading += 0.2;
                 else renderer.setAnimationLoop(null);
            });
            //No differrence so far
            //if (mapOptions.titl < 67.5) mapOptions.titl += 0.5
            //else if (mapOptions.heading <= 360) mapOptions.heading += 0.2;
            //else renderer.setAnimationLoop(null);
        }
    };

    webGLOverlayView.onDraw = (gl, coordinateTransformer) => {
        // on change
        const matrix = coordinateTransformer.fromLatLngAltitude(mapOptions.center, 120); // 120 meters
        camera.projectionMatrix = new THREE.Matrix4().fromArray(matrix);

        webGLOverlayView.requestRedraw();
        renderer.render(scene, camera);
        renderer.resetState();
    };
    webGLOverlayView.setMap(map);
}

function initMap() {  // called by google maps as callback
   
};

function getMyLocation(){
     navigator.geolocation.getCurrentPosition(success, error);
}
function success(pos) {
    var crd = pos.coords;
    latitud = crd.latitude
    longitud = crd.longitude
    var map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: latitud, lng: longitud },
        zoom: 14
    });
};
function error(err) {
    console.log(err);
}