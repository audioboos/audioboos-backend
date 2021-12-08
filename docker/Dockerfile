FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64 AS build-env
WORKDIR /app

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
ENV ASPNETCORE_ENVIRONMENT=Production
RUN apt-get update && apt-get -y install imagemagick
#RUN apk --update add imagemagick
ENTRYPOINT ["dotnet", "AudioBoos.Server.dll"]