using System.Drawing;
using System.Windows.Forms;

namespace GameOfLifeVisualized
{
    public partial class Form1 : Form
    {
        private PictureBox pictureBox;
        private int width;
        private int height;

        private Bitmap bitmap;

        public Form1(int width, int height)
        {
            InitializeComponent();
            this.width = width;
            this.height = height;

            this.Size = new Size(width, height);

            // Initialize PictureBox
            pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            this.Controls.Add(pictureBox);

            bitmap = new Bitmap(width, height);
            pictureBox.Image = bitmap;
        }

        public void UpdateBitmap(bool[,] boolArray)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int colorValue = boolArray[x, y] ? 0 : 255; // Black or white
                    Color color = Color.FromArgb(colorValue, colorValue, colorValue);
                    bitmap.SetPixel(x, y, color);
                }
            }
            pictureBox.Image = bitmap;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TheGameOfLife.Run(this);
        }
    }

    internal class TheGameOfLife
    {
        private static bool[,] field;
        private static bool[,] newField;

        private static bool[,] fieldCopy;

        private static float elapsedTimeBase = 0f;
        private static float elapsedTimeThread = 0f;

        private static Form1 originForm;
        public static void Run(Form1 form)
        {
            int dimY = 2000;
            int dimX = 2000;

            originForm = form;

            field = new bool[dimY, dimX];
            fieldCopy = new bool[dimY, dimX];

            GenerateRandomStartingPositions();

            int totalTicks = 10;

            MultiThreadSolution(dimY, dimX, totalTicks);

            field = fieldCopy;

            BaseSolution(dimY, dimX, totalTicks);
        }

        private static int CalculateLiveCells(int x, int y)
        {

            int count = 0;

            for (int i = x - 1; i < x + 1; i++)
            {
                for (int j = y - 1; j < y + 1; j++)
                {
                    if (i == x && j == y)
                        continue;

                    if (i >= 0 && j >= 0 && i < field.GetLength(0) && j < field.GetLength(1) && field[j, i])
                        count++;
                }
            }

            //if (x - 1 >= 0 && field[y, x - 1]) // left
            //{
            //    count++;
            //}

            //if (y - 1 >= 0 && field[y - 1, x]) // top
            //{
            //    count++;

            //}

            //if (x + 1 < field.GetLength(0) && field[y, x + 1]) // right
            //{
            //    count++;
            //}

            //if (y + 1 < field.GetLength(1) && field[y + 1, x]) // bottom
            //{
            //    count++;
            //}

            //if (x + 1 < field.GetLength(0) && y - 1 >= 0 && field[y - 1, x + 1])
            //{
            //    count++;
            //}

            //if (x + 1 < field.GetLength(0) && y + 1 < field.GetLength(1) && field[y + 1, x + 1])
            //{
            //    count++;
            //}

            //if (x - 1 >= 0 && y - 1 >= 0 && field[y - 1, x - 1])
            //{
            //    count++;
            //}

            //if (x - 1 >= 0 && y + 1 < field.GetLength(1) && field[y + 1, x - 1])
            //{
            //    count++;
            //}

            return count;
        }

        private static void GenerateRandomStartingPositions()
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (Random.Shared.Next(100) >= 70)
                    {
                        field[i, j] = true;
                        fieldCopy[i, j] = true;
                    }
                }
            }
        }

        private static void SimulatePart(int xStart, int xFinish, int yStart, int yFinish)
        {
            for (int i = yStart; i < yFinish; i++)
            {
                for (int j = xStart; j < xFinish; j++)
                {
                    int neighbors = CalculateLiveCells(j, i);
                    if (!field[i, j] && neighbors == 3)
                    {
                        newField[i, j] = true;
                    }
                    else if (field[i, j] && neighbors is 2 or 3)
                    {
                        newField[i, j] = true;
                    }
                    else
                    {
                        newField[i, j] = false;
                    }
                }
            }
        }

        private static void BaseSolution(int dimY, int dimX, int totalTicks)
        {
            for (int t = 0; t < totalTicks; t++)
            {
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                newField = new bool[dimY, dimX];
                for (int i = 0; i < dimY; i++)
                {
                    for (int j = 0; j < dimX; j++)
                    {
                        int neighbors = CalculateLiveCells(j, i);
                        if (!field[i, j] && neighbors == 3)
                        {
                            newField[i, j] = true;
                        }
                        else if (field[i, j] && neighbors is 2 or 3)
                        {
                            newField[i, j] = true;
                        }
                        else
                        {
                            newField[i, j] = false;
                        }
                    }
                }
                sw.Stop();
                elapsedTimeBase += sw.ElapsedMilliseconds;

                field = newField;
            }
        }

        private static void MultiThreadSolution(int dimY, int dimX, int totalTicks)
        {
            for (int t = 0; t < totalTicks; t++)
            {
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                newField = new bool[dimY, dimX];

                Thread[] threads = new Thread[4];
                int threadCount = 0;

                for (int i = 0; i < dimY / 1000; i++)
                {
                    for (int j = 0; j < dimX / 1000; j++)
                    {
                        var j1 = j;
                        var i1 = i;
                        threads[threadCount] = new Thread(() =>
                            SimulatePart(j1 * 1000, (j1 + 1) * 1000 - 1, i1 * 1000, (i1 + 1) * 1000 - 1));
                        threads[threadCount].Start();
                        threadCount++;
                    }
                }

                foreach (var thread in threads)
                {
                    thread.Join();
                }


                sw.Stop();
                elapsedTimeThread += sw.ElapsedMilliseconds;

                field = newField;
                originForm.UpdateBitmap(field);

                Thread.Sleep(200);
            }
        }
    }
}
