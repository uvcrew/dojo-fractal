open System
open System.Drawing
open System.Windows.Forms

let background = Color.Black
let foreground = Brushes.NavajoWhite

// Create a form to display the graphics
let width, height = 1024, 1024       
let form = new Form(Width = width, Height = height)
let box = new PictureBox(BackColor = background, Dock = DockStyle.Fill)
let image = new Bitmap(width, height)
let graphics = Graphics.FromImage image
//The following line produces higher quality images, 
//at the expense of speed. Uncomment it if you want
//more beautiful images, even if it's slower.
//Thanks to https://twitter.com/AlexKozhemiakin for the tip!
graphics.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.HighQuality
box.Image <- image
form.Controls.Add(box) 

// Compute the endpoint of a line
// starting at x, y, going at a certain angle
// for a certain length. 
let endpoint x y angle length =
    x + length * cos angle,
    y + length * sin angle

let flip x = (float)height - x

// Utility function: draw a line of given width, 
// starting from x, y
// going at a certain angle, for a certain length.
let drawLine (target : Graphics) (brush : Brush) 
             (x : float) (y : float) 
             (angle : float) (length : float) (width : float) =
    let x_end, y_end = endpoint x y angle length
    let origin = new PointF((single)x, (single)(y |> flip))
    let destination = new PointF((single)x_end, (single)(y_end |> flip))
    let pen = new Pen(brush, (single)width)
    target.DrawLine(pen, origin, destination)

let draw x y angle length width color = 
    drawLine graphics color x y angle length width

let pi = Math.PI

// Now... your turn to draw
// The trunk
// draw 250. 50. (pi*(0.5)) 100.0 4.
// let x, y = endpoint 250. 50. (pi*(0.5)) 100.
// first and second branches
// draw x y (pi*(0.5 + 0.3)) 50. 2.
// draw x y (pi*(0.5 - 0.4)) 50. 2.

// once the form is displayed, you are still able to use it
// via the FSI session. You just need to call form.Refresh()
// in order to display any pending graphics
form.Show()

// here is a handy function to clear the form
let clear() = 
    graphics.Clear background
    form.Refresh()




(* To do a nice fractal tree, using recursion is
probably a good idea. The following link might
come in handy if you have never used recursion in F#:
http://en.wikibooks.org/wiki/F_Sharp_Programming/Recursion
*)

let rec bifurcate x y angle width length depth =
    match depth with
    | 0 -> ()
    | _ ->
        let color = Color.FromArgb(128, 16 * depth, 16 * depth, 16 * depth)
        let colorBrush = new SolidBrush(color)
        draw x y angle length width colorBrush
        let startX, startY = endpoint x y angle length
        let nextDepth = depth - 1
        let nextLengthLeft = length * 0.8
        let nextLengthRight = length * 0.5
        let nextWidth = width * 0.8
        let nextAngleDiff = 0.3

        bifurcate startX startY (angle + nextAngleDiff) nextWidth nextLengthLeft nextDepth
        bifurcate startX startY (angle - nextAngleDiff) nextWidth nextLengthRight nextDepth

let callBifurcate angle =
    for a in [0.0 .. (pi / 6.0) .. 2.0 * pi] do
        bifurcate 512.0 512.0 (angle+(float a)) 8.0 100.0 10

let rotate = 
    async {
        for a in [1..1000] do
            do! Async.Sleep 10
            callBifurcate ((float a) * 0.01)
            form.Refresh()
            }
