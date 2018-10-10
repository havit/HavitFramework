$scriptPath = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
$packageConfigPath = "$scriptPath\packages.config"

Write-Host "Looking for installed Havit.Business.BLGenerator version (in $packageConfigPath)"
[xml]$packagesConfig = Get-Content -Path $packageConfigPath
$version = ($packagesConfig.packages.package | Where-Object { $_.id -eq 'Havit.Business.BLGenerator' } | Select-Object Version).Version
if (-Not $version)
{
	Write-Host 'Havit.Business.BLGenerator not found.'
}
else
{
	Write-Host "Found version $version."
	$blgExecutable = Join-Path $scriptPath "..\packages\Havit.Business.BLGenerator.$version\tools\BLG\Havit.Business.BusinessLayerGenerator.exe" -Resolve
	if ($blgExecutable)
	{
		Write-Host "Running BLG ($blgExecutable)"
		Push-Location $scriptPath
		& $blgExecutable -webconfig=..\Web\web.config -outputpath='.' "-targetplatform:SqlServer2008" "-strategy:HavitCodeFirst"
		Pop-Location
	}
}
