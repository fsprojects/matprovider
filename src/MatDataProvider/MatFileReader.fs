namespace MatDataProvider

module MatFileReader =
    open System
    open System.IO
    open System.IO.Compression
    open System.Text
    open MatDataTypes

    type MatData =
        | MatArray of arr: System.Array
        | MatString of str: string
        | MatStructure of fields: (string * MatData)[]
        | MatCells of cells: MatData[]
        | Unknown

    let readHeader (reader: BinaryReader) =
        let text = String (reader.ReadChars 116)
        let subsysDataOffset = reader.ReadBytes 8
        let vBytes = reader.ReadBytes 2
        
        if LittleEndianCode = reader.ReadInt16() then
            let description = text.Trim() 
            let version = int vBytes.[0] * 16 + int vBytes.[1]
            let littleEndian = true
            description, version, littleEndian
        else 
            failwith "Big Endian is not supported"

    let inline padding size n =
        let p = size % n
        if p = 0 then 0 else n - p 

    // get data type, # of bytes and padding
    let readTag (reader: BinaryReader) = 
        let dataType = reader.ReadInt32()
        if dataType >>> 16 = 0 then
            let size = reader.ReadInt32() // # of bytes
            matDataType dataType, size, padding size 8
        else
            let size = dataType >>> 16
            let compressedType = dataType &&& 0xffff
            matDataType compressedType, size, padding size 4
 
    let inline skip (reader: BinaryReader) n =
        reader.BaseStream.Seek(int64 n, SeekOrigin.Current) |> ignore

    let inline readElement (reader: BinaryReader) f =
        let dataType, size, padding = readTag reader
        let res = f size dataType
        if dataType <> MatDataType.Compressed then skip reader padding
        res

    let readString (reader: BinaryReader) (enc: Encoding) size =
        let bytes = reader.ReadBytes size
        match Array.tryFindIndex ((=)0uy) bytes with
        | Some i -> enc.GetString (bytes, 0, i)
        | _ -> enc.GetString bytes

    let inline readName (reader: BinaryReader) =
        readElement reader (fun len _ -> readString reader Encoding.UTF8 len)

    let inline (|ArrayDims|_|) data =
        match data with
        | MatArray (:? (int[]) as dims) -> Some dims
        | _ -> None

    let inline getArrayDims (_, data) = 
        match data with
        | ArrayDims [| 1; n |] -> [| n |]
        | ArrayDims dims -> dims
        | _ -> failwith "invalid array dimensions format"

    // arrayType, isComplex, isGlobal, isLogical
    let inline getArrayFlags (_, data) =
        match data with
        | MatArray (:? (uint32[]) as arr) -> 
            let value = arr.[0]
            let cl    = value &&& 0x00fful
            let flags = (value &&& 0xff00ul) >>> 8 |> byte
            let complex, glbl, logical = flags &&& 8uy, flags &&& 4uy, flags &&& 2uy
            matArrayType (int cl), complex <> 0uy, glbl <> 0uy, logical <> 0uy
        | _ -> failwith "invalid matrix format"

    // read array of 'dataType' values ('size' bytes) as array of 'arrayType' values
    let readArray (reader: BinaryReader) size dataType arrayType =
        let elem = sizeOfElement dataType
        let n = size / elem
        let bytes = reader.ReadBytes size

        let actualType = mapDataToSystemType dataType
        let values = Array.CreateInstance(actualType, n)
        Buffer.BlockCopy(bytes, 0, values, 0, size)

        if dataType = arrayType then 
            MatArray values
        else
            let targetType = mapDataToSystemType arrayType
            MatArray (convert values targetType)

    let rec readData (reader: BinaryReader) =
        readElement reader (fun size -> function
            | MatDataType.Compressed -> decompress reader size
            | MatDataType.Matrix -> readMatrix reader size
            | dataType -> null, readArrayData reader size dataType dataType)
    
    and readArrayData (reader: BinaryReader) size dataType arrayType = 
        match dataType with
        | MatDataType.Utf8 -> MatString (readString reader Encoding.UTF8 size)
        | MatDataType.Utf16 -> MatString (readString reader Encoding.Unicode size)
        | MatDataType.Utf32 -> MatString (readString reader Encoding.UTF32 size)
        | _ -> readArray reader size dataType arrayType
        
    // make recursive function because of binary reader 
    and decompress (reader: BinaryReader) size =
        use mstream = new MemoryStream()
        use res = new zlib.ZOutputStream(mstream)
        res.Write(reader.ReadBytes size, 0, size)
        mstream.Position <- 0L

        readData (new BinaryReader(mstream))
     
    and readFieldNames (reader: BinaryReader) =
        match readData reader |> snd with
        | MatArray (:? (int[]) as values) ->
            let nameLen = values.[0]
            readElement reader (fun len _ ->
                Array.init (len / nameLen) (fun _ -> readString reader Encoding.UTF8 nameLen))
        | _ -> failwith "invalid field names format"

    and readMatrix (reader: BinaryReader) size =
        let originalPos = reader.BaseStream.Position

        let arrayType, complex, _, logical = getArrayFlags (readData reader)
        let dims = getArrayDims (readData reader)
        let name = readName reader

        match arrayType with
        | MatArrayType.Structure -> 
            let fieldNames = readFieldNames reader
            let fields = Array.map (fun name -> name, readData reader |> snd) fieldNames
            name, MatStructure fields
        | MatArrayType.CellArray ->
            let n = Array.reduce ( * ) dims
            let cells = Array.init n (fun _ -> readData reader |> snd)
            name, MatCells cells
        | MatArrayType.Sparse ->
            skip reader (reader.BaseStream.Position - originalPos + int64 size)
            name, Unknown
        | _ -> 
            let arrayData = readElement reader (fun size dataType ->
                readArrayData reader size dataType (mapArrayToDataType arrayType))

            match arrayData with
            | MatArray (:? (byte[]) as values) when logical ->
                let bools = Array.map ((<>)0uy) values
                name, MatArray (reshape bools dims)
            | MatArray realValues when complex -> 
                let imValues = 
                    match readData reader with
                    | _, MatArray v -> v
                    | _ -> null
                name, MatArray (reshape (toComplexArray realValues imValues) dims)
            | MatArray values ->
                name, MatArray (reshape values dims)
            | MatString values ->
                name, arrayData
            | _ -> name, Unknown
           
    let readFile fileName =
        use str = new FileStream(fileName, FileMode.Open)
        use reader = new BinaryReader(str)
        let headerInfo = readHeader reader

        let len = str.Length
        let rec readAllData res =
            if reader.BaseStream.Position < len then
                readAllData (readData reader :: res)
            else res
        
        readAllData []
