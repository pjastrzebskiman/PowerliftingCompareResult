# Etap 1: Budowanie aplikacji
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Kopiuj plik projektu i przywróć zależności
COPY *.csproj ./
RUN dotnet restore

# Kopiuj resztę plików i zbuduj aplikację
COPY . ./
RUN dotnet publish -c Release -o out

# Etap 2: Tworzenie obrazu runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out ./

# Ustawienie zmiennej środowiskowej ASPNETCORE_URLS
ENV ASPNETCORE_URLS=http://+:8080

# Eksponuj port
EXPOSE 8080

# Ustawienie punktu wejścia
ENTRYPOINT ["dotnet", "PowerliftingCompareResult.dll"]
