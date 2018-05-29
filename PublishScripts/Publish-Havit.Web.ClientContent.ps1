xcopy NuGet\Havit.Web.ClientContent.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\.Web.ClientContent
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\.Web.ClientContent.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
