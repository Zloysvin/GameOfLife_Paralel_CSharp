using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    internal class TheGameOfLife
    {
        private static bool[,] field;
        private static bool[,] newField;

        private static bool[,] fieldCopy;

        private static float elapsedTimeBase = 0f;
        private static float elapsedTimeThread = 0f;

        private static bool visualizeMode = false;
        public static void Run(int threadNumber, int ticksAmount, int sizeY, int sizeX, bool onluMultithreadingSolution, bool visualize)
        {
            int dimY = sizeY;
            int dimX = sizeX;

            field = new bool[dimY, dimX];
            fieldCopy = new bool[dimY, dimX];

            visualizeMode = visualize;

            GenerateRandomStartingPositions();

            Console.WriteLine("Starting Conditions Generated");

            if(!onluMultithreadingSolution)
            {
                BaseSolution(dimY, dimX, ticksAmount);
            }


            field = fieldCopy;

            MultiThreadSolution(dimY, dimX, ticksAmount, threadNumber);

            if (!onluMultithreadingSolution)
            {
                Console.WriteLine($"Base Solution time: {elapsedTimeBase / ticksAmount}");
            }

            Console.WriteLine($"Multi Threading Solution time: {elapsedTimeThread / ticksAmount}");
        }

        private static int CalculateLiveCells(int x, int y)
        {
            int count = 0;
            for (int i = x - 1; i <= x + 1; i++) 
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if(i == x && j == y)
                        continue;

                    if (i >= 0 && j >= 0 && i < field.GetLength(0) && j < field.GetLength(1) && field[j, i])
                        count++;
                }
            }

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
            if(visualizeMode)
            {
                Console.Clear();
                Graphics.Draw(field);
                Console.ReadLine();
            }

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

                if(visualizeMode)
                {
                    Graphics.Draw(field, newField);
                    Console.ReadLine();
                }

                field = newField;

                if (!visualizeMode)
                    Console.WriteLine($"Time for tick {t} is {sw.ElapsedMilliseconds} ms");
            }
            Console.Clear();
        }        
        
        private static void MultiThreadSolution(int dimY, int dimX, int totalTicks, int threadNumber)
        {
            if (visualizeMode)
            {
                Console.Clear();
                Graphics.Draw(field);
                Console.ReadLine();
            }

            ArraySplitter ars = new ArraySplitter(dimY, dimX, threadNumber);

            for (int t = 0; t < totalTicks; t++)
            {
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                newField = new bool[dimY, dimX];

                Thread[] threads = new Thread[threadNumber];
                for (int i = threadNumber - 1; i >= 0; i--)
                {
                    var coordinates = ars.GetCoordinatesPair(i);
                    threads[i] = new Thread(() =>
                        SimulatePart(coordinates.x1, coordinates.x2, coordinates.y1, coordinates.y2));
                    threads[i].Start();
                }

                foreach (var thread in threads)
                {
                    thread.Join();
                }


                sw.Stop();
                elapsedTimeThread += sw.ElapsedMilliseconds;

                if (visualizeMode)
                {
                    Graphics.Draw(field, newField);
                    Console.ReadLine();
                }

                field = newField;

                if (!visualizeMode)
                    Console.WriteLine($"Time for tick {t} is {sw.ElapsedMilliseconds} ms");
            }
            Console.Clear();
        }
    }
}
