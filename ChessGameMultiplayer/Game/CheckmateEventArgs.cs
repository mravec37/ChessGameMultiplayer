using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game
{
    public class CheckmateEventArgs : EventArgs
    {
        public ChessPieceColor Winner { private set; get; }

        public CheckmateEventArgs(ChessPieceColor currentTurn)
        {
            this.Winner = currentTurn;
        }
    }
}
