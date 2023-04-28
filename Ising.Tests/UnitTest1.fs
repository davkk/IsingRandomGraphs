module Ising.Tests

open System.IO
open NUnit.Framework
open FsUnit

open Lattice
open Domain
open Gtk

let parameters: Params =
    {
        Rng = System.Random(2001)
        Sweeps = 5_000
        LatticeSize = 32
        Beta = 1.8
    }

let sampleLattice =
    let lines =
        File.ReadAllLines(
            Path.Combine(
                __SOURCE_DIRECTORY__,
                "data",
                $"2001-{parameters.Sweeps}-{parameters.LatticeSize}-{parameters.Beta}"
            )
        )

    Lattice(
        lines
        |> Array.map (fun line -> sbyte line),
        lines.Length
    )

[<Test>]
let Test1 () =
    Gtk.Application.Init()

    use window =
        new Window(
            $"{parameters.Sweeps}, {parameters.LatticeSize}, {parameters.Beta}"
        )

    use drawing = new DrawingArea()

    let windowSize = 512
    window.SetDefaultSize(windowSize, windowSize)

    let mutable lattice = Lattice(parameters)
    let cellSize = windowSize / lattice.Size

    let _ = Ising.simulate parameters (&lattice)

    drawing.Drawn.Add(fun args ->
        let cr = args.Cr

        for i in 0 .. lattice.Size - 1 do
            for j in 0 .. lattice.Size - 1 do
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

    window.Add(drawing)

    window.DeleteEvent.AddHandler(fun _ args ->
        Gtk.Application.Quit()
        args.RetVal <- true
    )

    window.ShowAll()
    window.Show()

    Gtk.Application.Run()

    lattice |> should equal sampleLattice
