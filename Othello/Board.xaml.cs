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
        private const int MINIMUM_SIZE = 32;
        private static int[][] _default;
        private Rectangle[][] rectangles;
        private Pawn[][] pawns;

        public event EventHandler<Position> Click;

        private static int[][] defaultState
        {
            get
            {
                if (_default == null)
                {
                    var mid = Board.ROWS / 2 - 1;
                    _default = new int[Board.ROWS][];
                    for (int i = 0; i < Board.ROWS; i++)
                    {
                        _default[i] = new int[Board.COLS];
                        for (int j = 0; j < Board.COLS; j++)
                        {
                            var color = Reversi.UNDEFINED;
                            if (i == mid)
                            {
                                if (j == i)
                                {
                                    color = Reversi.WHITE;
                                }
                                else if (j == i + 1)
                                {
                                    color = Reversi.BLACK;
                                }                                
                            }
                            else if (i == mid + 1)
                            {
                                if (j == i - 1)
                                {
                                    color = Reversi.BLACK;
                                }
                                else if (j == i)
                                {
                                    color = Reversi.WHITE;
                                }                                
                            }                                                       
                           _default[i][j] = color;                            
                        }
                    }
                }
                return _default;
            }            
        }

        public int[][] State()
        {
            var cells = new int[pawns.Length][];
            for (int i = 0; i < pawns.Length; i++)
            {
                cells[i] = new int[pawns[i].Length];
                for (int j = 0; j < cells[i].Length; j++)
                {
                    var pawn = pawns[i][j];
                    cells[i][j] = pawn == null ? Reversi.UNDEFINED : pawn.Color; 
                }
            }
            return cells;            
        }
        
        internal Board(int[][] state)
        {
            this.InitializeComponent();
            AddRowAndColDefinitions();
            CreateCells();
            if (state == null)
            {
                CreatePawns(defaultState);
            }
            else
            {
                CreatePawns(state);
            }
        }

        public Board() : this(null)
        {            
        }
       
        protected override Size MeasureOverride(Size availableSize)
        {
            var min = Math.Max(Math.Min(availableSize.Width, availableSize.Height), MINIMUM_SIZE);            
            foreach (var rowDefinition in root.RowDefinitions)
            {
                rowDefinition.Height = new GridLength(min / Board.ROWS, GridUnitType.Pixel);
            }
            foreach (var colDefinition in root.ColumnDefinitions)
            {
                colDefinition.Width = new GridLength(min / Board.COLS, GridUnitType.Pixel);
            }
            return new Size(min, min);
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
            var pawn = new Pawn(color);
            pawns[row][col] = pawn;
            AddUIElement(pawn, row, col);
        }

        private void AddRowAndColDefinitions()
        {
            for (int i = 0; i < ROWS; i++)
            {
                root.RowDefinitions.Add(new RowDefinition());
            }
            for (int j = 0; j < COLS; j++)
            {
                root.ColumnDefinitions.Add(new ColumnDefinition());
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
        
        private void CreatePawns(int[][] state)
        {
            pawns = new Pawn[Board.ROWS][];
            for (int i = 0; i < Board.ROWS; i++)
            {
                pawns[i] = new Pawn[Board.COLS];
                for (int j = 0; j < Board.COLS; j++)
                {
                    var color = state[i][j];
                    if (color != Reversi.UNDEFINED)
                    {
                        AddPawn(i, j, color);
                    }
                }
            }
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
