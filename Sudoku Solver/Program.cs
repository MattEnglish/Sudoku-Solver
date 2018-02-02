using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sudoku_Solver
{
    class Program
    {
        static void Main(string[] args)
        {
            var sudoku = new Sudoku();
            var strings = File.ReadAllLines("SudokuMap.csv");
            for (int i = 0; i < strings.Length; i++)
            {
                var nums= strings[i].Split(',');
                for (int j = 0; j < 9; j++)
                {
                    int? a = int.Parse(nums[j]);
                    sudoku.numbers[i, j] = a == 0 ? null : a;
                }

            }
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var s = Searching.Search(sudoku);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            PrintSudoku(s);

        }

        static void PrintSudoku(Sudoku s)
        {
            Console.WriteLine();
            Console.Write("¦");
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(s.numbers[i,j]);
                    if (j % 3 == 2)
                    {
                        Console.Write("¦");
                    }
                }
                if (i % 3 == 2)
                {
                    Console.Write("\n-------------");
                }
                Console.Write("\n¦");
            }
        }
    }

    public static class Searching
    {
        public static Sudoku Search(Sudoku s)
        {
            var fewestPossVec = GetSmallestPoss(s);
            var possNums = s.GetAllPossNum(fewestPossVec);
            if (possNums.Count == 0)
            {
                if (s.IsSudukoComplete())
                {
                    return s;
                }
                return null;
            }

            foreach (var num in possNums)
            {
                var newSudoku = s.createCopy();
                newSudoku.numbers[fewestPossVec.x, fewestPossVec.y] = num;
                var finSudoku = Search(newSudoku);
                if (finSudoku != null)
                {
                    return finSudoku;
                }
            }
            return null;

        }

        public static TwoVector GetNextEmpty(Sudoku s)
        {
            int smallest = 10;
            var smallestVec = new TwoVector();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var possNums = s.GetAllPossNum(new TwoVector() { x = i, y = j }).Count;
                    if ( possNums != 0)
                    {
                        smallest = s.GetAllPossNum(new TwoVector() { x = i, y = j }).Count;
                        smallestVec.x = i;
                        smallestVec.y = j;
                    }
                }
            }
            return smallestVec;
        }

        public static TwoVector GetSmallestPoss(Sudoku s)
        {
            int smallest = 10;
            var smallestVec = new TwoVector();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var possNums = s.GetAllPossNum(new TwoVector() {x = i, y = j}).Count;
                    if ( possNums < smallest && possNums != 0)
                    {
                        smallest = s.GetAllPossNum(new TwoVector() { x = i, y = j }).Count;
                        smallestVec.x = i;
                        smallestVec.y = j;
                    }
                }
            }
            return smallestVec;
        }

    }


    public class Sudoku
    {
        public int?[,] numbers = new int?[9, 9];

        public Sudoku createCopy()
        {
            var newArray = new int?[9, 9];
            Array.Copy(numbers,newArray,81);

            return new Sudoku()
            {
                numbers = newArray
            };
        }

        public bool IsSudukoComplete()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (numbers[i,j] == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public List<int> GetAllPossNum(TwoVector v)
        {
            if (numbers[v.x, v.y] != null)
            {
                return new List<int>();
            }
            var bannedNum = GetNumbersInRow(v);
            bannedNum.AddRange(GetNumbersInCol(v));
            bannedNum.AddRange(GetNumbersInSubSquare(v));
            var possNums = new List<int>();
            for (int i = 1; i < 10; i++)
            {
                if (!bannedNum.Contains(i))
                {
                    possNums.Add(i);
                }
            }
            return possNums;

        }

        private List<int?> GetNumbersInRow(TwoVector v)
        {
            var rowNumbers = new List<int?>();
            for (int i = 0; i < 9; i++)
            {
                if (numbers[i, v.y] != null)
                {
                    rowNumbers.Add(numbers[i,v.y]);
                }
            }
            return rowNumbers;
        }

        List<int?> GetNumbersInCol(TwoVector v)
        {
            var rowNumbers = new List<int?>();
            for (int i = 0; i < 9; i++)
            {
                if (numbers[v.x, i] != null)
                {
                    rowNumbers.Add(numbers[v.x, i]);
                }
            }
            return rowNumbers;
        }

        public SubSquare GetSubSquare(TwoVector v)
        {
            return new SubSquare()
            {
                x = v.x/3,
                y = v.y/3
            };
        }

        List<int?> GetNumbersInSubSquare(TwoVector v)
        {
            return GetNumbersInSubSquare(GetSubSquare(v));
        }

        List<int?> GetNumbersInSubSquare(SubSquare s)
        {
            var sqrNumber = new List<int?>();
            for (int i = s.x*3; i < s.x*3 + 3; i++)
            {
                for (int j = s.y*3; j < s.y*3 + 3; j++)
                {
                    if (numbers[i, j] != null)
                    {
                        sqrNumber.Add(numbers[i, j]);
                    }
                }
            }
            return sqrNumber;
        }
    }

    public class SubSquare
    {
        public int x;
        public int y;
    }

    public class TwoVector
    {
        public int x;
        public int y;
    }
}
