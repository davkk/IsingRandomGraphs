module Domain

[<Struct>]
type Params =
    {
        Rng: System.Random
        Sweeps: int
        LatticeSize: int
        Beta: float
    }

[<Struct>]
type Stats =
    {
        AvgE: float
        AvgM: float
    }
