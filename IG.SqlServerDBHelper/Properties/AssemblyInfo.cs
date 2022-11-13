using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("IG.SqlServerDBHelper")]
[assembly: AssemblyDescription("Assembly per eseguire le query verso Sql Server")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("IGConsulting")]
[assembly: AssemblyProduct("IG.SqlServerDBHelper")]
[assembly: AssemblyCopyright("Copyright © IGConsulting 2014-2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8349bc1e-7b1b-4684-afa7-337bdef017e5")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("4.7.2.*")]
[assembly: AssemblyFileVersion("4.7.2.0")]

/*
Versioni
4.0.4.0 [23/01/2017]
    Nessuna implementazione -> Compilazione per modifiche in IGCal.Security
                                   + Modificati namespace da IGCal.DB a IGCal.Security in classi di LowLevelFunctions
                                       - DB2UDBSecurity.cs
                                       - MySQLSecurity.cs
                                       - OracleSecurity.cs
                                       - SQLServer650Security.cs
                                       - SQLServer700Security.cs
                                       - SQLServerSecurity.cs
                                       - SybaseSecurity.cs
                                   + Nel controllo della versione di SQL viene distinto il caso di SQL Server 6.5
                                     da tutti gli altri (compresi eventuali errori nel recupero della versione
                                     a causa della stringa ritornata da SQL non comprensibile).
4.0.5.0: Nessuna implementazione -> Compilazione per modifiche in IGCal.DB
4.0.6.0 [15/01/2018]
    Riportate eventuali modifiche dal IGCal 3
4.6.8: Aggiunta referenza a Vertica
*/
