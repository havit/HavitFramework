xcopy NuGet\Havit.Business.BLGenerator.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.Business.BLGenerator
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.Business.BLGenerator.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host

