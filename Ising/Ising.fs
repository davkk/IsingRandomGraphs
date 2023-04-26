module Ising

open Domain
open Lattice

let simulate (parameters: Params) (lattice: Lattice byref) : Stats =
    let probabilities =
        [| for dE in -8. .. 4. .. 8. -> exp (-parameters.Beta * dE) |]

    let mutable energy =
        lattice |> Lattice.totalEnergy

    let mutable magnet =
        lattice |> Lattice.totalMagnet

    let mutable sweeps = parameters.Sweeps

    while sweeps > 0 do
        let i, j =
            parameters.Rng.Next(0, lattice.Size),
            parameters.Rng.Next(0, lattice.Size)

        let neighbors =
            lattice |> Lattice.sumNeighbors (i, j)

        let dE = 2y * lattice[i, j] * neighbors

        let dM = -2y * lattice[i, j]

        let shouldFlip =
            parameters.Rng.NextDouble() < probabilities[int dE / 4 + 2]
            || dE < 0y

        if shouldFlip then
            lattice[i, j] <- -lattice[i, j]

            energy <- energy + float dE
            magnet <- magnet + float dM

        sweeps <- sweeps - 1

    {
        AvgE = energy / float lattice.Length
        AvgM = magnet / float lattice.Length
    }
