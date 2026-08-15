# Étape 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copie des fichiers projets pour cache layer
COPY SanteSenegal.Domain/SanteSenegal.Domain.csproj SanteSenegal.Domain/
COPY SanteSenegal.Application/SanteSenegal.Application.csproj SanteSenegal.Application/
COPY SanteSenegal.Infrastructure/SanteSenegal.Infrastructure.csproj SanteSenegal.Infrastructure/
COPY SanteSenegal.Api/SanteSenegal.Api.csproj SanteSenegal.Api/

# Restore (télécharge les packages NuGet)
RUN dotnet restore SanteSenegal.Api/SanteSenegal.Api.csproj

# Copie du code source
COPY SanteSenegal.Domain/ SanteSenegal.Domain/
COPY SanteSenegal.Application/ SanteSenegal.Application/
COPY SanteSenegal.Infrastructure/ SanteSenegal.Infrastructure/
COPY SanteSenegal.Api/ SanteSenegal.Api/

# Build & Publish (sans --no-restore pour éviter les conflits obj/ Windows)
RUN dotnet publish SanteSenegal.Api/SanteSenegal.Api.csproj -c Release -o /app/publish

# Étape 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Créer un répertoire pour la base SQLite persistante
RUN mkdir -p /app/data

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SanteSenegal.Api.dll"]
