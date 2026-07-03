# Build the SvelteKit static site (adapter-static) in a plain Node image.
# vite.config.js skips the dev-cert/proxy setup for `vite build`, so no
# .NET toolchain is needed here.
FROM node:22-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build

# Serve the static output and proxy /api to the gateway.
FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/build /usr/share/nginx/html
EXPOSE 80
