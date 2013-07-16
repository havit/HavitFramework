xcopy Framework\*.dll \\topol.havit.local\library\framework /y
xcopy Framework\*.pdb \\topol.havit.local\library\framework /y
xcopy Framework\*.xml \\topol.havit.local\library\framework /y

xcopy Documentation \\topol.havit.local\Inetpub\havit.local\hfw\Documentation /e /y

$null = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")