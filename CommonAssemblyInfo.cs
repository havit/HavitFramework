using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("HAVIT, s.r.o.")]
[assembly: AssemblyProduct("HAVIT .NET Framework Extensions")]
[assembly: AssemblyCopyright("Copyright HAVIT, s.r.o.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

[assembly: AssemblyVersion("2.0.*")]

[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Retail")]
#endif
