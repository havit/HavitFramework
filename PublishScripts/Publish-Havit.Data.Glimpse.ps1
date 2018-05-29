xcopy NuGet\Havit.Data.Glimpse.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.Data.Glimpse
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.Data.Glimpse.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
