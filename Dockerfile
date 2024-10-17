# Build stage for .NET backend
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-backend
WORKDIR /app

# Copy backend .csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy all backend files and build the application
COPY . ./
RUN dotnet publish -c Release -o /out

# Build stage for React frontend in ClientApp folder
FROM node:16 AS build-frontend
WORKDIR /app

# Copy package.json and package-lock.json from ClientApp
COPY ClientApp/package*.json ./ClientApp/
WORKDIR /app/ClientApp
RUN npm install

# Copy the rest of the frontend files and build it
COPY ClientApp/ ./
RUN npm run build

# Runtime stage (final image)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy backend files from the build stage
COPY --from=build-backend /out .

# Copy frontend build files to the appropriate folder in the backend (for serving static files)
COPY --from=build-frontend /app/ClientApp/build ./wwwroot

# Expose port (adjust if necessary)
EXPOSE 80

ENTRYPOINT ["dotnet", "PowerliftingCompareResult.dll"]
