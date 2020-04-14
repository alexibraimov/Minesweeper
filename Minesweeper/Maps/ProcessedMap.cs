using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Minesweeper
{
    public class ProcessedMap
    {
        public readonly Dictionary<string, string> WayToTheImages = new Dictionary<string, string>();
        public Dictionary<Point, string> ImageNames = new Dictionary<Point, string>();
        public readonly Size SizeMap;
        public readonly int NumberOfMines;
        public readonly int Width;
        public readonly int Height;
        public readonly HashSet<Point> PositionMin = new HashSet<Point>();
        public readonly Dictionary<Point, HashSet<Point>> WayOfEmptyCells = new Dictionary<Point, HashSet<Point>>();

        public ProcessedMap(Map map, DirectoryInfo imagesDirectory = null)
        {
            if (imagesDirectory == null)
                imagesDirectory = new DirectoryInfo("Images");
            foreach (var e in imagesDirectory.GetFiles("*.png"))
                WayToTheImages[e.Name] = e.FullName;
            InitializesImageNames(map.mapCell);
            SizeMap = map.SizeMap;
            NumberOfMines = map.NumberOfMines;
            Width = map.SizeMap.Width;
            Height = map.SizeMap.Height;
            PositionMin = map.PositionMin;
            WayOfEmptyCells = map.WayOfEmptyCells;
        }

        private void InitializesImageNames(int[,] mapCell)
        {
            int rowLength = mapCell.GetLength(0);
            int columnLength = mapCell.GetLength(1);

            for (int x = 0; x < rowLength; x++)
                for (int y = 0; y < columnLength; y++)
                {
                    var tempStr = mapCell[x, y].ToString() + ".png";
                    ImageNames.Add(new Point(x, y), tempStr);
                }
        }
    }
}
