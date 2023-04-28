open Domain
open Lattice

[<EntryPoint>]
let main _ =
    let parameters: Params =
        {
            Rng = System.Random(2001)
            Sweeps = 5_000
            LatticeSize = 32
            Beta = 1.8
        }

    let mutable lattice = Lattice(parameters)
    let _ = Ising.simulate parameters (&lattice)

    for spin in lattice do
        printf "%A; " spin

    0
