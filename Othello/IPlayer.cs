using System;
using System.Collections.Generic;

namespace Othello
{
    internal interface IPlayer
    {
        int Color { get; }
        event EventHandler<Move> Over;
        void Play(IList<Move> possibleMoves, int[][] state);        
    }
}
