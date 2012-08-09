rem ****************************************************************************************************
rem Dotfuscate
rem ****************************************************************************************************

"%ProgramFiles%\Microsoft Visual Studio 8\Application\PreEmptive Solutions\Dotfuscator Community Edition\dotfuscator.exe" Dotfuscator.xml


rem ****************************************************************************************************
rem DLLs, které nejsou dotèeny obfuskátorem
rem ****************************************************************************************************

xcopy /y Havit.Data.SqlServer\bin\Release\Havit.Data.SqlServer.dll Dotfuscated\


rem ****************************************************************************************************
rem XML Comments
rem ****************************************************************************************************

xcopy /y Havit\bin\Release\Havit.xml Dotfuscated\
xcopy /y Havit.Business\bin\Release\Havit.Business.xml Dotfuscated\
xcopy /y Havit.Data\bin\Release\Havit.Data.xml Dotfuscated\
xcopy /y Havit.Data.SqlServer\bin\Release\Havit.Data.SqlServer.xml Dotfuscated\
xcopy /y Havit.Drawing\bin\Release\Havit.Drawing.xml Dotfuscated\
xcopy /y Havit.Web\bin\Release\Havit.Web.xml Dotfuscated\
xcopy /y Havit.Enterprise.Web\bin\Release\Havit.Enterprise.Web.xml Dotfuscated\
xcopy /y Havit.Xml\bin\Release\Havit.Xml.xml Dotfuscated\
xcopy /y Havit.PayMuzo\bin\Release\Havit.PayMuzo.xml Dotfuscated\


rem ****************************************************************************************************
rem Delete map files
rem ****************************************************************************************************

del Dotfuscated\Map*.xml


rem ****************************************************************************************************
rem Copy to Library
rem ****************************************************************************************************

xcopy /y Dotfuscated\*.* \\dev\Library\Framework\
rem System.Web.Extensions uz nekopirujeme do library frameworku, je k dispozici v library ajaxu
rem xcopy /y References\System.Web.Extensions.dll \\dev\Library\Framework\
rem xcopy /y "doc\HAVIT .NET Framework.chm" \\dev\Library\Documentation\

xcopy /y Scripts\*.js \\dev\Library\Scripts\
xcopy /y Styles\*.css \\dev\Library\Styles\

pause