using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Othello
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private EventHandler<int[][]> boardUpdatedHandler;
        private EventHandler<object> tickHandler;
        private EventHandler gameOverHandler;
        private DateTime t0;
        private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            
        public MainPage()
        {
            this.InitializeComponent();
            tickHandler = new EventHandler<object>(dispatcherTimer_Tick);
            boardUpdatedHandler = new EventHandler<int[][]>(game_BoardUpdated);
            gameOverHandler = new EventHandler(game_GameOver);
            timer.Tick += tickHandler;            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NewGame();
        }

        private void NewGame()
        {
            t0 = DateTime.Now;
            timer.Start();
            var game = new Game(board);
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
                    if (e[i][j] == Common.BLACK) { blackScore++; }
                    else if (e[i][j] == Common.WHITE) { whiteScore++; } 
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
    }
}
