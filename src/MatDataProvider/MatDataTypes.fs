namespace MatDataProvider

module MatDataTypes = 
    open System
    open System.IO
    open System.IO.Compression

    [<Literal>]
    let LittleEndianCode = 19785s // [| byte 'I'; byte 'M' |]

    /// MAT-File Data Types
    type MatDataType =
        /// 8-bit, signed
        | Int8 = 1
        /// 8-bit, unsigned
        | UInt8 = 2
        /// 16-bit, signed
        | Int16 = 3
        /// 16-bit, unsigned
        | UInt16 = 4
        /// 32-bit, signed
        | Int32 = 5
        /// 32-bit, unsigned
        | UInt32 = 6
        /// IEEE 754 single format
        | Single = 7
        /// IEEE 754 double format
        | Double = 9
        /// 64-bit, signed
        | Int64 = 12
        /// 64-bit, unsigned
        | UInt64 = 13
        /// MATLAB array
        | Matrix = 14
        /// Compressed Data
        | Compressed = 15
        /// Unicode UTF-8 Encoded Character Data
        | Utf8 = 16
        /// Unicode UTF-16 Encoded Character Data
        | Utf16 = 17
        /// Unicode UTF-32 Encoded Character Data
        | Utf32 = 18
    
    let inline matDataType x = enum<MatDataType> x

    /// MATLAB Array Types
    type MatArrayType = 
        /// Cell array
        | CellArray = 1
        /// Structure
        | Structure = 2
        /// Object
        | Object = 3
        /// Character array
        | Char = 4
        /// Sparse array
        | Sparse = 5
        /// Double precision array
        | Double = 6
        /// Single precision array
        | Single = 7
        /// 8-bit, signed integer
        | Int8 = 8
        /// 8-bit, unsigned integer
        | UInt8 = 9
        /// 16-bit, signed integer
        | Int16 = 10
        /// 16-bit, unsigned integer
        | UInt16 = 11
        /// 32-bit, signed integer
        | Int32 = 12
        /// 32-bit, unsigned integer
        | UInt32 = 13
        /// 64-bit, signed integer
        | Int64 = 14
        /// 64-bit, unsigned integer
        | UInt64 = 15

    let inline matArrayType x = enum<MatArrayType> x

    /// size of a single element in bytes
    let sizeOfElement dataType =  
        match dataType with
        | MatDataType.Int8 | MatDataType.UInt8 -> 1
        | MatDataType.Int16 | MatDataType.UInt16 -> 2
        | MatDataType.Int32 | MatDataType.UInt32 
        | MatDataType.Single -> 4
        | MatDataType.Int64 | MatDataType.UInt64
        | MatDataType.Double -> 8
        | MatDataType.Utf8  | MatDataType.Utf16 | MatDataType.Utf32 -> 1 
        | _ -> failwith (string dataType + " is not supported")
 
    let mapArrayToDataType dataType =
        match dataType with
        | MatArrayType.Int8   -> MatDataType.Int8
        | MatArrayType.UInt8  -> MatDataType.UInt8
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
        | MatDataType.UInt8 -> typeof<byte>
        | MatDataType.Int8 -> typeof<sbyte> 
        | MatDataType.Int16 -> typeof<int16>
        | MatDataType.UInt16 -> typeof<uint16>
        | MatDataType.Int32 -> typeof<int32>
        | MatDataType.UInt32 -> typeof<uint32>
        | MatDataType.Int64 -> typeof<int64>
        | MatDataType.UInt64 -> typeof<uint64>
        | MatDataType.Single -> typeof<single>
        | MatDataType.Double -> typeof<double>
        | _ -> failwith ("unexpected data to system type conversion: " + string dataType)

    let mapArrayToSystemType = mapArrayToDataType >> mapDataToSystemType