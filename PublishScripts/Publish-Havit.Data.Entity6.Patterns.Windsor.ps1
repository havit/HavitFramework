xcopy NuGet\Havit.Data.Entity6.Patterns.Windsor.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.Data.Entity6.Patterns.Windsor
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.Data.Entity6.Patterns.Windsor.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
