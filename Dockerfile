# Build stage for .NET backend
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-backend
WORKDIR /app

# Install Node.js (for npm install and npm run build in the .csproj)
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash - && \
    apt-get install -y nodejs

# Copy backend .csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy all backend files and build the application
COPY . ./
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy backend files from the build stage
COPY --from=build-backend /out .

# Expose port
EXPOSE 80

ENTRYPOINT ["dotnet", "PowerliftingCompareResult.dll"]
