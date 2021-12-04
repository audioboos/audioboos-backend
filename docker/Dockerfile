#FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./audioboos-data/. ./audioboos-data/
COPY ./audioboos-server/. ./audioboos-server/

WORKDIR /app/audioboos-server
RUN dotnet restore

WORKDIR /app/audioboos-server
RUN dotnet publish -c Release -o out /p:UseAppHost=false --no-restore


# Build runtime image
#FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/audioboos-server/out .
EXPOSE 80

RUN apt-get update && apt-get -y install imagemagick

ENTRYPOINT ["dotnet", "AudioBoos.Server.dll"]