using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Othello
{
    internal static class Utility
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
        
        public static void SetView(UIElement landscape, UIElement portrait, UIElement filled, UIElement snapped)
        {            
            switch (ApplicationView.Value)
            {
                case ApplicationViewState.Filled:
                    landscape.Visibility = Visibility.Collapsed;
                    portrait.Visibility = Visibility.Collapsed;
                    snapped.Visibility = Visibility.Collapsed;
                    filled.Visibility = Visibility.Visible;
                    break;
                case ApplicationViewState.FullScreenLandscape:
                    portrait.Visibility = Visibility.Collapsed;
                    snapped.Visibility = Visibility.Collapsed;
                    filled.Visibility = Visibility.Collapsed;
                    landscape.Visibility = Visibility.Visible;
                    break;
                case ApplicationViewState.FullScreenPortrait:
                    landscape.Visibility = Visibility.Collapsed;
                    snapped.Visibility = Visibility.Collapsed;
                    filled.Visibility = Visibility.Collapsed;
                    portrait.Visibility = Visibility.Visible;
                    break;
                case ApplicationViewState.Snapped:
                    portrait.Visibility = Visibility.Collapsed;
                    landscape.Visibility = Visibility.Collapsed;
                    filled.Visibility = Visibility.Collapsed;
                    snapped.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        public static Color IntToColor(int color)
        {
            if (color == Utility.BLACK) { return Colors.Black; }
            else if (color == Utility.WHITE) { return Colors.White; }
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
            Debug.Assert(color == Utility.BLACK || color == Utility.WHITE);
            bool isValidMove = false;
            if (move.PositionsToFlip.Count > 0)
            {
                Debug.Assert(cells[row][col] == Utility.UNDEFINED);
                isValidMove = true;
                foreach (var position in move.PositionsToFlip)
                {
                    Debug.Assert(cells[position.Row][position.Col] == -color);
                    cells[position.Row][position.Col] = color;                    
                }
                cells[row][col] = color;                
            }
            return isValidMove;
        }
      
        public static List<Position> GetPositionsToFlip(int row, int col, int color, int[][] cells)
        {
            var answer = new List<Position>();
            var opposite = -color;
            if (cells[row][col] == Utility.UNDEFINED)
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

        public static int[][] Clone(int[][] x)
        {
            var y = new int[x.Length][];
            for (int i = 0; i < x.Length; i++)
            {
                y[i] = new int[x[i].Length];
                for (int j = 0; j < x[i].Length; j++)
                {
                    y[i][j] = x[i][j];
                }
            }
            return y;
        }

        public static void Assert(bool expression)
        {
            if (!expression)
            {
                Debugger.Break();
            }
        }
    }
}
