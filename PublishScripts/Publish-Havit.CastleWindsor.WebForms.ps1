xcopy NuGet\Havit.CastleWindsor.WebForms.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.CastleWindsor.WebForms
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.CastleWindsor.WebForms.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
