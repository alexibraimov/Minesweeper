using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public class GameForm : Form
    {
        private TableLayoutPanel BigTable { get; set; }
        private TableLayoutPanel SmallTable { get; set; }
        private Label SmallLabel { get; set; }
        private ProcessedMap State { get; set; }
        private int NumberBomb { get; set; }
        private Dictionary<Point, string> StatusIndicator { get; set; }
        private int CorrectFlags { get; set; }

        public GameForm(ProcessedMap map)
        {
            this.State = map;
            this.Text = "Сапёр";
            this.Size = new Size(State.Width * 50 + 28, State.Height * 50 + 60 + 48);
            NumberBomb = map.NumberOfMines;
            CorrectFlags = map.NumberOfMines;
            SmallLabel = new Label { Text = NumberBomb.ToString(), Dock = DockStyle.Bottom };
            BigTable = new TableLayoutPanel { Dock = DockStyle.Fill };
            SmallTable = new TableLayoutPanel { Dock = DockStyle.Fill };
            StatusIndicator = new Dictionary<Point, string>();

            var bigMenu = new MenuStrip();
            var menu = new ToolStripMenuItem("Игра");
            var smallMenu1 = new ToolStripMenuItem("Новая игра");
            var smallMenu2 = new ToolStripMenuItem("Выйти");
            smallMenu1.Click += (sender, args) => Application.Restart();
            smallMenu2.Click += (sender, args) => this.Close();

            menu.DropDownItems.Add(smallMenu1);
            menu.DropDownItems.Add(smallMenu2);
            bigMenu.Items.Add(menu);

            BigTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            BigTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            BigTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            BigTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            for (int x = 0; x < map.Width; x++)
                for (int y = 0; y < map.Height; y++)
                {
                    SmallTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
                    SmallTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
                    var button = new ButtonExtended
                    {
                        TablePoint = new Point(x, y),
                        MinimumSize = new Size(50, 50),
                        Image = Image.FromFile(State.WayToTheImages["closed.png"]),
                        Enabled = true
                    };
                    StatusIndicator.Add(new Point(x, y), "closed.png");

                    button.MouseEnter += (sender, args) =>
                    {
                        if (StatusIndicator[button.TablePoint] == "closed.png")
                            ChangeButton(button, true, true, "mouseclosed.png");
                        if (StatusIndicator[button.TablePoint] == "inform.png")
                            ChangeButton(button, true, true, "mouseinform.png");
                        if (StatusIndicator[button.TablePoint] == "flaged.png")
                            ChangeButton(button, true, false, "mouseflaged.png");
                    };
                    button.MouseLeave += (sender, args) =>
                    {
                        if (StatusIndicator[button.TablePoint] == "mouseclosed.png")
                            ChangeButton(button, true, true, "closed.png");
                        if (StatusIndicator[button.TablePoint] == "mouseinform.png")
                            ChangeButton(button, true, true, "inform.png");
                        if (StatusIndicator[button.TablePoint] == "mouseflaged.png")
                            ChangeButton(button, true, false, "flaged.png");
                    };
                    button.MouseClick += (sender, args) =>
                    {
                        if (args.Button == MouseButtons.Left && button.LeftEnabled)
                            FieldLeftClick(sender, args);
                        if (args.Button == MouseButtons.Right)
                            FieldRightClick(sender, args);
                        if (CorrectFlags == 0 && SmallLabel.Text == "0")
                            Victory();
                    };

                    SmallTable.Controls.Add(button, x, y);
                }

            Controls.Add(bigMenu);
            BigTable.Controls.Add(SmallTable, 0, 1);
            BigTable.Controls.Add(SmallLabel, 0, 2);
            Controls.Add(BigTable);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        void FieldLeftClick(object sender, EventArgs args)
        {
            var button = (ButtonExtended)sender;
            Point tablePoint = button.TablePoint;
            ChangeButton(button, false, false);
            if (State.ImageNames[tablePoint] == "9.png")
                GameOver(tablePoint);
            if (State.ImageNames[tablePoint] == "0.png")
                OpenEmptyCells(tablePoint);
        }

        void GameOver(Point point)
        {
            var buttons = new List<ButtonExtended>();
            foreach (var item in SmallTable.Controls)
            {
                var button = (ButtonExtended)item;
                var name = StatusIndicator[button.TablePoint] == "flaged.png";
                if (State.PositionMin.Contains(button.TablePoint)
                    && point != button.TablePoint)
                    ChangeButton(button, false, false, "bomb.png");
                if (name && !State.PositionMin.Contains(button.TablePoint))
                    ChangeButton(button, false, false, "nobomb.png");
            }
            MessageBox.Show("Игра закончена!\nВы проиграли!", "Game over", MessageBoxButtons.OK);
            SmallTable.Enabled = false;
        }

        void Victory()
        {
            MessageBox.Show("Игра закончена!\nВы выиграли!", "Victory!", MessageBoxButtons.OK);
            SmallTable.Enabled = false;
        }

        void OpenEmptyCells(Point point)
        {
            var buttons = new List<ButtonExtended>();
            foreach (var item in SmallTable.Controls)
            {
                var button = (ButtonExtended)item;
                if (State.WayOfEmptyCells[point].Contains(button.TablePoint)
                    && StatusIndicator[button.TablePoint] != "flaged.png")
                    buttons.Add(button);
            }
            foreach (var button in buttons)
                ChangeButton(button, false, false);
        }

        void FieldRightClick(object sender, EventArgs args)
        {
            var button = (ButtonExtended)sender;
            Point point = button.TablePoint;
            if (StatusIndicator[point] == "mouseclosed.png")
            {
                ChangeButton(button, true, false, "mouseflaged.png");
                NumberBomb--;
                if (State.PositionMin.Contains(point))
                    CorrectFlags--;
                SmallLabel.Text = NumberBomb.ToString();
            }
            else if (StatusIndicator[point] == "mouseflaged.png")
            {
                ChangeButton(button, true, true, "mouseinform.png");
                NumberBomb++;
                if (State.PositionMin.Contains(point))
                    CorrectFlags--;
                SmallLabel.Text = NumberBomb.ToString();
            }
            else if (StatusIndicator[point] == "mouseinform.png")
                ChangeButton(button, true, true, "mouseclosed.png");
        }

        void ChangeButton(ButtonExtended button, bool enabled, bool leftEnabled, string imageName = null)
        {
            if (imageName == null)
                imageName = State.ImageNames[button.TablePoint];
            button.Enabled = enabled;
            button.Image = Image.FromFile(State.WayToTheImages[imageName]);
            button.LeftEnabled = leftEnabled;
            StatusIndicator[button.TablePoint] = imageName;
        }
    }

    public class ButtonExtended : PictureBox
    {
        public Point TablePoint;
        public bool LeftEnabled = true;
    }
}
