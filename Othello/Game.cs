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
        public Board Board { get { return board; } }

        public Game()
        {
            board = new Board(new Size(80, 80));            
            player1 = new Player(Common.BLACK, board);
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
        }
                        
        public void Begin()
        {
            Next(null, player1);            
        }

        private async void Next(Move move, IPlayer other)
        {
            RemoveMarkers();
            if (move != null)
            {
                board.AddPawn(move.Row, move.Col, move.Color);
                await board.Flip(move.PositionsToFlip);
            }
            var state = board.State();
            var moves = GetValidMoves(other, state);
            if (moves.Count > 0 || move != null)
            {
                AddMarkers(moves.Select(x => new Position { Row = x.Row, Col = x.Col }));
                other.Play(moves, state);
            }
            else
            {
                EndGame();
            }
        }

        private static async void EndGame()
        {
            // applying async/await is optional since there is no code 
            // after the ShowAsync method, but if you omit it,
            // compiler will issue a warning.
            var dialog = new MessageDialog("GAME OVER");
            await dialog.ShowAsync();   
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

        private IList<Move> GetValidMoves(IPlayer player, int[][] state)
        {
            var answer = new List<Move>();            
            var color = player.Color;
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
