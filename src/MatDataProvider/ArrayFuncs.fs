namespace MatDataProvider

[<AutoOpen>]
module ArrayFuncs =
    open System

    // define all necessary array types from T[][]..[] to T[]
    let inline private types (t: Type) n =
        let res = Array.zeroCreate n
        res.[n - 1] <- t
        let rec make i =
            if i >= 0 then 
                res.[i] <- res.[i + 1].MakeArrayType()
                make (i - 1)
        make (n - 2)
        res

    // prod of prev dimenstions: [|2; 3; 4|] -> [|1; 2; 6|]
    let inline private prods (arr: _[]) =
        let res = Array.zeroCreate arr.Length
        res.[0] <- 1
        let rec prod i =
            if i < arr.Length then 
                res.[i] <- arr.[i - 1] * res.[i - 1]
                prod (i + 1)
        prod 1
        res

    // init array of specific type
    let inline private fill t (len: int) f =
        let xs = Array.CreateInstance(t, len)
        for i in 0..len - 1 do xs.SetValue(f i, i)
        xs
       
    let reshape (arr: Array) (dims: int[]) = 
        let t = arr.GetType().GetElementType()

        let ps = prods dims
        let ts = types t dims.Length

        let rec init dim k =
            if dim = dims.Length - 1 then
                fill ts.[dim] dims.[dim] (fun i -> arr.GetValue (ps.[dim] * i + k))
            else 
                fill ts.[dim] dims.[dim] (fun i -> init (dim+1) (ps.[dim] * i + k))
        init 0 0

    let inline convert (arr: Array) (targetType: Type) = 
        fill targetType arr.Length (fun i -> Convert.ChangeType(arr.GetValue i, targetType))

    let inline private complex r i =
        Numerics.Complex(
            Convert.ChangeType(r, typeof<double>) |> unbox, 
            Convert.ChangeType(i, typeof<double>) |> unbox)

    let inline internal toComplexArray (real: Array) (im: Array) =
        if im = null || real.Length <> im.Length then 
            Array.init real.Length (fun i -> complex (real.GetValue i) 0.0)
        else 
            Array.init real.Length (fun i -> complex (real.GetValue i) (im.GetValue i))
