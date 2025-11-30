FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY InnoShop.sln ./

COPY backend/src/Users/InnoShop.Users.Api/InnoShop.Users.Api.csproj           src/Users/InnoShop.Users.Api/
COPY backend/src/Users/InnoShop.Users.Application/InnoShop.Users.Application.csproj src/Users/InnoShop.Users.Application/
COPY backend/src/Users/InnoShop.Users.Infrastructure/InnoShop.Users.Infrastructure.csproj src/Users/InnoShop.Users.Infrastructure/
COPY backend/src/Users/InnoShop.Users.Domain/InnoShop.Users.Domain.csproj     src/Users/InnoShop.Users.Domain/
COPY backend/src/Users/InnoShop.Users.Contracts/InnoShop.Users.Contracts.csproj src/Users/InnoShop.Users.Contracts/
COPY backend/src/Products/InnoShop.Products.Domain/InnoShop.Products.Domain.csproj src/Products/InnoShop.Products.Domain/

RUN dotnet restore src/Users/InnoShop.Users.Api/InnoShop.Users.Api.csproj

COPY backend/ .

RUN dotnet publish src/Users/InnoShop.Users.Api/InnoShop.Users.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "InnoShop.Users.Api.dll"]
