FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY InnoShop.sln ./

COPY backend/src/Products/InnoShop.Products.Api/InnoShop.Products.Api.csproj                 src/Products/InnoShop.Products.Api/
COPY backend/src/Products/InnoShop.Products.Application/InnoShop.Products.Application.csproj src/Products/InnoShop.Products.Application/
COPY backend/src/Products/InnoShop.Products.Infrastructure/InnoShop.Products.Infrastructure.csproj src/Products/InnoShop.Products.Infrastructure/
COPY backend/src/Products/InnoShop.Products.Domain/InnoShop.Products.Domain.csproj           src/Products/InnoShop.Products.Domain/
COPY backend/src/Products/InnoShop.Products.Contracts/InnoShop.Products.Contracts.csproj     src/Products/InnoShop.Products.Contracts/

COPY backend/src/Users/InnoShop.Users.Domain/InnoShop.Users.Domain.csproj                    src/Users/InnoShop.Users.Domain/
COPY backend/src/Users/InnoShop.Users.Contracts/InnoShop.Users.Contracts.csproj              src/Users/InnoShop.Users.Contracts/

RUN dotnet restore src/Products/InnoShop.Products.Api/InnoShop.Products.Api.csproj

COPY backend/ .

RUN dotnet publish src/Products/InnoShop.Products.Api/InnoShop.Products.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "InnoShop.Products.Api.dll"]
