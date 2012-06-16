using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Popups;

namespace Othello
{
    internal class Game
    {
        private Board board;
        private IPlayer player1;
        private IPlayer player2;
        private IList<Marker> markers = new List<Marker>();

        public event EventHandler<int[][]> BoardUpdated;
        public event EventHandler GameOver;

        public Game(Board board)
        {
            this.board = board;            
            player1 = new Player(Common.BLACK, board);
            // uncomment for 2 player game 
            // player2 = new Player(Common.WHITE, board); 
            player2 = new Strategy(Common.WHITE, 1);
            player1.Over += (s, e) =>
                {
                    Next(e, player2);
                };
            player2.Over += (s, e) =>
                {
                    Next(e, player1);
                };
            ShowMarkers = true;
        }
                        
        public void Begin()
        {
            Next(null, player1);            
        }

        public bool ShowMarkers { get; set; }

        private async void Next(Move move, IPlayer other)
        {
            RemoveMarkers();
            if (move != null)
            {
                board.AddPawn(move.Row, move.Col, move.Color);
                await board.Flip(move.PositionsToFlip);
            }
            var state = board.State();
            if (BoardUpdated != null)
            {
                BoardUpdated(this, state);
            }
            var moves = GetValidMoves(other.Color, state);
            if (moves.Count > 0 || GetValidMoves(-other.Color, state).Count > 0)
            {
                if (ShowMarkers)
                {
                    AddMarkers(moves.Select(x => new Position { Row = x.Row, Col = x.Col }));
                }
                other.Play(moves, state);
            }
            else
            {
                EndGame();
            }
        }

        private async void EndGame()
        {
            var dialog = new MessageDialog("GAME OVER");
            await dialog.ShowAsync();
            if (GameOver != null)
            {
                GameOver(this, new EventArgs());
            }
        }

        private void RemoveMarkers()
        {
            foreach (var marker in markers)
            {
                board.RemoveUIElement(marker);
            }
            markers.Clear();
        }

        private void AddMarkers(IEnumerable<Position> positions)
        {
            foreach (var position in positions)
            {
                var marker = new Marker();
                board.AddUIElement(marker, position.Row, position.Col);
                markers.Add(marker);
            }
        }

        private IList<Move> GetValidMoves(int color, int[][] state)
        {
            var answer = new List<Move>();                        
            for(int i = 0; i < Board.ROWS; i++)
			{
				for(int j = 0; j < Board.COLS; j++)
				{
					if (state[i][j] == Common.UNDEFINED)
					{
                        var positions = Common.GetPositionsToFlip(i, j, color, state);
                        if (positions.Count > 0)
                        {
                            answer.Add(new Move { Row = i, Col = j, Color = color, PositionsToFlip = positions });
                        }
					}
				}
			}
            return answer;
        }
    }
}
