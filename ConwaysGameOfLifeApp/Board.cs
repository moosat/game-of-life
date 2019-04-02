using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConwaysGameOfLifeApp
{
    public struct Position
    {
        public int X;
        public int Y;
    }

    public class Cell
    {
        private readonly Board2 m_board;

        public Cell(Board2 board)
        {
            m_board = board;
        }

        public Position Pos;

        public IEnumerable<Position> GetDeadBrothersPosition()
        {
            foreach (var position in GetBrothersPosition())
            {
                if (!m_board.LiveCells.ContainsKey(position))
                    yield return position;

            }
        }

        public IEnumerable<Cell> GetDeadBrothers()
        {
            foreach (var position in GetBrothersPosition())
            {
                if (!m_board.LiveCells.ContainsKey(position))
                    yield return new Cell(m_board)
                    {
                        Pos = position
                    }; 
            }
        }


        public int GetNumberOfLivingBrothers()
        {
            return GetBrothersPosition().Count(position => m_board.LiveCells.ContainsKey(position));
        }

        public IEnumerable<Position> GetBrothersPosition()
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if ( (i==0 && j==0 ) ||
                         Pos.X + i < 0 || Pos.Y + j < 0 ||
                         Pos.X + i >= m_board.DimX ||
                         Pos.Y + j >= m_board.DimY)
                        continue;
                    yield return new Position() {X = Pos.X + i, Y = Pos.Y + j};
                }
            }
        }
    }

    public class Board2
    {
        public Dictionary<Position, Cell> LiveCells;
        public readonly int DimX;
        public readonly int DimY;

        public Board2(Dictionary<Position, Cell> initLiveCells, int dimX, int dimY)
        {
            LiveCells = initLiveCells;
            DimX = dimX;
            DimY = dimY;
        }

        public Board2 GetNextBoard2()
        {
            var live2gen = new Dictionary<Position, Cell>();
            foreach (var liveCell in LiveCells.Values)
            {
                var n = liveCell.GetNumberOfLivingBrothers();
                if (n == 3 || n == 2)
                {
                    live2gen[liveCell.Pos] = liveCell;
                }

                foreach (var newCell in liveCell.GetDeadBrothers())
                {
                    var nd = newCell.GetNumberOfLivingBrothers();
                    if (nd == 3)
                        live2gen[newCell.Pos] = newCell;
                }
            }
            return new Board2(live2gen, this.DimX, this.DimY);
        }
    }

    public class Board
    {
        public readonly bool[,] m_mat;

        public Board(bool[,] mat)
        {
            m_mat = mat;
        }
        private int GetNumOfNeibInside(int x, int y)
        {
            
            int n=0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if  (i==0 && j==0 ) 
                        continue;
                    n += m_mat[x + i, y + j] ? 1 : 0;
                }
            }

            return n;
        }

        private int GetNumOfNeib(int x, int y)
        {
            
            int n=0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if ( (i==0 && j==0 ) ||
                        x+i < 0 || y+j < 0 ||
                        x+i >= m_mat.GetLength(0) ||
                        y+j >= m_mat.GetLength(1))
                        continue;
                    n += m_mat[x + i, y + j] ? 1 : 0;
                }
            }

            return n;
        }

        private bool GetNextCellGenInside(int x, int y)
        {
            //  return true;
            int n = GetNumOfNeibInside(x, y);
            if (m_mat[x, y])
                return n == 2 || n == 3;

            return n == 3;
        }

        public Board GetNextGenBoard()
        {       
            var nextBoard = new bool[m_mat.GetLength(0),
                m_mat.GetLength(1)];
            Parallel.For(0, 1, border =>
            {
                if (border == 1)
                {
                    FillBorder(nextBoard);
                }
                else
                {
                    Parallel.For(1, m_mat.GetLength(0)-1,  i =>
                    {
                        Parallel.For(1, m_mat.GetLength(1)-1, j =>
                        {
                            nextBoard[i, j] = GetNextCellGenInside(i, j);
                        });
                    });
                }

            });
            

            //for (int i = 0; i < m_mat.GetLength(0); i++)
            //{
            //    for (int j = 0; j < m_mat.GetLength(1); j++)
            //    {
            //        nextBoard[i, j] = GetNextCellGen(i, j);
            //    }
            //}
            return new Board(nextBoard);
        }

        private void FillBorder(bool[,] nextBoard)
        {
            
        }
    }
}
