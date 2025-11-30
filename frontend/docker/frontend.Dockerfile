FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .

ARG VITE_USERS_API
ARG VITE_PRODUCTS_API
ENV VITE_USERS_API=$VITE_USERS_API
ENV VITE_PRODUCTS_API=$VITE_PRODUCTS_API
RUN npm run build

FROM nginx:alpine
COPY docker/nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 8080
