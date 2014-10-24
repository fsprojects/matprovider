namespace MatDataProvider.Tests

open MatDataProvider
open NUnit.Framework

module CompressedTypes =

    [<Literal>]
    let compressedTypesFile = __SOURCE_DIRECTORY__ + "/../../data/compressedTypes.mat"

    type M = MatFile<compressedTypesFile>

    [<Test>]
    let ``int32 array stored as signed bytes``() =
        approxEqual [| 127; 0; -128 |]  M.y.int32int8

    [<Test>]
    let ``int32 array stored as unsigned bytes``() =
        approxEqual [| [|255|]; [|0|]; [|0|] |] M.y.int32uint8

    [<Test>]
    let ``int64 array stored as signed int32``() =
        approxEqual [| 2147483647L; 0L; -2147483648L |] M.y.int64int32

    [<Test>]
    let ``int64 array stored as unsigned int32``() =
        approxEqual [| [|4294967295L|]; [|0L|]; [|0L|] |] M.y.int64uint32

    [<Test>]
    let ``double array stored as signed bytes``() =
        approxEqual [| 127.0; 0.0; -128.0 |] M.y.doubleint8

    [<Test>]
    let ``double array stored as unsigned bytes``() =
        approxEqual [| [|255.0|]; [|0.0|]; [|0.0|] |] M.y.doubleuint8

    [<Test>]
    let ``double array stored as signed int16``() =
        approxEqual [| 32767.0; 0.0; -32768.0 |] M.y.doubleint16

    [<Test>]
    let ``double array stored as unsigned int16``() =
        approxEqual [| [|65535.0|]; [|0.0|]; [|0.0|] |] M.y.doubleuint16

    [<Test>]
    let ``double array stored as signed int32``() =
        approxEqual [| 2147483647.0; 0.0; -2147483648.0 |] M.y.doubleint32

    [<Test>]
    let ``double array stored as unsigned int32``() =
        approxEqual [| [|4294967295.0|]; [|0.0|]; [|0.0|] |] M.y.doubleuint32

    [<Test>]
    let ``double array stored as signed int64``() =
        approxEqual [| 9223372036854775807.0; 0.0; -9223372036854775808.0 |] M.y.doubleint64

    [<Test>]
    let ``double array stored as unsigned int64``() =
        approxEqual [| [|18446744073709551615.0|]; [|0.0|]; [|0.0|] |] M.y.doubleuint64