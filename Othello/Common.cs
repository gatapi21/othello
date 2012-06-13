using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI;

namespace Othello
{
    internal static class Common
    {
        public const int UNDEFINED = 0;
        public const int BLACK = -1;
        public const int WHITE = 1;

        private static Direction[] directions = new Direction[]
        { 
            new Direction { X = 1, Y = 0 },
            new Direction { X = 1, Y = -1},
            new Direction { X = 0, Y = -1},
            new Direction { X = -1, Y = -1},
            new Direction { X = -1, Y = 0},
            new Direction { X = -1, Y = 1},
            new Direction { X = 0, Y = 1},
            new Direction { X = 1, Y = 1}
        };

        public static Color IntToColor(int color)
        {
            if (color == Common.BLACK) { return Colors.Black; }
            else if (color == Common.WHITE) { return Colors.White; }
            else 
            {
                return Colors.Red;
            }
        }        

        // this method is used to simulate a move 
        public static bool TryPlay(int[][] cells, Move move)
        {
            int row = move.Row;
            int col = move.Col;
            int color = move.Color;
            Debug.Assert(color == Common.BLACK || color == Common.WHITE);
            bool isValidMove = false;
            if (cells[row][col] != Common.UNDEFINED)
            {
                return false;       // cell is already occupied
            }
            foreach (var position in move.PositionsToFlip)
            {               
                Debug.Assert(cells[position.Row][position.Col] == color);
                cells[position.Row][position.Col] = -cells[position.Row][position.Col];
                isValidMove = true;
            }
            if (isValidMove)
            {
                cells[row][col] = color;
            }
            return isValidMove;
        }
      
        public static List<Position> GetPositionsToFlip(int row, int col, int color, int[][] cells)
        {
            var answer = new List<Position>();
            var opposite = -color;
            if (cells[row][col] == Common.UNDEFINED)
            {
                // test each direction	
                for (int k = 0; k < directions.Length; k++)
                {
                    var d = directions[k];
                    var l = row + d.Y;
                    var m = col + d.X;
                    var temp = new List<Position>();
                    while (l >= 0 && l < Board.ROWS && m >= 0 && m < Board.COLS && cells[l][m] == opposite)
                    {
                        temp.Add(new Position { Row = l, Col = m });
                        l += d.Y;
                        m += d.X;
                    }
                    if (l >= 0 && l < Board.ROWS && m >= 0 && m < Board.COLS && cells[l][m] == color)
                    {
                        answer.AddRange(temp);
                    }
                }
            }
            return answer;
        }
    }
}
