namespace MatDataProvider.Tests

open NUnit.Framework

[<AutoOpen>]
module TestUtils =
    let inline approxEqual (expected: _[]) (actual: _[]) = 
        Assert.That(actual, Is.EqualTo(expected).Within 1e-7)
