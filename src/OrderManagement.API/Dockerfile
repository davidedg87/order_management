# Fase di build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ENV ASPNETCORE_URLS=http://+:80


# Copia i file csproj di tutti i progetti
COPY src/OrderManagement.Common/OrderManagement.Common.csproj OrderManagement.Common/

COPY src/OrderManagement.AddressService.Core/OrderManagement.AddressService.Core.csproj src/OrderManagement.AddressService.Core/
COPY src/OrderManagement.AddressService.Data/OrderManagement.AddressService.Data.csproj src/OrderManagement.AddressService.Data/

COPY src/OrderManagement.OrderService.Core/OrderManagement.OrderService.Core.csproj src/OrderManagement.OrderService.Core/
COPY src/OrderManagement.OrderService.Data/OrderManagement.OrderService.Data.csproj src/OrderManagement.OrderService.Data/

COPY src/OrderManagement.ProductService.Core/OrderManagement.ProductService.Core.csproj src/OrderManagement.ProductService.Core/
COPY src/OrderManagement.ProductService.Data/OrderManagement.ProductService.Data.csproj src/OrderManagement.ProductService.Data/

COPY src/OrderManagement.UserService.Core/OrderManagement.UserService.Core.csproj src/OrderManagement.UserService.Core/
COPY src/OrderManagement.UserService.Data/OrderManagement.UserService.Data.csproj src/OrderManagement.UserService.Data/

COPY src/OrderManagement.API/OrderManagement.API.csproj OrderManagement.API/

# Ripristina le dipendenze
RUN dotnet restore OrderManagement.API/OrderManagement.API.csproj

# Copia tutto il codice sorgente
COPY src/OrderManagement.Common/ src/OrderManagement.Common/
COPY src/OrderManagement.AddressService.Core/ src/OrderManagement.AddressService.Core/
COPY src/OrderManagement.AddressService.Data/ src/OrderManagement.AddressService.Data/
COPY src/OrderManagement.OrderService.Core/ src/OrderManagement.OrderService.Core/
COPY src/OrderManagement.OrderService.Data/ src/OrderManagement.OrderService.Data/
COPY src/OrderManagement.ProductService.Core/ src/OrderManagement.ProductService.Core/
COPY src/OrderManagement.ProductService.Data/ src/OrderManagement.ProductService.Data/
COPY src/OrderManagement.UserService.Core/ src/OrderManagement.UserService.Core/
COPY src/OrderManagement.UserService.Data/ src/OrderManagement.UserService.Data/
COPY src/OrderManagement.API/ src/OrderManagement.API/

# Compila e pubblica l'applicazione in modalità Release
WORKDIR src/OrderManagement.API
RUN dotnet publish -c Release -o /app/publish

# Fase di runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrderManagement.API.dll"]
