namespace Lattice

open Domain

module private Helpers =
    let inline (%/) a b = (a + b) % b

    let isInRange (i: int) (j: int) (size: int) =
        if i >= 0 && i < size && j >= 0 && j < size then
            true
        else
            raise
            <| System.IndexOutOfRangeException
                $"Either i={i} or j={j} was out of range"

            false

open Helpers

[<Struct>]
type Lattice(spins: sbyte array, size: int) =
    new(parameters: Params) =
        let size = parameters.LatticeSize

        let lattice =
            Array.zeroCreate<sbyte> (size * size)

        lattice
        |> Array.iteri (fun i _ ->
            lattice.[i] <- if parameters.Rng.NextDouble() < 0.5 then 1y else -1y
        )

        Lattice(lattice, size)

    member _.Spins = spins
    member _.Size = size

    member self.Item
        with inline get (i: int, j: int) =
            let i = i %/ self.Size
            let j = j %/ self.Size
            self.Spins[i + j * self.Size]

        and inline set (i: int, j: int) value =
            let i = i %/ self.Size
            let j = j %/ self.Size
            self.Spins[i + j * self.Size] <- value


module Lattice =
    let inline sumNeighbors (i: int, j: int) (lattice: Lattice) =
        lattice[i - 1, j]
        + lattice[i + 1, j]
        + lattice[i, j - 1]
        + lattice[i, j + 1]

    let inline totalEnergy (lattice: Lattice) =
        let mutable sum = 0

        for i in 0 .. lattice.Size - 1 do
            for j in 0 .. lattice.Size - 1 do
                let neighborSum =
                    lattice |> sumNeighbors (i, j)

                sum <- sum + int (lattice[i, j] * neighborSum)

        -float sum / 2.0

    let inline totalMagnetization (lattice: Lattice) =
        let mutable sum = 0

        for i in 0 .. lattice.Size - 1 do
            for j in 0 .. lattice.Size - 1 do
                sum <- sum + int (lattice[i, j])

        float sum
