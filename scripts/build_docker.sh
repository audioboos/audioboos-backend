#!/usr/bin/env bash

source ~/.prv/github_token && \
    echo $GITHUB_TOKEN | docker login ghcr.io -u fergalmoran --password-stdin && \
    docker buildx build \
        -t ghcr.io/audioboos/audioboos-backend \
        --file docker/Dockerfile \
        --platform linux/arm/v7 --push .
