using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game
{
    public class TimeExpiredEventArgs : EventArgs
    {
        public ChessPieceColor ExpiredTimePlayerColor { get; }

        public TimeExpiredEventArgs(ChessPieceColor color)
        {
            ExpiredTimePlayerColor = color;
        }
    }
}
