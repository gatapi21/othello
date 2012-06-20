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
        /// <summary>
        /// This event is raised just before a player plays his turn. It reflects the current state of the board,
        /// and the player who is about to play. This information will be need to be persisted in case the app is suspended
        /// </summary>
        public event EventHandler<GameUpdateArgs> Update;
        public event EventHandler GameOver;

        public Game(Board board, IPlayer player1, IPlayer player2)
        {
            this.board = board;
            this.player1 = player1;
            this.player2 = player2;
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
                        
        public void Begin(IPlayer player)
        {
            Next(null, player);            
        }

        public bool ShowMarkers { get; set; }

        private async void Next(Move move, IPlayer player)
        {
            RemoveMarkers();
            if (move != null)
            {
                board.AddPawn(move.Row, move.Col, move.Color);
                await board.Flip(move.PositionsToFlip);
            }
            var state = board.State();
            if (Update != null)
            {
                Update(this, new GameUpdateArgs { Cells = state, Player = player });
            }
            var moves = GetValidMoves(player.Color, state);
            if (moves.Count > 0 || GetValidMoves(-player.Color, state).Count > 0)
            {
                if (ShowMarkers)
                {
                    AddMarkers(moves.Select(x => new Position { Row = x.Row, Col = x.Col }));
                }
                player.Play(moves, state);
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
					if (state[i][j] == Reversi.UNDEFINED)
					{
                        var positions = Reversi.GetPositionsToFlip(i, j, color, state);
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
