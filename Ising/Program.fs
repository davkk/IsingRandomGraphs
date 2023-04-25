open Domain
open Lattice

open Gtk

let OnDelete (sender: obj) (args: DeleteEventArgs) =
    Gtk.Application.Quit()
    args.RetVal <- true

[<EntryPoint>]
let main _ =
    // Create the window
    Gtk.Application.Init()
    use window = new Window("Ising - 2D lattice")

    let windowSize = 512
    window.SetDefaultSize(windowSize, windowSize)

    use drawing = new DrawingArea()


    let parameters: Params =
        {
            Rng = System.Random(2001)
            Sweeps = 10_000_000
            LatticeSize = 256
            Beta = 1.4
        }

    let result = Ising.simulate parameters

    let cellSize =
        windowSize / parameters.LatticeSize

    drawing.Drawn.Add(fun args ->
        let cr = args.Cr

        for i in 0 .. parameters.LatticeSize - 1 do
            for j in 0 .. parameters.LatticeSize - 1 do
                if result.Lattice[i, j] = 1y then
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

    window.DeleteEvent.AddHandler(fun s a -> OnDelete s a) // fun still needed
    window.ShowAll()
    window.Show()

    Application.Run()

    0
