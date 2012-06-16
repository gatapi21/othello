﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Othello
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GamePage : Othello.Common.LayoutAwarePage
    {
        private EventHandler<int[][]> boardUpdatedHandler;
        private EventHandler<object> tickHandler;
        private EventHandler gameOverHandler;
        private DateTime t0;
        private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

        public GamePage()
        {
            this.InitializeComponent();            
            tickHandler = new EventHandler<object>(dispatcherTimer_Tick);
            boardUpdatedHandler = new EventHandler<int[][]>(game_BoardUpdated);
            gameOverHandler = new EventHandler(game_GameOver);
            timer.Tick += tickHandler;            
        }

        private void NewGame(IPlayer player1, IPlayer player2)
        {
            t0 = DateTime.Now;
            timer.Start();
            var game = new Game(board, player1, player2);
            game.BoardUpdated += boardUpdatedHandler;
            game.GameOver += gameOverHandler;
            game.Begin();
        }

        void game_GameOver(object sender, EventArgs e)
        {
            timer.Stop();
        }

        void game_BoardUpdated(object sender, int[][] e)
        {
            int blackScore = 0, whiteScore = 0;
            for (int i = 0; i < e.Length; i++)
            {
                for (int j = 0; j < e[i].Length; j++)
                {
                    if (e[i][j] == Utility.BLACK) { blackScore++; }
                    else if (e[i][j] == Utility.WHITE) { whiteScore++; }
                }
            }
            blackScoreTextBlock.Text = blackScore.ToString();
            whiteScoreTextBlock.Text = whiteScore.ToString();
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            var offset = DateTime.Now - t0;
            time.Text = string.Format("{0:00}:{1:00}:{2:00}", offset.Hours, offset.Minutes, offset.Seconds);
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (pageState == null)
            {
                int? difficulty = navigationParameter as int?;
                IPlayer player1 = new Player(Utility.BLACK, this.board);
                IPlayer player2 = null;
                if (difficulty != null && difficulty.Value >= 0 && difficulty.Value <= 2)
                {
                    player2 = new Strategy(Utility.WHITE, difficulty.Value);
                }
                else
                {
                    player2 = new Player(Utility.WHITE, this.board);
                }
                NewGame(player1, player2);
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}