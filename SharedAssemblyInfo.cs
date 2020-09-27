using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Distant Object Enhancement KSP Plugin")]
[assembly: AssemblyProduct("Distant Object Enhancement")]
[assembly: AssemblyDescription("KSP Plugin to render distant planets and spacecraft")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Duckytopia / MOARdV / TheDarkBadger")]
[assembly: AssemblyCopyright("Copyright © 2014-2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.
[assembly: AssemblyVersion("2.0.1.0")]

// Use KSPAssembly to allow other DLLs to make this DLL a dependency in a 
// non-hacky way in KSP.  Format is (AssemblyProduct, major, minor), and it 
// does not appear to have a hard requirement to match the assembly version. 
[assembly: KSPAssembly("DistantObject", 2, 1)]

// The following attributes are used to specify the signing key for the assembly,
// if desired. See the Mono documentation for more information about signing.
//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]
