namespace MatDataProvider.Tests

open System
open MatDataProvider
open NUnit.Framework

module SimpleTypes =

    [<Literal>]
    let simpleTypesFile = __SOURCE_DIRECTORY__ + "/../../data/simpleTypes.mat"

    type M = MatFile<simpleTypesFile>

    [<Test>]
    let ``single-dimensional bools array``() =
        Assert.AreEqual([| true; false |], M.x.bools)

    [<Test>]
    let ``string (chars array)``() =
        Assert.AreEqual("test string", M.x.chars)

    [<Test>]
    let ``single-dimensional doubles array``() =
        // realmax('double'); 0; realmin('double')
        approxEqual [| Double.MaxValue; 0.0; 2.225073859e-308 |] M.x.doubles

    [<Test>]
    let ``single-dimensional singles array``() =
        // realmax('single'); 0; realmin('single')
        approxEqual [| Single.MaxValue; 0.0f; 1.17549435e-38f |] M.x.singles

    [<Test>]
    let ``single-dimensional signed bytes array``() =
        approxEqual [| SByte.MaxValue; 0y; SByte.MinValue |] M.x.int8row

    [<Test>]
    let ``single-dimensional signed int16 array``() =
        approxEqual [| Int16.MaxValue; 0s; Int16.MinValue |] M.x.int16row

    [<Test>]
    let ``single-dimensional signed int32 array``() =
        approxEqual [| Int32.MaxValue; 0; Int32.MinValue |] M.x.int32row

    [<Test>]
    let ``single-dimensional signed int64 array``() =
        approxEqual [| Int64.MaxValue; 0L; Int64.MinValue |] M.x.int64row

    [<Test>]
    let ``two-dimensional unsigned bytes array``() =
        approxEqual [| [|Byte.MaxValue|]; [|0uy|]; [|Byte.MinValue|] |] M.x.uint8col

    [<Test>]
    let ``two-dimensional unsigned int16 array``() =
        approxEqual [| [|UInt16.MaxValue|]; [|0us|]; [|UInt16.MinValue|] |] M.x.uint16col

    [<Test>]
    let ``two-dimensional unsigned int32 array``() =
        approxEqual [| [|UInt32.MaxValue|]; [|0u|]; [|UInt32.MinValue|] |] M.x.uint32col

    [<Test>]
    let ``two-dimensional unsigned int64 array``() =
        approxEqual [| [|UInt64.MaxValue|]; [|0UL|]; [|UInt64.MinValue|] |] M.x.uint64col

    [<Test>]
    let ``cell array with 2 string cells``() =
        Assert.AreEqual ("test", M.x.strcell.Cell_0)
        Assert.AreEqual ("string", M.x.strcell.Cell_1)
