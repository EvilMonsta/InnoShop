# Инструкция

## Роль админа выдавать через БД(эндпоинта нет)

## Создать в папке deploy файл .env, назначить в нем:

### DB
PG_USER=<имя>
PG_PASSWORD=<пароль>
USERS_DB=<название бд для юзеров>
PRODUCTS_DB=<название бд для продуктов>
PG_HOST_PORT=5432

### JWT
JWT_ISSUER=innoshop
JWT_AUDIENCE=innoshop.clients
JWT_SIGNING_KEY=<придумать более-менее длинный ключ, символов 40 норм будет>

### Internal API key
INTERNAL_API_KEY=<придумать ключ для внутреннего взаимодействия>

### SMTP 
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=<почта рассыльного>
SMTP_PASSWORD=<через гугл акк получить 16-значный пароль, вводить без пробелов>
SMTP_FROM=e<почта рассыльного>
SMTP_ENABLE_SSL=true

### Frontend build-time(хосты можно и другие, я юзал эти)
VITE_USERS_API=http://localhost:5001 
VITE_PRODUCTS_API=http://localhost:5002

### CORS(хосты можно и другие, я юзал эти)
CORS_ALLOWED_ORIGINS=http://localhost:8080;http://localhost:5173;https://localhost:5173

### Frontend public URL 
FRONTEND_BASE_URL=http://localhost:8080/#

### ASP.NET
ASPNETCORE_ENVIRONMENT=Development


## В папке фронта создать .env и написать в нем
VITE_USERS_API=http://localhost:5001
VITE_PRODUCTS_API=http://localhost:5002
