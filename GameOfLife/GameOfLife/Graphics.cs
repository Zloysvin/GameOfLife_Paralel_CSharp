using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public static class Graphics
    {
        private static Dictionary<bool, char> symbols = new() { { true, '#' }, { false, ' ' } };
        public static void Draw(bool[,] field, bool[,] newField)
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i,j] != newField[i,j])
                    {
                        Console.SetCursorPosition(j, i);
                        Console.Write(symbols[newField[i, j]]);
                    }
                }
            }
        }
        public static void Draw(bool[,] field)
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                        Console.Write(symbols[field[i, j]]);
                }
                Console.WriteLine();
            }
        }
    }
}
