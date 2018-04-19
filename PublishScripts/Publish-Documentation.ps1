xcopy Havit.Documentation.Homepage \\preview.havit.local\Inetpub\hfw.havit.local /e /y
Expand-Archive -Path Documentation\Documentation.zip -DestinationPath \\preview.havit.local\Inetpub\hfw.havit.local\Documentation -Force

Write-Host "Press enter to continue..."
Read-Host
