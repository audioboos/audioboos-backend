FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64  AS build-env
#FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-amd64 AS build

WORKDIR /app
ARG VERSION=0.0.1

COPY ./audioboos-data/. ./audioboos-data/
COPY ./audioboos-server/. ./audioboos-server/

WORKDIR /app/audioboos-server
RUN dotnet restore

WORKDIR /app/audioboos-server
RUN dotnet publish -c release -o out /p:UseAppHost=false -p:Version=$VERSION --no-restore
/p:AssemblyVersion=${{ steps.gitversion.outputs.assemblySemFileVer }} /p:FileVersion=${{ steps.gitversion.outputs.assemblySemFileVer }} /p:InformationalVersion=${{ steps.gitversion.outputs.Sha }}
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

RUN apk update
RUN apk add tzdata
RUN cp /usr/share/zoneinfo/Europe/Dublin /etc/localtime
RUN rm -r /usr/share/zoneinfo/Africa && \
    rm -r /usr/share/zoneinfo/Antarctica && \
    rm -r /usr/share/zoneinfo/Arctic && \
    rm -r /usr/share/zoneinfo/Asia && \
    rm -r /usr/share/zoneinfo/Atlantic && \
    rm -r /usr/share/zoneinfo/Australia && \
    rm -r /usr/share/zoneinfo/Europe  && \
    rm -r /usr/share/zoneinfo/Indian && \
    rm -r /usr/share/zoneinfo/Mexico && \
    rm -r /usr/share/zoneinfo/Pacific && \
    rm -r /usr/share/zoneinfo/Chile && \
    rm -r /usr/share/zoneinfo/Canada
RUN echo "Europe/Dublin" >  /etc/timezone

ENV TZ Europe/Dublin
ENV LANG en_IE.UTF-8
ENV LANGUAGE en_IE.UTF-8
ENV LC_ALL en_IE.UTF-8

RUN apk --update add imagemagick
RUN apk add --no-cache file

WORKDIR /app
COPY --from=build-env /app/audioboos-server/out .
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "AudioBoos.Server.dll"]