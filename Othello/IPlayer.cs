using System;
using System.Collections.Generic;

namespace Othello
{
    /// <summary>
    /// interface to represent a player - whether it be human, or computer
    /// </summary>
    internal interface IPlayer
    {
        /// <summary>
        /// Player's color - black or white
        /// </summary>
        int Color { get; }
        /// <summary>
        /// Player will raise this event to notify the <c>Game</c> the <c>Move</c> he wants to make
        /// </summary>
        event EventHandler<Move> Over;
        /// <summary>
        /// Give a chance to player to make his/her move
        /// </summary>
        /// <param name="possibleMoves">this is a list of possible moves the player can make</param>
        /// <param name="state">this is the current state of the board</param>
        void Play(IList<Move> possibleMoves, int[][] state);        
    }
}
