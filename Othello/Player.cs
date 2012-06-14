using System;
using System.Collections.Generic;

namespace Othello
{
    internal class Player : IPlayer
    {
        private int color;
        private Board board;
        public event EventHandler<Move> Over;
        private EventHandler<Position> handler;
        private bool isPlaying;
        private IList<Move> moves;
        public int Color
        { 
            get
            { 
                return color;  
            }            
        }

        public Player(int color, Board board)
        {
            if (color != 1 && color != -1)
            {
                throw new ArgumentException();
            }
            this.board = board;
            this.color = color;
            this.handler = new EventHandler<Position>(board_Click);
        }

        public void Play(IList<Move> moves, int[][] state)
        {
            if (!isPlaying)
            {
                this.moves = moves;
                board.Click += handler;
                isPlaying = true;
            }
        }

        void board_Click(object sender, Position e)
        {
            if (moves != null)
            {
                foreach (var move in moves)
                {
                    if (move.Row == e.Row && move.Col == e.Col)
                    {
                        board.Click -= handler;
                        isPlaying = false;
                        if (Over != null)
                        {
                            Over(this, move);
                        }
                    }
                }
            }
        }        
    }
}
