xcopy NuGet\Havit.ApplicationInsights.DependencyCollector.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.ApplicationInsights.DependencyCollector
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.ApplicationInsights.DependencyCollector.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
