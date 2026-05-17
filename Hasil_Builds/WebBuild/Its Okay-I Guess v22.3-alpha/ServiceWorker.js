const cacheName = "leanst-Proto-IOIG-22.3";
const contentToCache = [
    "Build/Its Okay-I Guess v22.3-alpha.loader.js",
    "Build/Its Okay-I Guess v22.3-alpha.framework.js.br",
    "Build/Its Okay-I Guess v22.3-alpha.data.br",
    "Build/Its Okay-I Guess v22.3-alpha.wasm.br",
    "TemplateData/style.css"

];

self.addEventListener('install', function (e) {
    console.log('[Service Worker] Install');
    
    e.waitUntil((async function () {
      const cache = await caches.open(cacheName);
      console.log('[Service Worker] Caching all: app shell and content');
      await cache.addAll(contentToCache);
    })());
});

self.addEventListener('fetch', function (e) {
    e.respondWith((async function () {
      let response = await caches.match(e.request);
      console.log(`[Service Worker] Fetching resource: ${e.request.url}`);
      if (response) { return response; }

      response = await fetch(e.request);
      const cache = await caches.open(cacheName);
      console.log(`[Service Worker] Caching new resource: ${e.request.url}`);
      cache.put(e.request, response.clone());
      return response;
    })());
});
