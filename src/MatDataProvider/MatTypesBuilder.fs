namespace MatDataProvider

module internal MatTypesBuilder =
    open Microsoft.FSharp.Core.CompilerServices
    open Microsoft.FSharp.Quotations
    open ProviderImplementation.ProvidedTypes
    open System.Numerics
    open MatDataTypes
    open MatFileReader

    let rec arrayToExpr (arr: System.Array) skipSingleton =
        let elemType = arr.GetType().GetElementType()

        let rec createExprs i res =
            if i < arr.Length then      
                let elem = 
                    if elemType.GetElementType() = null then       
                        if elemType = typeof<Complex> then
                            let num = arr.GetValue i :?> Complex
                            let real, imaginary = num.Real, num.Imaginary
                            <@@ Complex(real, imaginary) @@>
                        else 
                            Expr.Coerce(Expr.Value(arr.GetValue i), elemType)
                    else arrayToExpr (arr.GetValue i :?> System.Array) false |> snd
                createExprs (i+1) (elem::res)
            else List.rev res

        let exprs = createExprs 0 []
        if skipSingleton && arr.Length = 1 then 
            elemType, exprs.Head
        else
            arr.GetType(), (Quotations.Expr.NewArray(elemType, exprs))

    let inline provideProperty (ty: ProvidedTypeDefinition) name propTy expr =
        ProvidedProperty(name, propTy, IsStatic = true, GetterCode = (fun _ -> expr))
        |> ty.AddMember

    let inline provideSubtype (ty: ProvidedTypeDefinition) name =
        let subtype = ProvidedTypeDefinition(name, Some typeof<obj>, HideObjectMethods = true)
        ty.AddMember subtype
        subtype

    let inline concreteArrayExpr (arr: 'T[]) =
        if arr.Length = 1 then
            let v = unbox<'T> arr.[0] in Some (typeof<'T>, <@@ v @@>)
        else Some (typeof<'T[]>, <@@ arr @@>)

    let inline (|DoubleArray|_|) data =
        match data with
        | MatArray (:? (double[]) as arr) -> concreteArrayExpr arr
        | MatArray (:? (double[][]) as arr) -> concreteArrayExpr arr
        | MatArray (:? (double[][][]) as arr) -> concreteArrayExpr arr
        | _ -> None

    let rec addMembers (ty: ProvidedTypeDefinition) (name, data) =
        if not (System.String.IsNullOrWhiteSpace name) then 
            match data with
            | DoubleArray (exprType, expr) -> 
                provideProperty ty name exprType expr
            | MatArray arr -> 
                let exprType, expr = arrayToExpr arr true
                provideProperty ty name exprType expr
            | MatStructure fields -> 
                let structTy = provideSubtype ty name
                Array.iter (addMembers structTy) fields
            | MatString str -> 
                provideProperty ty name typeof<string> <@@ str @@>
            | MatCells cells -> 
                let cellsTy = provideSubtype ty name
                Array.iteri (fun i cell -> addMembers cellsTy ("Cell_" + string i, cell)) cells
            | _ -> ()

    let createTypes (ty: ProvidedTypeDefinition) fileName =
        List.iter (addMembers ty) (let f = readFile fileName in printfn "%A" f; f)
