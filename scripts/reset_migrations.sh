#!/usr/bin/env bash
PROJECT_ROOT=$HOME/dev/audioboos/audioboos-server
SQLITEDB="$HOME/dev/audioboos/audioboos-server/audioboos.sqlite"
export PGPASSWORD='hackme'
export ASPNETCORE_ENVIRONMENT=Development

echo $SQLITEDB

if [ -f "$SQLITEDB" ]; then 
    echo deleting $SQLITEDB
    rm "$SQLITEDB"
fi

dropdb -f --if-exists -h localhost -U postgres audioboos
createdb -h localhost -U postgres audioboos

rm -rf $PROJECT_ROOT/audioboos-data/Migrations/*
cd $PROJECT_ROOT/audioboos-server
dotnet ef migrations add Initial --project ../audioboos-data/audioboos-data.csproj
dotnet ef database update --project ../audioboos-data/audioboos-data.csproj

cd $PROJECT_ROOT
