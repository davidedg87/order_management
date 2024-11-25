# Fase di build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ENV ASPNETCORE_URLS=http://+:80


# Copia i file csproj di tutti i progetti
COPY src/PhotoSiTest.Common/PhotoSiTest.Common.csproj PhotoSiTest.Common/

COPY src/PhotoSiTest.AddressService.Core/PhotoSiTest.AddressService.Core.csproj src/PhotoSiTest.AddressService.Core/
COPY src/PhotoSiTest.AddressService.Data/PhotoSiTest.AddressService.Data.csproj src/PhotoSiTest.AddressService.Data/

COPY src/PhotoSiTest.OrderService.Core/PhotoSiTest.OrderService.Core.csproj src/PhotoSiTest.OrderService.Core/
COPY src/PhotoSiTest.OrderService.Data/PhotoSiTest.OrderService.Data.csproj src/PhotoSiTest.OrderService.Data/

COPY src/PhotoSiTest.ProductService.Core/PhotoSiTest.ProductService.Core.csproj src/PhotoSiTest.ProductService.Core/
COPY src/PhotoSiTest.ProductService.Data/PhotoSiTest.ProductService.Data.csproj src/PhotoSiTest.ProductService.Data/

COPY src/PhotoSiTest.UserService.Core/PhotoSiTest.UserService.Core.csproj src/PhotoSiTest.UserService.Core/
COPY src/PhotoSiTest.UserService.Data/PhotoSiTest.UserService.Data.csproj src/PhotoSiTest.UserService.Data/

COPY src/PhotoSiTest.API/PhotoSiTest.API.csproj PhotoSiTest.API/

# Ripristina le dipendenze
RUN dotnet restore PhotoSiTest.API/PhotoSiTest.API.csproj

# Copia tutto il codice sorgente
COPY src/PhotoSiTest.Common/ src/PhotoSiTest.Common/
COPY src/PhotoSiTest.AddressService.Core/ src/PhotoSiTest.AddressService.Core/
COPY src/PhotoSiTest.AddressService.Data/ src/PhotoSiTest.AddressService.Data/
COPY src/PhotoSiTest.OrderService.Core/ src/PhotoSiTest.OrderService.Core/
COPY src/PhotoSiTest.OrderService.Data/ src/PhotoSiTest.OrderService.Data/
COPY src/PhotoSiTest.ProductService.Core/ src/PhotoSiTest.ProductService.Core/
COPY src/PhotoSiTest.ProductService.Data/ src/PhotoSiTest.ProductService.Data/
COPY src/PhotoSiTest.UserService.Core/ src/PhotoSiTest.UserService.Core/
COPY src/PhotoSiTest.UserService.Data/ src/PhotoSiTest.UserService.Data/
COPY src/PhotoSiTest.API/ src/PhotoSiTest.API/

# Compila e pubblica l'applicazione in modalità Release
WORKDIR src/PhotoSiTest.API
RUN dotnet publish -c Release -o /app/publish

# Fase di runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PhotoSiTest.API.dll"]
