using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Othello
{
    internal class Player : IPlayer
    {
        private int color;
        private Board board;
        private Button passButton;
        public event EventHandler<Move> Over;
        private EventHandler<Position> handler;
        private RoutedEventHandler passButtonClickHandler;
        private bool isPlaying;
        private IList<Move> moves;
        public int Color
        { 
            get
            { 
                return color;  
            }            
        }

        public Player(int color, GamePage page)
        {
            if (color != 1 && color != -1)
            {
                throw new ArgumentException();
            }
            this.board = page.Board;
            this.passButton = page.PassButton;
            this.color = color;
            this.handler = new EventHandler<Position>(board_Click);
            this.passButtonClickHandler = new RoutedEventHandler(passButton_Click);
        }

        public void Play(IList<Move> moves, int[][] state)
        {
            if (!isPlaying)
            {
                if (moves != null && moves.Count > 0)
                {
                    this.moves = moves;
                    board.Click += handler;
                }
                else
                {
                    this.passButton.Visibility = Visibility.Visible;
                    this.passButton.IsEnabled = true;
                    this.passButton.Click += passButtonClickHandler;
                }
                isPlaying = true;
            }
        }

        void passButton_Click(object sender, RoutedEventArgs e)
        {
            this.passButton.Click -= passButtonClickHandler;
            this.passButton.Visibility = Visibility.Collapsed;
            isPlaying = false;
            if (Over != null)
            {
                Over(this, null);
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
