using ChessGameMultiplayer.Game.Board;

namespace ChessGameMultiplayer.Game.ChessPieces
{
    public class King : ChessPiece
    {
        public King(ChessPieceColor color) : base(color) { }

        public override bool IsValidMove(ChessBoard board, Position from, Position to)
        {
            if (board.GetPieceAt(to) != null && board.GetPieceAt(to).Color == Color)
            {
                return false; // Cannot capture own piece
            }
            
            if(Math.Abs(from.X - to.X) <= 1 && Math.Abs(from.Y - to.Y) <= 1)
            {
                return true; // King can move one square in any direction
            }
            return false; // Invalid move for king;
        }

        public override char GetSymbol() => Color == ChessPieceColor.White ? 'k' : 'K';
    }
}
