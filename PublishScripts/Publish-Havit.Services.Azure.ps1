xcopy NuGet\Havit.Services.Azure.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.Services.Azure
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.Services.Azure.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
