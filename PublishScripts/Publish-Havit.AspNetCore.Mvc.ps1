xcopy NuGet\Havit.AspNetCore.Mvc.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.AspNetCore.Mvc
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.AspNetCore.Mvc.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
