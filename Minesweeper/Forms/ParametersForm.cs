using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public class ParametersForm : Form
    {
        public Size SizeMap { get; set; }
        public bool Ok { get; private set; }
        public ParametersForm()
        {
            this.Size = new Size(300, 180);
            this.Text = "Параметры игры \"Сапёр\"";
            var label1 = new Label
            {
                Text = "Длина поля:",
                Font = new Font("Arial", 9f),
                Size = new Size(125, 30),
                Location = new Point(10, 10)
            };
            var label2 = new Label
            {
                Text = "Ширина поля:",
                Font = new Font("Arial", 9f),
                Size = new Size(125, 30),
                Location = new Point(10, label1.Bottom)
            };
            var label3 = new Label
            {
                Text = "Количество бомб:",
                Font = new Font("Arial", 9f),
                Size = new Size(125, 30),
                Location = new Point(10, label2.Bottom)
            };
            var width = new NumericUpDown()
            {
                Minimum = 4,
                Maximum = 14,
                Value = 10,
                Location = new Point(label1.Right, 10)
            };
            var height = new NumericUpDown()
            {
                Minimum = 4,
                Maximum = 14,
                Value = 10,
                Location = new Point(label2.Right, label1.Bottom)
            };
            var bombs = new NumericUpDown()
            {
                Minimum = 2,
                Maximum = width.Value > height.Value ? (int)height.Value : (int)width.Value,
                Value = width.Value > height.Value ? (int)height.Value : (int)width.Value,
                Location = new Point(label3.Right, label2.Bottom)
            };
            height.ValueChanged += (sender, args) => bombs.Value = bombs.Maximum = width.Value > height.Value ? (int)width.Value : (int)height.Value;
            width.ValueChanged += (sender, args) => bombs.Value = bombs.Maximum = width.Value > height.Value ? (int)width.Value : (int)height.Value;
            var buttonOk = new Button
            {
                Text = "Принять",
                Location = new Point(10, label3.Bottom)
            };
            buttonOk.Click += (sender, args) =>
            {
                SizeMap = new Size((int)width.Value, (int)height.Value);
                Ok = true;
                Close();
            };
            Controls.Add(label1);
            Controls.Add(width);
            Controls.Add(label2);
            Controls.Add(height);
            Controls.Add(label3);
            Controls.Add(buttonOk);
            Controls.Add(bombs);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }
    }
}
