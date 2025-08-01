using ChessGameMultiplayer.Game.Board;

namespace ChessGameMultiplayer.Game.ChessPieces
{
    public class Bishop : ChessPiece
    {
        public Bishop(ChessPieceColor color) : base(color) { }

        public override bool IsValidMove(ChessBoard board, Position from, Position to)
        {
            if(board.GetPieceAt(to) != null && board.GetPieceAt(to).Color == Color)
            {
                return false; // Cannot capture own piece
            }
            if (Math.Abs(from.X - to.X) != Math.Abs(from.Y - to.Y))
            {
                return false; // Bishop can only move diagonally
            }
            int dx = Math.Sign(to.X - from.X);
            int dy = Math.Sign(to.Y - from.Y);

            int x = from.X + dx;
            int y = from.Y + dy;
            while (x != to.X && y != to.Y)
            {
                if (board.GetPieceAt(new Position(x, y)) != null)
                {
                    return false; // Path is blocked
                }
                x += dx;
                y += dy;
            }
            return true;
        }

        public override char GetSymbol() => Color == ChessPieceColor.White ? 'b' : 'B';
    }
}
