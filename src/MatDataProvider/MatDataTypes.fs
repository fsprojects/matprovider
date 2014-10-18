namespace MatDataProvider

module MatDataTypes = 
    open System
    open System.IO
    open System.IO.Compression

    [<Literal>]
    let LittleEndianCode = 19785s // [| byte 'I'; byte 'M' |]

    type MatDataType =
        | SByte = 1
        | Byte = 2
        | Int16 = 3
        | UInt16 = 4
        | Int32 = 5
        | UInt32 = 6
        | Single = 7
        | Double = 9
        | Int64 = 12
        | UInt64 = 13
        | Matrix = 14
        | Compressed = 15
        | Utf8 = 16
        | Utf16 = 17
        | Utf32 = 18
    
    let inline matDataType x = enum<MatDataType> x

    type MatArrayType = 
        | CellArray = 1
        | Structure = 2
        | Object = 3
        | Char = 4
        | Sparse = 5
        | Double = 6
        | Single = 7
        | SByte = 8
        | Byte = 9
        | Int16 = 10
        | UInt16 = 11
        | Int32 = 12
        | UInt32 = 13
        | Int64 = 14
        | UInt64 = 15

    let inline matArrayType x = enum<MatArrayType> x

    // size of a single element in bytes
    let sizeOfElement dataType =  
        match dataType with
        | MatDataType.SByte | MatDataType.Byte -> 1
        | MatDataType.Int16 | MatDataType.UInt16 -> 2
        | MatDataType.Int32 | MatDataType.UInt32 
        | MatDataType.Single -> 4
        | MatDataType.Int64 | MatDataType.UInt64
        | MatDataType.Double -> 8
        | MatDataType.Utf8  | MatDataType.Utf16 | MatDataType.Utf32 -> 1 
        | _ -> failwith (string dataType + " is not supported")
 
    let mapArrayToDataType dataType =
        match dataType with
        | MatArrayType.Byte   -> MatDataType.Byte
        | MatArrayType.SByte  -> MatDataType.SByte
        | MatArrayType.Int16  -> MatDataType.Int16
        | MatArrayType.UInt16 -> MatDataType.UInt16
        | MatArrayType.Int32  -> MatDataType.Int32
        | MatArrayType.UInt32 -> MatDataType.UInt32
        | MatArrayType.Int64  -> MatDataType.Int64
        | MatArrayType.UInt64 -> MatDataType.UInt64
        | MatArrayType.Single -> MatDataType.Single
        | MatArrayType.Double -> MatDataType.Double
        | MatArrayType.Char   -> MatDataType.Utf8
        | _ -> failwith ("unexpected data to array type conversion: " + string dataType)
    
    let mapDataToSystemType dataType =
        match dataType with
        | MatDataType.Byte -> typeof<byte>
        | MatDataType.SByte -> typeof<sbyte> 
        | MatDataType.Int16 -> typeof<int16>
        | MatDataType.UInt16 -> typeof<uint16>
        | MatDataType.Int32 -> typeof<int32>
        | MatDataType.UInt32 -> typeof<uint32>
        | MatDataType.Int64 -> typeof<int64>
        | MatDataType.UInt64 -> typeof<uint64>
        | MatDataType.Single -> typeof<single>
        | MatDataType.Double -> typeof<double>
        | _ -> failwith ("unexpected data to system type conversion: " + string dataType)