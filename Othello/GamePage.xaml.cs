using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Othello.Common;
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
    [DataContract]
    internal enum GameMode 
    { 
        [EnumMember]
        OnePlayer, 
        [EnumMember]
        TwoPlayer 
    };

    /// <summary>
    /// information needed to resume a game, if app is suspended.
    /// </summary>
    [DataContract]
    internal class GameState
    {
        [DataMember]
        public GameMode GameMode { get; set; }
        [DataMember]
        public int Player { get; set; }
        [DataMember]
        public int Difficulty { get; set; }
        [DataMember]
        public int[][] Cells { get; set; }
        [DataMember]
        public DateTime? TimeStamp { get; set; }
        [DataMember]
        public bool GameOver { get; set; }            
    }
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GamePage : Othello.Common.LayoutAwarePage
    {
        private Board board;
        private EventHandler<GameUpdateArgs> boardUpdatedHandler;
        private EventHandler<object> tickHandler;
        private EventHandler gameOverHandler;
        private DateTime t0;
        private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        private GameState state;

        internal GameState State { get { return state; } }

        public GamePage()
        {
            this.InitializeComponent();
            this.SizeChanged += (s, e) =>
            {
                Reversi.SetView(this.unsnappedView, this.unsnappedView, this.unsnappedView, this.snapView);
            };
            tickHandler = new EventHandler<object>(dispatcherTimer_Tick);
            boardUpdatedHandler = new EventHandler<GameUpdateArgs>(game_BoardUpdated);
            gameOverHandler = new EventHandler(game_GameOver);
            timer.Tick += tickHandler;            
        }        

        void game_GameOver(object sender, EventArgs e)
        {
            timer.Stop();
            state.GameOver = true;
        }

        void game_BoardUpdated(object sender, GameUpdateArgs args)
        {
            var cells = args.Cells;
            int blackScore = 0, whiteScore = 0;
            for (int i = 0; i < cells.Length; i++)
            {
                for (int j = 0; j < cells[i].Length; j++)
                {
                    if (cells[i][j] == Reversi.BLACK) { blackScore++; }
                    else if (cells[i][j] == Reversi.WHITE) { whiteScore++; }
                }
            }
            blackScoreTextBlock.Text = blackScore.ToString();
            whiteScoreTextBlock.Text = whiteScore.ToString();
            state.Cells = cells;
            state.Player = args.Player.Color;
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            var offset = DateTime.Now - t0;
            time.Text = string.Format("{0:00}:{1:00}:{2:00}", offset.Hours, offset.Minutes, offset.Seconds);
        }

        internal Board Board { get { return this.board; } }

        internal Button PassButton { get { return this.passButton; } }

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
            NewGame(navigationParameter as GameState);            
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
        
        private void NewGame(GameState state)
        {
            this.state = state;
            this.board = new Board(state.Cells);
            board.Margin = new Thickness(20);
            board.SetValue(Grid.RowProperty, 0);
            board.SetValue(Grid.ColumnProperty, 0);
            this.root.Children.Add(board);
            IPlayer player1 = new Player(Reversi.BLACK, this);
            IPlayer player2 = null;
            if (state.GameMode == GameMode.OnePlayer)
            {
                int difficulty = Math.Max(0, Math.Min(state.Difficulty, 2));            
                player2 = new Strategy(Reversi.WHITE, difficulty);
            }
            else
            {
                player2 = new Player(Reversi.WHITE, this);
            }
            if (state.TimeStamp == null)
            {
                state.TimeStamp = DateTime.Now;
            }
            t0 = state.TimeStamp.Value;
            timer.Start();
            var game = new Game(board, player1, player2);
            game.Update += boardUpdatedHandler;
            game.GameOver += gameOverHandler;
            game.Begin(state.Player != Reversi.WHITE ? player1 : player2);            
        }

        private void backButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
