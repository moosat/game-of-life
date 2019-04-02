using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ConwaysGameOfLifeApp.Tests
{
    public class XnunitTest
    {
        private readonly ITestOutputHelper  m_logger;

        public XnunitTest(ITestOutputHelper  logger)
        {
            m_logger = logger;
        }

        [Fact]
        public void Get_NumOf_Neighobur3()
        {

            //var mat = new bool[,]
            //{
            //    {true, true, true },
            //    {false, false, false },
            //    {false, false, false },
            //};
            //var board = new MatLife.Board(mat);
            //var nextGenBoard = MatLife.GetNextGeneration(board);
            //Assert.True(nextGenBoard.CellLive(1,1));

        }

        //[Theory]
        //[InlineData(1,1, true)]
        //[InlineData(1,2, true)]
        //[InlineData(1,1, true)]
        //[InlineData(1,1, true)]
        //[InlineData(1,1, true)]
        //public void Get_NumOf_Neighobur3(int x, int y, bool expected)
        //{
        //    var mat = new bool[,]
        //    {
        //        {true, true, true },
        //        {false, false, false },
        //        {false, false, false },
        //    };
        //    var board = new Board(mat);
        //    int x = board.GetNumOfLivingNeigbours(1, 1);
        //    Assert.True(x == 3);
        //}

        private static bool[,] InitMatrix(int x, int y, double percentFill)
        {
            Random rand = new Random(1);
            var mat = new bool[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    mat[i, j] = rand.NextDouble() < percentFill;
                }            
            }

            return mat;
        }

        [Fact]     
        public void PerformanceTest()
        {
            var mat = InitMatrix(5000, 5000, 0.01);
            var lifeMat = new Board(mat);
            for (int i = 0; i < 10; i++)
            {
                lifeMat = lifeMat.GetNextGenBoard();

            }
        }

        [Fact]
        public void EqualOfPosition()
        {
            var pos1 = new Position(){X=3,Y=6};
            var pos2 = new Position(){X=3,Y=6};
            Assert.True(pos1.Equals(pos2));
        }

        [Fact]
        public void NotEqualOfPosition()
        {
            var pos1 = new Position(){X=3,Y=6};
            var pos2 = new Position(){X=2,Y=6};
            Assert.False(pos1.Equals(pos2));
        }

        [Fact]
        public void PerformanceTestBoard2JustInit()
        {
            var mat = InitMatrix(5000, 5000, 0.1);
            var lifeMat = CrreateInitBoard2(mat);
            Assert.True(lifeMat.DimX == 5_000);
            Assert.True(mat.GetLength(0) == 5000);
        }

        [Fact]     
        public void PerformanceTestBoard2()
        {
            var mat = InitMatrix(5000, 5000, 0.01);
            var lifeMat = CrreateInitBoard2(mat);
            for (int i = 0; i < 10; i++)
            {
                lifeMat = lifeMat.GetNextBoard2();
            }
        }


        [Fact]
        public void NextGenCell_With3N_LiveB2()
        {
            var mat = new bool[,]
            {
                {true, true, true },
                {false, false, false },
                {false, false, false },
            };

            var board2 = CrreateInitBoard2(mat);

            var board2Gen2 = board2.GetNextBoard2();
            
            Assert.True(board2Gen2.LiveCells.ContainsKey(new Position() {X=1, Y=1}));
        }

        private static Board2 CrreateInitBoard2(bool[,] mat)
        {
            int dimX = mat.GetLength(0);
            int dimY = mat.GetLength(1);
            Dictionary<Position, Cell> initB = new Dictionary<Position, Cell>(dimX*dimY);

            var board2 = new Board2(initB, dimX, dimY);
            for (int i = 0; i < dimX; i++)
            {
                for (int j = 0; j < dimY; j++)
                {
                    if (mat[i, j])
                    {
                        var cell = new Cell(board2) {Pos = new Position() {X = i, Y = j}};
                        initB[cell.Pos] = cell;
                    }
                }
            }

            return board2;
        }


        [Fact]
        public void NextGenCell_With3N_Live()
        {
            var mat = new bool[,]
            {
                {true, true, true },
                {false, false, false },
                {false, false, false },
            };
            var board = new Board(mat);
            var x = board.GetNextGenBoard();
            Assert.True(x.m_mat[1,1]);
        }
    }
}
