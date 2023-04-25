module Domain

[<Struct>]
type Params =
    {
        Rng: System.Random
        Sweeps: int
        LatticeSize: int
        Beta: float
    }

// [<Struct>]
// type Stats =
//     {
//         Lattice: Lattice
//         Beta: float
//         AvgE: float
//         AvgM: float
//     }
