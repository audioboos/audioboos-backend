#!/usr/bin/env bash

docker build -t fergalmoran/audioboos-server ../
docker push fergalmoran/audioboos-server

docker-compose up -d
docker-compose logs -f
