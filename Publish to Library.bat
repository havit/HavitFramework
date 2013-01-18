@echo off

xcopy /y Havit\bin\Release\Havit.dll \\topol\Library\Framework\
xcopy /y Havit\bin\Release\Havit.xml \\topol\Library\Framework\
xcopy /y Havit\bin\Release\Havit.pdb \\topol\Library\Framework\
rem xcopy /y Havit\bin\Release\CodeContracts\Havit.Contracts.dll \\topol\Library\Framework\CodeContracts
rem xcopy /y Havit\bin\Release\CodeContracts\Havit.Contracts.pdb \\topol\Library\Framework\CodeContracts

xcopy /y Havit.Business\bin\Release\Havit.Business.dll \\topol\Library\Framework\
xcopy /y Havit.Business\bin\Release\Havit.Business.xml \\topol\Library\Framework\
xcopy /y Havit.Business\bin\Release\Havit.Business.pdb \\topol\Library\Framework\
rem xcopy /y Havit.Business\bin\Release\CodeContracts\Havit.Business.Contracts.dll \\topol\Library\Framework\CodeContracts
rem xcopy /y Havit.Business\bin\Release\CodeContracts\Havit.Business.Contracts.pdb \\topol\Library\Framework\CodeContracts

xcopy /y Havit.Data\bin\Release\Havit.Data.dll \\topol\Library\Framework\
xcopy /y Havit.Data\bin\Release\Havit.Data.xml \\topol\Library\Framework\
xcopy /y Havit.Data\bin\Release\Havit.Data.pdb \\topol\Library\Framework\
rem xcopy /y Havit.Data\bin\Release\CodeContracts\Havit.Data.Contracts.dll \\topol\Library\Framework\CodeContracts
rem xcopy /y Havit.Data\bin\Release\CodeContracts\Havit.Data.Contracts.pdb \\topol\Library\Framework\CodeContracts

xcopy /y Havit.Data.SqlServer\bin\Release\Havit.Data.SqlServer.dll \\topol\Library\Framework\
xcopy /y Havit.Data.SqlServer\bin\Release\Havit.Data.SqlServer.xml \\topol\Library\Framework\
rem xcopy /y Havit.Data.SqlServer\bin\Release\Havit.Data.SqlServer.pdb \\topol\Library\Framework\
rem xcopy /y Havit.Data.SqlServer\bin\Release\CodeContracts\Havit.Data.SqlServer.Contracts.dll \\topol\Library\Framework\CodeContracts
rem xcopy /y Havit.Data.SqlServer\bin\Release\CodeContracts\Havit.Data.SqlServer.Contracts.pdb \\topol\Library\Framework\CodeContracts

xcopy /y Havit.Drawing\bin\Release\Havit.Drawing.dll \\topol\Library\Framework\
xcopy /y Havit.Drawing\bin\Release\Havit.Drawing.xml \\topol\Library\Framework\
xcopy /y Havit.Drawing\bin\Release\Havit.Drawing.pdb \\topol\Library\Framework\
rem xcopy /y Havit.Drawing\bin\Release\CodeContracts\Havit.Drawing.Contracts.dll \\topol\Library\Framework\CodeContracts
rem xcopy /y Havit.Drawing\bin\Release\CodeContracts\Havit.Drawing.Contracts.pdb \\topol\Library\Framework\CodeContracts

xcopy /y Havit.Web\bin\Release\Havit.Web.dll \\topol\Library\Framework\
xcopy /y Havit.Web\bin\Release\Havit.Web.xml \\topol\Library\Framework\
xcopy /y Havit.Web\bin\Release\Havit.Web.pdb \\topol\Library\Framework\
rem xcopy /y Havit.Web\bin\Release\CodeContracts\Havit.Web.Contracts.dll \\topol\Library\Framework\CodeContracts
rem xcopy /y Havit.Web\bin\Release\CodeContracts\Havit.Web.Contracts.pdb \\topol\Library\Framework\CodeContracts

xcopy /y Havit.Enterprise.Web\bin\Release\Havit.Enterprise.Web.dll \\topol\Library\Framework\
xcopy /y Havit.Enterprise.Web\bin\Release\Havit.Enterprise.Web.xml \\topol\Library\Framework\
xcopy /y Havit.Enterprise.Web\bin\Release\Havit.Enterprise.Web.pdb \\topol\Library\Framework\
rem xcopy /y Havit.Enterprise.Web\bin\Release\CodeContracts\Havit.Enterprise.Web.Contracts.dll \\topol\Library\Framework\CodeContracts
rem xcopy /y Havit.Enterprise.Web\bin\Release\CodeContracts\Havit.Enterprise.Web.Contracts.pdb \\topol\Library\Framework\CodeContracts

xcopy /y Havit.Xml\bin\Release\Havit.Xml.dll \\topol\Library\Framework\
xcopy /y Havit.Xml\bin\Release\Havit.Xml.xml \\topol\Library\Framework\
xcopy /y Havit.Xml\bin\Release\Havit.Xml.pdb \\topol\Library\Framework\
rem xcopy /y Havit.Xml\bin\Release\CodeContracts\Havit.Xml.Contracts.dll \\topol\Library\Framework\CodeContracts
rem xcopy /y Havit.Xml\bin\Release\CodeContracts\Havit.Xml.Contracts.pdb \\topol\Library\Framework\CodeContracts

xcopy /y Havit.PayMuzo\bin\Release\Havit.PayMuzo.dll \\topol\Library\Framework\
xcopy /y Havit.PayMuzo\bin\Release\Havit.PayMuzo.xml \\topol\Library\Framework\
xcopy /y Havit.PayMuzo\bin\Release\Havit.PayMuzo.pdb \\topol\Library\Framework\
rem xcopy /y Havit.PayMuzo\bin\Release\CodeContracts\Havit.PayMuzo.Contracts.dll \\topol\Library\Framework\CodeContracts
rem xcopy /y Havit.PayMuzo\bin\Release\CodeContracts\Havit.PayMuzo.Contracts.pdb \\topol\Library\Framework\CodeContracts

xcopy /y Scripts\*.js \\topol\Library\Scripts\
xcopy /y Styles\*.css \\topol\Library\Styles\


pause
