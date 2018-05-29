xcopy NuGet\Havit.Web.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.Web
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.Web.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
