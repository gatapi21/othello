using System;
using Windows.UI.ViewManagement;
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
        public MainPage()
        {
            this.InitializeComponent();            
            this.SizeChanged += (s, e) =>
            { 
                Reversi.SetView(this.filledView, this.filledView, this.filledView, this.snapView);
            };
        }       

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {            
            this.Frame.Navigate(typeof(ChooseDifficultyPage));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GamePage), new GameState { GameMode = GameMode.TwoPlayer });
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CreditsPage));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RulesPage));
        }                
    }
}
