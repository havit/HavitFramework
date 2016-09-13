xcopy NuGet\Havit.Web.Bootstrap.*.nupkg \\topol.havit.local\Library\NuGet\Packages.DropFolder /y

xcopy Havit.Web.Bootstrap.Tutorial \\topol.havit.local\Inetpub\havit.local\hfw\Havit.Web.Bootstrap.Tutorial /e /y

Write-Host "Press enter to continue..."
Read-Host
