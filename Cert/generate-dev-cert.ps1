$password = [guid]::newguid() #if you want a random password for each developer machine, use this GUID -> [guid]::newguid()

$certPath = "$PSScriptRoot\localhost-dev.pfx"
#$certPath = "$env:APPDATA\ASP.NET\Https\localhost-dev.pfx"
Write-Host "Password generated: " $password
Write-Host "CertPath: " $certPath

Write-Host "Generating dev-certs https."
dotnet dev-certs https -ep $certPath -p $password

Write-Host "Settings user-secrets for project $projectPath"
$projectPath = (Convert-Path "$PSScriptRoot/../HttpsAspNetCore/HttpsAspNetCore.csproj")

dotnet user-secrets --project $projectPath set "CertPassword" "$password"

#This setting for CertPath is required for running in VisualStudio, for Docker it is overriden with environment variable
dotnet user-secrets --project $projectPath set "CertPath" "$certPath"