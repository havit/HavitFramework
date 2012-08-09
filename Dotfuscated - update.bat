rem DLLs

xcopy /y Havit.Data.SqlServer\bin\Release\Havit.Data.SqlServer.dll Dotfuscated\




rem XML Comments

xcopy /y Havit\bin\Release\Havit.xml Dotfuscated\
xcopy /y Havit.Business\bin\Release\Havit.Business.xml Dotfuscated\
xcopy /y Havit.Data\bin\Release\Havit.Data.xml Dotfuscated\
xcopy /y Havit.Data.SqlServer\bin\Release\Havit.Data.SqlServer.xml Dotfuscated\
xcopy /y Havit.Drawing\bin\Release\Havit.Drawing.xml Dotfuscated\
xcopy /y Havit.Web\bin\Release\Havit.Web.xml Dotfuscated\
xcopy /y Havit.Web.UI.WebControls.DynarchCalendar\bin\Release\Havit.Web.UI.WebControls.DynarchCalendar.xml Dotfuscated\
xcopy /y Havit.Xml\bin\Release\Havit.Xml.xml Dotfuscated\



rem Delete map files
del Dotfuscated\Map*.xml



pause