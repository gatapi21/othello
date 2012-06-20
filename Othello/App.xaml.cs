using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Othello
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// The filename in which the favourites list is stored.
        /// </summary>
        private const string FileName1 = "Othello.xml";

        /// <summary>
        /// The filename used while writing out the latest copy of the list.
        /// </summary>
        /// <remarks>
        /// When we save state, we write out to a new file so that if we crash or the power fails part way
        /// through, we don't lose anything.
        /// Once we're done writing it out, we rename the file to the normal name, specifying that we want to
        /// overwrite whatever was there before.
        /// </remarks>
        private const string FileName2 = "Othello.tmp";
        private Frame rootFrame;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;                       
        }

       
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }
            
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                var gameState = await LoadGameStateAsync();
                if (gameState != null)
                {
                    CompleteLaunch(typeof(GamePage), gameState);
                    return;
                }                
            }

            CompleteLaunch(typeof(MainPage), null);
        }

        private void CompleteLaunch(Type pageType, object args)
        {
            rootFrame = new Frame();
            if (!rootFrame.Navigate(pageType, args))
            {
                throw new Exception("failed to create main page!");
            }
            
            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }
        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            var page = rootFrame.Content as GamePage;            
            if (page != null && !page.State.GameOver)
            {
                // game is in progress, save its state
                await SaveGameStateAsync(page.State);
            }
            else
            {
                // delete any old file
                DeleteAsync();
            }
            deferral.Complete();
        }

        private async Task SaveGameStateAsync(GameState state)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(FileName2, CreationCollisionOption.ReplaceExisting);
            IRandomAccessStream ras = await file.OpenAsync(FileAccessMode.ReadWrite);
            IOutputStream ostr = ras.GetOutputStreamAt(0);
            var serializer = new DataContractSerializer(typeof(GameState));
            using (Stream clrStream = ostr.AsStreamForWrite())
            {
                serializer.WriteObject(clrStream, state);
            }
            await file.RenameAsync(FileName1, NameCollisionOption.ReplaceExisting);
        }

        private static async Task<GameState> LoadGameStateAsync()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;            
            StorageFile file;
            try
            {
                file = await folder.GetFileAsync(FileName1);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            IRandomAccessStream ras = await file.OpenAsync(FileAccessMode.ReadWrite);
            IInputStream ostr = ras.GetInputStreamAt(0);
            var serializer = new DataContractSerializer(typeof(GameState));
            using (Stream clrStream = ostr.AsStreamForRead())
            {
                return serializer.ReadObject(clrStream) as GameState;
            }            
        }

        private static async void DeleteAsync()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file;
            try
            {
                file = await folder.GetFileAsync(FileName1);
            }
            catch (FileNotFoundException)
            {
                return;
            }
            await file.DeleteAsync();
        }
    }
}
