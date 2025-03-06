using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class RoundedTextBox : Panel
{
    private TextBox textBox;

    public RoundedTextBox()
    {
        // Set default size
        this.Size = new Size(200, 40);
        this.BackColor = Color.White;

        // Initialize TextBox
        textBox = new TextBox
        {
            BorderStyle = BorderStyle.None,
            Multiline = true,
            BackColor = Color.White,
            ForeColor = Color.Black,
            Font = new Font("Arial", 12),
            Location = new Point(10, 10),
            Width = this.Width - 20,
            Height = this.Height - 20
        };

        this.Controls.Add(textBox);
        this.Resize += new EventHandler(RoundedTextBox_Resize);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        // Create a rounded rectangle region
        using (GraphicsPath path = new GraphicsPath())
        {
            int radius = 15; // Corner radius
            int borderWidth = 2;

            // Create rounded rectangle
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();

            this.Region = new Region(path);

            using (Pen pen = new Pen(Color.Gray, borderWidth)) // Border color
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(pen, path);
            }
        }
    }

    private void RoundedTextBox_Resize(object sender, EventArgs e)
    {
        textBox.Width = this.Width - 20;
        textBox.Height = this.Height - 20;
    }
}
