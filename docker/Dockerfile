FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ../audioboos-data/. ./audioboos-data/
COPY ../audioboos-server/. ./audioboos-server/

WORKDIR /app/audioboos-server
RUN dotnet restore

WORKDIR /app/audioboos-server
RUN dotnet publish -c Release -o out


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app
COPY --from=build-env /app/audioboos-server/out .
EXPOSE 80

ENTRYPOINT ["dotnet", "AudioBoos.Server.dll"]