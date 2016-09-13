xcopy Havit.Documentation.Homepage \\preview.havit.local\Inetpub\hfw.havit.local /e /y
xcopy Documentation \\preview.havit.local\Inetpub\hfw.havit.local\Documentation /e /y

Write-Host "Press enter to continue..."
Read-Host
