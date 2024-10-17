# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Install Node.js (for frontend dependencies)
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash - && \
    apt-get install -y nodejs

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build the application
COPY . ./
RUN npm install   # Instalacja zależności frontendu
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Expose port (jeśli API nasłuchuje na danym porcie)
EXPOSE 80
ENTRYPOINT ["dotnet", "PowerliftingCompareResult.dll"]
