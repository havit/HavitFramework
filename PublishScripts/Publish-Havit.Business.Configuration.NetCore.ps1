xcopy NuGet\Havit.Business.Configuration.NetCore.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.Business.Configuration.NetCore
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.Business.Configuration.NetCore.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
