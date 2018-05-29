xcopy NuGet\Havit.PayMuzo.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.PayMuzo
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.PayMuzo.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
