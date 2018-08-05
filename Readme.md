# Hot to run ASP.NET Core app on HTTPS in Docker

## Generate local development certificate

Execute `Cert\generate-dev-cert.ps1` file which will create a certificate: `Cert\localhost-dev.pfx` 
with and auto-generated random password (a GUID) and saves that password in **UserSecrets** (`secrets.json`) for this project

## Run docker-compose

In the root catalog (where the `docker-compose.yml` file is) run command
```
docker-compose up -d --build
```
this will build docker image and create a docker container. 

## Open browser

Open https://localhost:44309/api/values to confirm the app is running correctly.