using ChessGameMultiplayer.Game.Board;

namespace ChessGameMultiplayer.Game.ChessPieces
{
    public class Queen : ChessPiece
    {
        public Queen(ChessPieceColor color) : base(color) { }

        public override bool IsValidMove(ChessBoard board, Position from, Position to)
        {
            if (board.GetPieceAt(to) != null && board.GetPieceAt(to).Color == Color)
            {
                return false; // Cannot capture own piece
            }

            // Queen can move diagonally or in straight lines
            if (CheckDiagonalMovement(board, from, to) || CheckStraightLineMovement(board, from, to))
            {
                return true; 
            }
            return false; 

        }

        private bool CheckStraightLineMovement(ChessBoard board, Position from, Position to)
        {
            if (from.X != to.X && from.Y != to.Y)
            {
                return false; 
            }

            int dx = Math.Sign(to.X - from.X);
            int dy = Math.Sign(to.Y - from.Y);

            int x = from.X + dx;
            int y = from.Y + dy;

            while (x != to.X || y != to.Y)
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

        private bool CheckDiagonalMovement(ChessBoard board, Position from, Position to)
        {
            if (Math.Abs(from.X - to.X) != Math.Abs(from.Y - to.Y))
            {
                return false; 
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

        public override char GetSymbol() => Color == ChessPieceColor.White ? 'q' : 'Q';
    }
}
