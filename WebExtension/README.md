npm install -g browserify
npm install -g typescript
npm install -g tsify
npm install -g http-browserify
npm install http-browserify

browserify extension.ts -p [ tsify ] > browserified.js

web-ext run