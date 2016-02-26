$scriptPath = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

[xml]$packagesConfig = Get-Content -Path 'DataLayer\packages.config'
$version = ($packagesConfig.packages.package | Where-Object { $_.id -eq 'Havit.Data.Entity6.CodeGenerator' } | Select-Object Version).Version

$codeGenerator = "$scriptPath\..\packages\Havit.Data.Entity6.CodeGenerator.$version\tools\Havit.Data.Entity6.CodeGenerator.exe"
&$codeGenerator