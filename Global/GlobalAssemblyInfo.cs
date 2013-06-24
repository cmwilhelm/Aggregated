using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyProduct("Aggregated")]
[assembly: AssemblyCompany("Aggregated Contributors")]
[assembly: AssemblyCopyright("Copyright © 2013")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyVersion("0.0.0")]

[assembly: ComVisible(false)]
