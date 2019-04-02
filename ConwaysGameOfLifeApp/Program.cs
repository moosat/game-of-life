using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace ConwaysGameOfLifeApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var ui = new UniverseConsoleUi();
            //var mat = new bool[,]
            //{
            //    {true, true, true},
            //    {false, false, false},
            //    {false, false, false}
            //};
            var mat = InitMatrix(5000, 5000);
            var lifeMat = new Board(mat);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 10; i++)
            {
                lifeMat = lifeMat.GetNextGenBoard();

            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
                //ui.Draw(lifeMat.m_mat, 0);
                //lifeMat = lifeMat.GetNextGenBoard();
                Console.ReadKey();
                //Thread.Sleep(100);
            
        }

        private static bool[,] InitMatrix(int x, int y)
        {
            Random rand = new Random(1);
            var mat = new bool[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    mat[i, j] = rand.Next(100) > 50;
                }            
            }

            return mat;
        }
    }

}
