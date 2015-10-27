namespace MatDataProvider

open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open System.Reflection
open MatTypesBuilder

[<TypeProvider>]
type MatDataProvider (cfg : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces ()
    let context = new Context(this, cfg)

    let ns = "MatDataProvider"
    let asm = Assembly.GetExecutingAssembly()

    let staticParams = [ ProvidedStaticParameter("fileName", typeof<string>) ]
    let fileType = ProvidedTypeDefinition(asm, ns, "MatFile", Some typeof<obj>, HideObjectMethods = true)

    do fileType.DefineStaticParameters(
        parameters = staticParams,
        instantiationFunction = (fun typeName parameterValues ->
            match parameterValues with
            | [| :? string as fileName |] ->
                let ty = ProvidedTypeDefinition(asm, ns, typeName, None)
                createTypes ty fileName
                context.WatchFile fileName
                ty
            | _ -> failwith "Unexpected parameter values"))

    do this.AddNamespace(ns, [fileType])
    do this.Disposing.Add (fun _ -> dispose context)
         
[<assembly:TypeProviderAssembly>]
do ()