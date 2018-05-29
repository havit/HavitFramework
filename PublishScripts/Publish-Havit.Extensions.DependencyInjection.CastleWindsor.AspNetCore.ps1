xcopy NuGet\Havit.Extensions.DependencyInjection.CastleWindsor.AspNetCore.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.Extensions.DependencyInjection.CastleWindsor.AspNetCore
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.Extensions.DependencyInjection.CastleWindsor.AspNetCore.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
