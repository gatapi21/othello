using System;
using System.Collections.Generic;

namespace Othello
{
    internal class Position
    {
        public int Col { get; set; }
        public int Row { get; set; }
    }

    internal class Direction
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    internal class Move
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Color { get; set; }
        public List<Position> PositionsToFlip { get; set; }
    }

    internal class GameUpdateArgs : EventArgs
    {
        public IPlayer Player { get; set; }
        public int[][] Cells { get; set; }
    }
}
