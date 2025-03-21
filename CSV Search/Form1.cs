using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CSV_Search
{
    public partial class Form1 : Form
    {
        private string csvFilePath = Path.Combine(Application.StartupPath, "CSVFiles", "default.csv");
        private List<PersonRecord> records = new List<PersonRecord>();

        public Form1()
        {
            InitializeComponent();
            LoadCsvData();
            CustomizeButton();
            this.Paint += new PaintEventHandler(Form1_Paint);
            textBox1.BackColor = Color.White;
            textBox1.ForeColor = Color.Black;
            textBox1.Font = new Font("Arial", 12);

            textBox2.BackColor = Color.White; 
            textBox2.ForeColor = Color.Black;
            textBox2.Font = new Font("Arial", 12);

            label3.Paint += label1_Paint;

            foreach (Control control in this.Controls)
            {
                if (control is System.Windows.Forms.Label label)
                {
                    label.Click += Label_Click;
                }
            }

        }

        


        private void label1_Paint(object sender, PaintEventArgs e)
        {
            System.Windows.Forms.Label lbl = sender as System.Windows.Forms.Label;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = new GraphicsPath())
            {
                int borderRadius = 20;
                Rectangle rect = lbl.ClientRectangle;
                int diameter = borderRadius * 2;

                path.StartFigure();
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();

                lbl.Region = new Region(path);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int radius = 15; 
            int borderWidth = 2;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(Color.Gray, borderWidth)) 
            {
                
                using (GraphicsPath path1 = GetRoundedRectanglePath(textBox1.Bounds, radius))
                {
                    g.DrawPath(pen, path1);
                }

                
                using (GraphicsPath path2 = GetRoundedRectanglePath(textBox2.Bounds, radius))
                {
                    g.DrawPath(pen, path2);
                }
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
        private void CustomizeButton()
        {

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, button1.Width, button1.Height);
            button1.Region = new Region(path);


            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.BackColor = Color.White;


            string imagePath = System.IO.Path.Combine(Application.StartupPath, "search_icon.png");

            if (System.IO.File.Exists(imagePath))
            {
                Image originalImage = Image.FromFile(imagePath);
                Image resizedImage = new Bitmap(originalImage, new Size(20, 20));
                button1.Image = resizedImage;
                button1.ImageAlign = ContentAlignment.MiddleCenter;
            }
            else
            {
                MessageBox.Show("Search icon not found in Debug folder.");
            }
        }

        private void LoadCsvData()
        {
            records.Clear();

            if (!File.Exists(csvFilePath))
            {
                // Don't show error message on initial load, just return
                // MessageBox.Show("CSV file not found: " + csvFilePath);
                return;
            }

            try
            {
                var lines = File.ReadAllLines(csvFilePath);
                if (lines.Length <= 1)
                    return;

                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = ParseCsvLine(lines[i]);
                    if (fields.Length >= 4)
                    {
                        PersonRecord record = new PersonRecord
                        {
                            Name = fields[0].Trim(),
                            Phone = fields[1].Trim(),
                            Address = fields[2].Trim(),
                            Profession = fields[3].Trim()
                        };
                        records.Add(record);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading CSV file: " + ex.Message);
            }
        }

        private string[] ParseCsvLine(string line)
        {
            List<string> fields = new List<string>();
            bool inQuotes = false;
            StringBuilder field = new StringBuilder();
            foreach (char c in line)
            {
                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(field.ToString());
                    field.Clear();
                }
                else
                {
                    field.Append(c);
                }
            }

            fields.Add(field.ToString());
            return fields.ToArray();
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            if (!isLabel3Clicked)
            {
                MessageBox.Show("Click on כרטיס first!");
                return;
            }
            */
            string query = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Please enter a profession to search.");
                return;
            }

            var filteredRecords = records
                .Where(r => r.Profession.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (filteredRecords.Count == 0)
            {
                MessageBox.Show("No matching records found.");
                return;
            }

            StringBuilder resultText = new StringBuilder();
            foreach (var record in filteredRecords)
            {
                resultText.AppendLine($"Name: {record.Name}");
                resultText.AppendLine($"Phone: {record.Phone}");
                resultText.AppendLine($"Address: {record.Address}");
                resultText.AppendLine($"Profession: {record.Profession}");
                resultText.AppendLine(new string('-', 40));
            }

            textBox2.Text = resultText.ToString();
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            const string prefix = "        "; 

            if (!textBox1.Text.StartsWith(prefix))
            {
                textBox1.Text = prefix + textBox1.Text.TrimStart();
                textBox1.SelectionStart = prefix.Length;
            }
        }
        private void Label_Click(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.Label clickedLabel)
            {
                string fileName = $"{clickedLabel.Text}.csv";
                string directoryPath = Path.Combine(Application.StartupPath, "CSVFiles");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string filePath = Path.Combine(directoryPath, fileName);

                if (File.Exists(filePath))
                {
                    csvFilePath = filePath;

                    // Reset UI elements
                    records.Clear();
                    textBox1.Clear();
                    textBox2.Clear();

                    // Load the new CSV data
                    LoadCsvData();

                    // Update label styling
                    foreach (Control control in this.Controls)
                    {
                        if (control is System.Windows.Forms.Label lbl)
                        {
                            lbl.ForeColor = Color.Black;
                        }
                    }

                    // Highlight the selected label
                    clickedLabel.ForeColor = ColorTranslator.FromHtml("#0066CC");
                }
                else
                {
                    MessageBox.Show($"File not found: {fileName}\nLooking for file at: {filePath}",
                                   "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            
            label3.ForeColor = ColorTranslator.FromHtml("#0066CC"); 
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }
    }
    public class PersonRecord
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Profession { get; set; }
    }
}
