using ChessGameMultiplayer.Game.Board;

namespace ChessGameMultiplayer.Game.ChessPieces
{
    public class Rook : ChessPiece
    {
        public Rook(ChessPieceColor color) : base(color) { }

        public override bool IsValidMove(ChessBoard board, Position from, Position to)
        {
            if (board.GetPieceAt(to) != null && board.GetPieceAt(to).Color == Color)
            {
                return false; // Cannot capture own piece
            }
            if (from.X != to.X && from.Y != to.Y) {
                return false; // Rook can only move in straight lines
            }

            int dx = Math.Sign(to.X - from.X);
            int dy = Math.Sign(to.Y - from.Y);

            int x = from.X + dx;
            int y = from.Y + dy;

            while (x != to.X || y != to.Y)
            {
                //Dokonci
                if (board.GetPieceAt(new Position(x, y)) != null)
                {
                    return false; // Path is blocked
                }
                x += dx;
                y += dy;
            }

            return true;
        }

        public override char GetSymbol() => Color == ChessPieceColor.White ? 'r' : 'R';
    }
}
