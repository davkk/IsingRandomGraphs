open System.Threading
open Domain
open Lattice

open Gtk

let parameters: Params =
    {
        Rng = System.Random(2001)
        Sweeps = 10_000_000
        LatticeSize = 256
        Beta = 0.8
    }

let mutable lattice =
    Lattice.create parameters

let probabilities =
    [| for dE in -8. .. 4. .. 8. -> exp (-parameters.Beta * dE) |]

let mutable energy =
    lattice |> Lattice.totalEnergy

let mutable magnet =
    lattice |> Lattice.totalMagnet

let windowSize = 512

let cellSize =
    windowSize / parameters.LatticeSize

let drawLattice (cr: Cairo.Context) (lattice: Lattice) =
    lattice
    |> Lattice.iter (fun i j ->
        if lattice[i, j] = 1y then
            cr.SetSourceRGB(0.0, 0.0, 0.0)
        else
            cr.SetSourceRGB(1.0, 1.0, 1.0)

        cr.Rectangle(
            new Cairo.Rectangle(
                new Cairo.Point(i * cellSize, j * cellSize),
                cellSize,
                cellSize
            )
        )

        cr.Fill()
    )

let update (area: DrawingArea) =
    let i = parameters.Rng.Next(0, lattice.Size)
    let j = parameters.Rng.Next(0, lattice.Size)

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

    area.QueueDraw()
    true

[<EntryPoint>]
let main _ =
    Gtk.Application.Init()

    use window =
        new Window("Ising - 2D lattice")

    use drawing = new DrawingArea()
    window.SetDefaultSize(windowSize, windowSize)

    drawing.Drawn.Add(fun args -> drawLattice args.Cr lattice)

    let updateTask =
        async {
            while true do
                update drawing |> ignore
        }

    Async.Start(updateTask, CancellationToken.None)

    window.Add(drawing)

    window.DeleteEvent.AddHandler(fun _ args ->
        Application.Quit()
        args.RetVal <- true
    )

    window.ShowAll()
    window.Show()

    Application.Run()

    0
