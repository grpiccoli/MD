
const CACHE_NAME = `PWA-TEST`;
var urlsToCache = [
    `./`,
    `./splash.html`,
    `./login.html`,
    `./index.html`,
    `./signup.html`,
    `./doctor-detail.html`,
    `./css/splash.css`,
    `./css/index.css`,
    `./css/doctor-detail.css`,
    `./js/splash.js`,
    `./img/splash.png`,
    `./img/splash2.png`,
    `./img/splash3.png`
];


self.addEventListener('install', function (event) {
    // Perform install steps
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(function (cache) {
                console.log('Opened cache');
                return cache.addAll(urlsToCache);
            })
    );
});


self.addEventListener('activate', function (event) {

    var cacheWhitelist = ['index.html', 'login.html'];

    event.waitUntil(
        caches.keys().then(function (cacheNames) {
            return Promise.all(
                cacheNames.map(function (cacheName) {
                    if (cacheWhitelist.indexOf(cacheName) === -1) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});


self.addEventListener('fetch', function (event) {
    event.respondWith(
        caches.match(event.request)
            .then(function (response) {
                // Cache hit - return response
                if (response) {
                    return response;
                }
                return fetch(event.request);
            }
            )
    );
});

