xcopy NuGet\Havit.Enterprise.Web.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.Enterprise.Web
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.Enterprise.Web.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
