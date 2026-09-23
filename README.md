[![.NET](https://github.com/gruppering/app/actions/workflows/dotnet.yml/badge.svg)](https://github.com/gruppering/app/actions/workflows/dotnet.yml)

# Gruppering
En gruppematching-plattform for både studenter og lærere, 
hvor man kan finne, opprette grupper eller opprette courses etter ønsker.

## Build

Hvis dotnet-ef ikke er installert
`dotnet tool install dotnet-ef --version "10.*"`

`dotnet build`


## Run 
`docker compose -f compose.dev.yaml up -d`

`dotnet ef database update`

`dotnet run`

## Testing
`dotnet test`
