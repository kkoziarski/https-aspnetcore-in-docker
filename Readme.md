# How to run ASP.NET Core app on HTTPS in Docker

## This prove of concept demonstrate following topics:
1. Configuration of HTTPS in ASP.NET Core app: `Program.cs` and `Startup.cs`
2. Using SSL certificate file in ASP.NET Core app: `localhost-dev.pfx`
3. Generating localhost development certifcate file: `generate-dev-cert.ps1`
4. Running ASP.NET Core on HTTPS inside Docker container: `Dockerfile` and `docker-compose.override.yml`
5. Configuring HTTPS ports for Visula Studio and Docker container: `launchSettings.json` and `docker-compose.override.yml`

## Step 1:  Generate local development certificate

Execute `Cert\generate-dev-cert.ps1` file which will create a certificate: `Cert\localhost-dev.pfx` 
with and auto-generated random password (a GUID) and saves that password in **UserSecrets** (`secrets.json`) for this project

## Step 2: Run docker-compose

In the root catalog (where the `docker-compose.yml` file is) run command
```
docker-compose up -d --build
```
this will build docker image and create a docker container. 

## Step 3: Open browser

Open https://localhost:44309/api/values to confirm the app is running correctly.

## Explanation

This PoC uses PowerShell script `Cert\generate-dev-cert.ps1` which is using [dotnet dev-certs](https://blogs.msdn.microsoft.com/webdev/2018/02/27/asp-net-core-2-1-https-improvements/#https-in-development) 
tool to generate a certificate file `Cert\localhost-dev.pfx` 
with a random password (a GUID). This password and full path to the generated certificate file is then saved to [User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows)
for the project.


Good starting point for HTTPS development is: [Enforce HTTPS in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.1&tabs=visual-studio)

### Main points

HTTPS is configured in `Program.cs`
```
.UseKestrel(options =>
    {
        //...
        options.Listen(IPAddress.Any, httpsPort, listenOptions =>
        {
            listenOptions.UseHttps(certPath, certPassword);
        });
    });
```
and `Startup.cs`

```
if (env.IsDevelopment())
{
    //...
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
```

HTTPS port and allowed urls are configured with environment variables 
defined in `launchSettings.json` (for running in Visual Studio) and `docker-compose.override.yml` (for running in Docker container)

```
"environmentVariables": {
    "ASPNETCORE_HTTPS_PORT": "44300",
    "ASPNETCORE_URLS": "https://*:443"
}
------
environment:
    - ASPNETCORE_HTTPS_PORT=44309
    - ASPNETCORE_URLS=https://+:443;http://+:80
    - CertPath=/app/cert/localhost-dev.pfx
```

### Running in Visual Studio
When running in Visul Studio, HTTPS port and allowed urls are overriden by `launchSettings.json`.
`CertPath` and `CertPassword` are stored in _User Secrets_ for the project and used when running the application: `configuration.GetValue<string>("CertPassword")`



### Running in Docker
When running in Docker, HTTPS port and allowed urls are overriden by `docker-compose.override.yml`

`CertPassword` is still stored in _User Secrets_ for the project and is available inside Docker container 
by creating a `volume` linked to _User Secrets_ folder on the host machine in `docker-compose.override.yml`
```
    volumes:
      - ${APPDATA}/Microsoft/UserSecrets:/root/.microsoft/usersecrets:ro
```

`CertPath` in _User Secrets_ is overriden by environment variable defined in `docker-compose.override.yml`. This is the same path
as defined in `Dockerfile`: `/app/cert/localhost-dev.pfx`
```
ARG CERT_PATH_DEST=/app/cert/localhost-dev.pfx
ENV CertPath=$CERT_PATH_DEST
COPY Cert/localhost-dev.pfx $CertPath
```
