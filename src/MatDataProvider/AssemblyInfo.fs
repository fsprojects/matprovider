namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("MatDataProvider")>]
[<assembly: AssemblyProductAttribute("MatDataProvider")>]
[<assembly: AssemblyDescriptionAttribute("Type provider for .mat files")>]
[<assembly: AssemblyVersionAttribute("0.0.1")>]
[<assembly: AssemblyFileVersionAttribute("0.0.1")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.0.1"
