module Ising

open Domain
open Lattice

[<Struct>]
type Stats =
    {
        Lattice: Lattice
        Beta: float
        AvgE: float
        AvgM: float
    }

let simulate (parameters: Params) : Stats =
    let probabilities =
        [| for dE in -8. .. 4. .. 8. -> exp (-parameters.Beta * dE) |]

    let mutable lattice = Lattice(parameters)

    let rec loop (sweep, energy, magnetization) =
        if sweep = parameters.Sweeps then
            {
                Lattice = lattice
                Beta = parameters.Beta
                AvgE = energy / float lattice.Spins.Length
                AvgM =
                    magnetization
                    / float lattice.Spins.Length
            }

        else
            let i, j =
                parameters.Rng.Next(0, lattice.Size),
                parameters.Rng.Next(0, lattice.Size)

            let neighborSum =
                lattice |> Lattice.sumNeighbors (i, j)

            let dE = 2y * lattice[i, j] * neighborSum
            let dM = -2y * lattice[i, j]

            let shouldFlip =
                parameters.Rng.NextDouble() < probabilities[int dE / 4 + 2]
                || dE < 0y

            // TODO: byref??
            if shouldFlip then
                lattice[i, j] <- -lattice[i, j]

                loop (sweep + 1, energy + float dE, magnetization + float dM)
            else
                loop (sweep + 1, energy, magnetization)

    loop (
        1,
        lattice |> Lattice.totalEnergy,
        lattice |> Lattice.totalMagnetization
    )
