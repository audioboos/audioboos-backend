#!/usr/bin/env bash

git checkout trunk
git push
git push --tags
git checkout develop
git push
git push {fergalmoran} :release/1.4.0
