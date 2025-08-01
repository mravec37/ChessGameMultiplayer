using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game.Moves
{
    public class MoveEffect
    {
        public MoveEffectType Type { get; set; }
        public Position from { get; set; }
        public Position to { get; set; }
        public ChessPiece? Piece { get; set; } 
    }
}
