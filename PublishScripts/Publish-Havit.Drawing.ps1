xcopy NuGet\Havit.Drawing.1.*.nupkg \\topol.havit.local\Library\NuGet\Packages\Havit.Drawing
\\topol\library\nuget\commandlineutility\nuget.exe push -source "HavitVSTS" -ApiKey VSTS NuGet\Havit.Drawing.1.*.nupkg

Write-Host "Press enter to continue..."
Read-Host
