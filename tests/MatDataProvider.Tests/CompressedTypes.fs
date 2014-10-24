namespace MatDataProvider.Tests

open MatDataProvider
open NUnit.Framework

module CompressedTypes =

    [<Literal>]
    let compressedTypesFile = __SOURCE_DIRECTORY__ + "/../../data/compressedTypes.mat"

    type M = MatFile<compressedTypesFile>

    [<Test>]
    let ``double array stored as signed bytes``() =
        approxEqual [| 127.0; 0.0; -128.0 |] M.y.doubleint8

    [<Test>]
    let ``double array stored as unsigned bytes``() =
        approxEqual [| [|255.0|]; [|0.0|]; [|0.0|] |] M.y.doubleuint8
