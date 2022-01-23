#!/usr/bin/env bash

docker build \
    -t fergalmoran/audioboos-backend \
    --build-arg VERSION=6.6.6-staging \
    -f docker/Dockerfile \
    . --load

docker run \
    -p 8082:80 \
    -e System__AudioLookupService="musicbrainz" \
    -e SystemSettings__CachePath="/opt/audioboos-cache" \
    -e ConnectionStrings__PostgresConnection="Host=frankfurt-postgres.render.com;Port=5432;Database=audioboos_3mtn;Username=audioboos;Password=dOfEHd7urR7YFb1KglWaG7Livsn0U4GS" \
    -e System__WebClientUrl="https://audioboos.info" \
    -e JWTOptions__Audience="https://audioboos.info" \
    -e JWTOptions__Issuer="https://api.audioboos.info" \
    -e JWTOptions__Secret="8hJaMlttHR/tVg9sXVxtPqhzPd/eC7ZRnbb6QdtX0mw=" \
    -e JWTOptions__TokenLifetime="00:15:00" \
    -e JWTOptions__RefreshTokenLifetime="182.00:00:00" \
    fergalmoran/audioboos-backend
