namespace MatDataProvider.Tests

open System
open MatDataProvider
open NUnit.Framework

module BigFile =

    [<Literal>]
    let bigMatFile = __SOURCE_DIRECTORY__ + "/../../data/ex3data1.mat"

    type M = MatFile<bigMatFile>

    [<Test>]
    [<Ignore("Fix is to be implemented.")>]
    let ``should read the file``() =
        let X = M.X
        Assert.AreEqual([| true; false |], M.X)