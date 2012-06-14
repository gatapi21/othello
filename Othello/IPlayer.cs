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
        /// <param name="opponentsMove">this is the move which the opponent has just played. It is equal to null
        /// if opponent passed (had no move to play)</param>
        void Play(IList<Move> possibleMoves, int[][] state, Move opponentsMove);        
    }
}
