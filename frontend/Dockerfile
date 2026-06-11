FROM node:20-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci --prefer-offline --no-audit --no-fund

COPY public ./public
COPY src ./src
COPY tsconfig.json tailwind.config.js postcss.config.js ./

ARG REACT_APP_API_URL=http://localhost:5055/api
ENV REACT_APP_API_URL=$REACT_APP_API_URL
ENV CI=true
RUN npm run build

FROM nginx:alpine AS runtime
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/build /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
