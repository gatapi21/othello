using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Othello
{
    internal class Strategy : IPlayer
    {
        private static readonly Position[] _corners = new Position[] 
        {
            new Position { Row = 0, Col = 0},
            new Position { Row = 0, Col = 7},
            new Position { Row = 7, Col = 0},
            new Position { Row = 7, Col = 7}
        };
		private static readonly Position[] _xsquares = new Position[]
        {
            new Position { Row = 1, Col = 1},
            new Position { Row = 1, Col = 6},
            new Position { Row = 6, Col = 1},
            new Position { Row = 6, Col = 6}
        };
        private int color;        
        private int depth;

        public Strategy(int color, int depth)
        {
            Debug.Assert(depth >= 0);
            Debug.Assert(color == Utility.BLACK || color == Utility.WHITE);
            this.color = color;            
            this.depth = depth;
        }

		private static Task<Move> FindBestMove(IEnumerable<Move> moves,
            int[][] cells,			
			int color,
			int depth)
		{
            return Task.Run<Move>(() =>
            {
                Move bestMove = null;
                int bestscore = int.MinValue;
                if (moves != null)
                {
                    foreach (var move in moves)
                    {
                        var copy = Utility.Clone(cells);
                        Debug.Assert(Utility.TryPlay(copy, move));                            
                        // find the best move our opponent can make. Hence, the minus sign.                        
                        int ss = -Score(copy, 0, -color, depth, false);
                        if (ss > bestscore)
                        {
                            bestscore = ss;
                            bestMove = move;                                    
                        }                            
                    }
                }
                return bestMove;
            });
         }		

		private static int Score(int[][] cells,
							   int d, 
							   int color,
							   int depth,
							   bool pass // true means our opponent did not have any move to play; he passed
								)
		{			
			int other = -color;
			if (d > depth)
			{
				// time to stop recursion
			    int s = 0;
				for(int i = 0; i < _corners.Length; i++)
				{
                    var corner = _corners[i];
                    var xsquare = _xsquares[i];
					var q = cells[corner.Row][corner.Col];
					if (q == color) { s += 300; }
					else if (q == other) { s -= 300; }
					else
					{
						q = cells[xsquare.Row][xsquare.Col];
						if (q == color) { s -= 50; }
						else if (q == other) { s += 50; }
					}
				}
				return s;
			}						
			int bestScore = int.MinValue;
			int n = 0; // will store the # of valid moves we can make
			for(int row = 0; row < Board.ROWS; row++)
			{
				for(int col = 0; col < Board.COLS; col++)
				{                    
                    var copy = Utility.Clone(cells);
                    var move = new Move { Row = row, Col = col, Color = color, PositionsToFlip = Utility.GetPositionsToFlip(row, col, color, copy) };
					if (Utility.TryPlay(copy, move))
					{
						n++;
						// find the score our opponent can make.
						int ss = -Score(copy, d+1, other, depth, false);
						if (ss > bestScore)
						{
							bestScore = ss;							
						}
					}
				}
			}
			if (n == 0)
			{
				// there is no valid move that we can play. we can't set bestPos to any meaningful value.
				if (pass)
				{
					// opponent also had no move to play
					// this means game will end 
					for(int i = 0; i < Board.ROWS; i++)
					{
						for(int j = 0; j < Board.COLS; j++)
						{
							if (cells[i][j] == color) { n++;}
							else if (cells[i][j] == other) {n--;}
						}
					}
					if (n > 0)
					{
						// we will win. 
						return n + 8000;
					}
					else
					{
						// we will lose. 
						return n - 8000;
					}						
				}
				else
				{
					bestScore = -Score(cells, d+1, other, depth, true);
				}		
			}
			if (d >= depth - 1)
			{
				return bestScore + (n << 3);
			}
			else
			{
				return bestScore;
			}
		}

        public int Color
        {
            get { return color; }
        }

        public event EventHandler<Move> Over;

        public async void Play(IList<Move> moves, int[][] state)
        {
            var move = await FindBestMove(moves, state, color, depth);
            RaiseEvent(move);
        }       

        private void RaiseEvent(Move move)
        {
            if (Over != null)
            {
                Over(this, move);
            }
        }
    }
}
