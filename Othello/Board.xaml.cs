/*---------------------------------
<author>siddjain</author>
 * All Rights Reserved
/---------------------------------*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Othello
{
    internal sealed partial class Board : UserControl
    {        
        public const int ROWS = 8;
        public const int COLS = 8;
        private const int THICKNESS = 3;
        private double cellWidth = 80;
        private double cellHeight = 80;
        private Size pawnSize;
        private Rectangle[][] rectangles;
        private Pawn[][] pawns;

        public event EventHandler<Position> Click;

        public int[][] State()
        {
            var cells = new int[pawns.Length][];
            for (int i = 0; i < pawns.Length; i++)
            {
                cells[i] = new int[pawns[i].Length];
                for (int j = 0; j < cells[i].Length; j++)
                {
                    var pawn = pawns[i][j];
                    cells[i][j] = pawn == null ? Common.UNDEFINED : pawn.Color; 
                }
            }
            return cells;            
        }

        public Board() : this(new Size(80, 80), null) { }

        public Board(Size cellSize, MediaElement soundElement)
        {
            this.InitializeComponent();
            cellHeight = cellSize.Height;
            cellWidth = cellSize.Width;
            pawnSize = new Size { Width = cellWidth - 2 * (THICKNESS + 1), Height = cellHeight - 2 * (THICKNESS + 1) };
            AddRowAndColDefinitions();
            CreateCells();
            CreatePawns();
        }        

        public async Task Flip(IEnumerable<Position> positions)
        {
            foreach (var position in positions)
            {
                var pawn = pawns[position.Row][position.Col];
                Debug.Assert(pawn != null);
                await pawn.Flip();
            }            
        }

        public void AddPawn(int row, int col, int color)
        {
            Debug.Assert(pawns[row][col] == null);
            var pawn = new Pawn(color, pawnSize, App.SoundElement);
            pawns[row][col] = pawn;
            AddUIElement(pawn, row, col);
        }

        private void AddRowAndColDefinitions()
        {
            for (int i = 0; i < ROWS; i++)
            {
                root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellHeight, GridUnitType.Pixel) });
            }
            for (int j = 0; j < COLS; j++)
            {
                root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cellWidth, GridUnitType.Pixel) });
            }
        }

        private void CreateCells()
        {
            rectangles = new Rectangle[ROWS][];
            var blackBrush = new SolidColorBrush(Colors.Black);
            var greenBrush = new SolidColorBrush(Color.FromArgb(0xff, 0, 0xaa, 0));
            var thickness = new Thickness(THICKNESS);
            for (int i = 0; i < ROWS; i++)
            {
                rectangles[i] = new Rectangle[COLS];
                for (int j = 0; j < COLS; j++)
                {
                    var cell = new Rectangle { Stroke = blackBrush, StrokeThickness = THICKNESS, Fill = greenBrush };
                    cell.SetValue(Grid.RowProperty, i);
                    cell.SetValue(Grid.ColumnProperty, j);
                    cell.Tapped += cell_Tapped;
                    rectangles[i][j] = cell;
                    root.Children.Add(cell);
                }
            }
        }

        private void CreatePawns()
        {
            pawns = new Pawn[Board.ROWS][];
            for (int i = 0; i < Board.ROWS; i++)
            {
                pawns[i] = new Pawn[Board.COLS];
            }
            AddPawn(3, 3, Common.BLACK);
            AddPawn(4, 4, Common.BLACK);
            AddPawn(3, 4, Common.WHITE);
            AddPawn(4, 3, Common.WHITE);
        }        

        public void AddUIElement(UIElement element, int row, int col)
        {
            if (element != null)
            {
                element.SetValue(Grid.RowProperty, row);
                element.SetValue(Grid.ColumnProperty, col);
                root.Children.Add(element);
            }
        }

        public void RemoveUIElement(UIElement element)
        {
            if (element != null)
            {
                root.Children.Remove(element);
            }
        }

        
        void cell_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int row, col;
            GetRowCol(sender as Rectangle, out row, out col);
            if (Click != null && row >= 0 && row < ROWS && col >= 0 && col < COLS && pawns[row][col] == null)
            {
                Click(sender, new Position { Col = col, Row = row });
            }            
        }

        private void GetRowCol(Rectangle r, out int row, out int col)
        {
            row = -1;
            col = -1;
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    if (rectangles[i][j] == r)
                    {
                        row = i;
                        col = j;
                        return;
                    }
                }
            }
        }
    }
}
