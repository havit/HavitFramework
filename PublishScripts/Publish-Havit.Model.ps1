xcopy NuGet\Havit.Model.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.Model
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.Model.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
