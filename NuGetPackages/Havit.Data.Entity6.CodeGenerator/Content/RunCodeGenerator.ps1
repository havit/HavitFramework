$scriptPath = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
$packageConfigPath = "$scriptPath\packages.config"

Write-Host "Looking for installed Havit.Data.Entity6.CodeGenerator version (in $packageConfigPath)"
[xml]$packagesConfig = Get-Content -Path $packageConfigPath
$version = ($packagesConfig.packages.package | Where-Object { $_.id -eq 'Havit.Data.Entity6.CodeGenerator' } | Select-Object Version).Version
if (-Not $version)
{
	Write-Host 'Havit.Data.Entity6.CodeGenerator not found.'
}
else
{
	Write-Host "Found version $version."
	$codeGenerator = Join-Path $scriptPath "..\packages\Havit.Data.Entity6.CodeGenerator.$version\tools\CodeGenerator\Havit.Data.Entity6.CodeGenerator.exe" -Resolve
	if ($codeGenerator)
	{
		Write-Host "Running code generator ($codeGenerator)"
		Push-Location $scriptPath
		& $codeGenerator
		Pop-Location
	}
}
