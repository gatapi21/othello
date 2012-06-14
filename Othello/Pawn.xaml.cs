using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Othello
{
    public sealed partial class Pawn : UserControl
    {
        private int color;
        private TaskCompletionSource<bool> tcs;

        public int Color
        {
            get { return color; }
        }

        public Pawn() : this(Common.UNDEFINED, new Size(73, 73)) { }

        public Pawn(int color, Size size)
        {
            this.InitializeComponent();
            this.color = color;
            myBrush.Color = Common.IntToColor(color);
            myEllipse.Width = size.Width;
            myEllipse.Height = size.Height;
            myStoryboard1.Completed += myStoryboard1_Completed;
            myStoryboard2.Completed += myStoryboard2_Completed;            
        }

        void myStoryboard2_Completed(object sender, object e)
        {            
            tcs.SetResult(true);
            tcs = null;
        }       

        void myStoryboard1_Completed(object sender, object e)
        {
            myStoryboard2.Begin();
            ToggleColor(); 
        }

        private void ToggleColor()
        {
            var c = myBrush.Color;
            myBrush.Color = Windows.UI.Color.FromArgb(0xff, (byte)(0xff - c.R), (byte)(0xff - c.G), (byte)(0xff - c.B));            
        }

        public Task Flip()
        {
            Debug.Assert(tcs == null);
            tcs = new TaskCompletionSource<bool>();
            color = -color;
            myStoryboard1.Begin();
            return tcs.Task;
        }       
    }
}
