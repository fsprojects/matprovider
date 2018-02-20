MatDataProvider
=======================
  
#### What is it?

MatDataProvider is an erased type provider for .mat files (binary MATLAB format files).

#### Usage

Here's a simple example of reading 'simulation.mat' file, where 'parameters' and 'results' 
are the variables stored there (a struct and array):

```fsharp
[<Literal>]
let fileName = "/path/to/file/simulation.mat"

type Simulation = MatDataProvider.MatFile<fileName>

Simulation.parameters.paths
Simulation.parameters.seed
...
Simulations.results
```

#### Limitations

This is an initial implementation and certain element types are not supported 
(e.g. sparse matrices) or not finalized yet (cell arrays). There're some sample .mat files
in the 'data' folder.

#### Build status

[![Build status](https://ci.appveyor.com/api/projects/status/hb814824p50t9pj2?svg=true)](https://ci.appveyor.com/project/luajalla/matprovider)
[![Build Status](https://travis-ci.org/fsprojects/matprovider.svg)](https://travis-ci.org/fsprojects/matprovider)

#### Links

Mat-file format description ([pdf](http://www.mathworks.com/help/pdf_doc/matlab/matfile_format.pdf))

