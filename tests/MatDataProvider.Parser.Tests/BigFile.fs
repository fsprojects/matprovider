namespace MatDataProvider.Tests

open System
open MatDataProvider
open NUnit.Framework
open MatDataProvider.MatFileReader

module BigFile =

    [<Literal>]
    let bigMatFile = __SOURCE_DIRECTORY__ + "/../../data/ex3data1.mat"


    [<Test>]
    let ``should read the file``() =
        let X = MatFileReader.readFile bigMatFile

        let name1,content1 = X |> Seq.head
        Assert.AreEqual(name1,"y")
        match content1 with
        | MatData.MatArray arr -> Assert.AreEqual(arr.Length,5000)
        | _ -> failwith "wrong type"

        let name2,content2 = X |> Seq.skip 1 |> Seq.head
        Assert.AreEqual(name2,"X")
        match content2 with
        | MatData.MatArray arr -> Assert.AreEqual(arr.Length,5000)
        | _ -> failwith "wrong type"