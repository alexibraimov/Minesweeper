using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Minesweeper
{
    public class Map
    {
        public int[,] mapCell;
        public Size SizeMap { get; set; }
        private int numberOfMines;
        public int NumberOfMines
        {
            get
            {
                return numberOfMines;
            }
            set
            {
                int numberOfCells = SizeMap.Height * SizeMap.Width;
                if (value <= 0)
                    throw new ArgumentException("Количество мин должно быть больше 0");
                if (value >= numberOfCells)
                    throw new ArgumentException("Количество мин должно быть меньше " + numberOfCells);
                numberOfMines = value;
            }
        }
        public HashSet<Point> PositionMin { get; set; }
        public Dictionary<Point, HashSet<Point>> WayOfEmptyCells { get; set; }
        private HashSet<Point> EmptyCells;

        public Map(Size sizeMap, int numberOfMines)
        {
            WayOfEmptyCells = new Dictionary<Point, HashSet<Point>>();
            EmptyCells = new HashSet<Point>();
            SizeMap = sizeMap;
            NumberOfMines = numberOfMines;
            mapCell = new int[sizeMap.Width, sizeMap.Height];
            PositionMin = new HashSet<Point>();
        }

        public Map(string mapLine)
        {
            WayOfEmptyCells = new Dictionary<Point, HashSet<Point>>();
            EmptyCells = new HashSet<Point>();
            PositionMin = new HashSet<Point>();
            MakeMapFromString(mapLine);
            FindTheWayOfEmptyCells();
        }

        public void GenerateMap()
        {
            var rand = new Random();
            int count = 0;
            while (PositionMin.Count != NumberOfMines)
            {
                int x = rand.Next(0, SizeMap.Width - 1);
                int y = rand.Next(0, SizeMap.Height - 1);
                var nextPoint = new Point(x, y);
                if (!PositionMin.Contains(nextPoint))
                    PositionMin.Add(nextPoint); 
                else
                    count++;
                if (count > 100)
                    break;
            }
            NumberOfMines = PositionMin.Count;
            for (int x = 0; x < SizeMap.Width; x++)
                for (int y = 0; y < SizeMap.Height; y++)
                {
                    var point = new Point(x, y);
                    if (PositionMin.Contains(point))
                        mapCell[x, y] = 9;
                    else
                        mapCell[x, y] = NumberOfMinesAroundThePoint(point);

                    if (mapCell[x, y] == 0)
                        EmptyCells.Add(new Point(x, y));
                }
            FindTheWayOfEmptyCells();
        }

        public void MakeMapFromString(string mapLine)
        {
            if (mapLine == "" || mapLine == null)
            {
                SizeMap = new Size(0, 0);
                mapCell = new int[0, 0];
                numberOfMines = 0;
                return;
            }
            var lines = mapLine.Split('\n');
            SizeMap = new Size(lines[0].Length, lines.Length);
            mapCell = new int[SizeMap.Width, SizeMap.Height];
            for (int x = 0; x < SizeMap.Width; x++)
                for (int y = 0; y < SizeMap.Height; y++)
                {
                    if (lines[y][x] >= '0' && lines[y][x] <= '9')
                    {
                        mapCell[x, y] = Convert.ToInt32(lines[y][x]) - 48;
                        if (mapCell[x, y] == 0)
                            EmptyCells.Add(new Point(x, y));
                        if (mapCell[x, y] == 9)
                        {
                            numberOfMines++;
                            PositionMin.Add(new Point(x, y));
                        }
                    }
                    else
                        throw new ArgumentException("Неверный формат карты");
                }
        }

        private void FindTheWayOfEmptyCells()
        {
            foreach (var emptyPoint in EmptyCells)
            {
                var visited = new int[mapCell.GetLength(0), mapCell.GetLength(1)];
                var queue = new Queue<Point>();
                queue.Enqueue(emptyPoint);
                WayOfEmptyCells[emptyPoint] = new HashSet<Point>();
                while (queue.Count != 0)
                {
                    var point = queue.Dequeue();
                    if (point.X < 0 || point.X >= mapCell.GetLength(0) || point.Y < 0 || point.Y >= mapCell.GetLength(1))
                        continue;
                    if (visited[point.X, point.Y] == 1) continue;
                    if (PositionMin.Contains(point)) continue;
                    visited[point.X, point.Y] = 1;
                    WayOfEmptyCells[emptyPoint].Add(point);
                    if (mapCell[point.X, point.Y] == 0)
                    {
                        for (var dy = -1; dy <= 1; dy++)
                            for (var dx = -1; dx <= 1; dx++)
                                if (dx != 0 && dy != 0) continue;
                                else queue.Enqueue(new Point { X = point.X + dx, Y = point.Y + dy });
                    }
                }
            }
        }

        private bool IsOwnedMap(int x, int y)
        {
            return x < SizeMap.Width && y < SizeMap.Height
                && x >= 0 && y >= 0;
        }

        private int NumberOfMinesAroundThePoint(Point point)
        {
            int result = 0;
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                    var nextPoint = new Point(point.X + dx, point.Y + dy);
                    if (PositionMin.Contains(nextPoint))
                        result++;
                }
            return result;
        }
    }

    [TestFixture]
    public class MapTest
    {
        [TestCase("00000\n00000\n00000\n00000", 0)]
        [TestCase("1110000019\n1910000011\n2220001110\n1910001910\n2220002220\n1910001910", 6)]
        [TestCase("", 0)]
        [TestCase("0", 0)]
        [TestCase("9999999999", 10)]
        public void NumberOfMines_Test(string mapStr, int numberOfMines)
        {
            var map = new Map(mapStr);
            Assert.AreEqual(map.NumberOfMines, numberOfMines);
        }

        [TestCase("11119\n19111\n22111\n91119", 5, 4)]
        [TestCase("111\n111\n111\n111\n111\n111", 3, 6)]
        [TestCase("1111111111\n1111111111", 10, 2)]
        [TestCase("", 0, 0)]
        [TestCase(null, 0, 0)]
        public void Size_Test(string mapStr, int width, int height)
        {
            var map = new Map(mapStr);
            Assert.AreEqual(map.SizeMap.Width, width);
            Assert.AreEqual(map.SizeMap.Height, height);
        }
    }
}
