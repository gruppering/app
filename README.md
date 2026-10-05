[![.NET](https://github.com/gruppering/app/actions/workflows/dotnet.yml/badge.svg)](https://github.com/gruppering/app/actions/workflows/dotnet.yml)

# Gruppering
En gruppematching-plattform for både studenter og lærere, 
hvor man kan finne, opprette grupper eller opprette courses etter ønsker.


## Run 
1. Customize .env file.
2. `docker compose -f compose.yaml up -d`

### Credentials
Assuming that the environment variable `TEST_DB_SEEDING` in the docker compose file is present and set to true,
the credentials for logging in as a course owner is username: `adminTest`, password: `adminTest`
and to login as the users are; username `testUser*` (where * is a number between 0 and 150), and password: `test`.

If the environment variable is not set, you need to manually create an account with the signup endpoint, which you will be redirected to automatically.


## Testing
`dotnet test`

## Development
1. Create .env file from example.env

2. `docker compose -f compose.dev.yaml up -d`
3. dotnet run
