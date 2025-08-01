using ChessGameMultiplayer.Game.Board;

namespace ChessGameMultiplayer.Game.ChessPieces
{
    public class Pawn : ChessPiece
    {
        public Pawn(ChessPieceColor color) : base(color) { }

        public override bool IsValidMove(ChessBoard board, Position from, Position to)
        {
            if (board.GetPieceAt(to) != null && board.GetPieceAt(to).Color == Color)
            {
                return false; // Cannot capture own piece
            }
            if(Color == ChessPieceColor.Black)
            {
               return BlackPawnIsValidMovement(board, from, to);
            }
            else
            {
              return WhitePawnIsValidMovement(board, from, to);
            }

            return false;
        }

        private bool WhitePawnIsValidMovement(ChessBoard board, Position from, Position to)
        {
            if (from.Y == 6 && to.Y == 4 && from.X == to.X && board.GetPieceAt(new Position(from.X, 5)) == null)
            {
                return true; // Initial double move
            }
            if (to.Y == from.Y - 1 && to.X == from.X && board.GetPieceAt(to) == null)
            {
                return true; // Single forward move
            }
            if (to.Y == from.Y - 1 && Math.Abs(to.X - from.X) == 1 && board.GetPieceAt(to) != null)
            {
                return true; // Capture move
            }

            return false;
        }

        private bool BlackPawnIsValidMovement(ChessBoard board, Position from, Position to)
        {
            if (from.Y == 1 && to.Y == 3 && from.X == to.X && board.GetPieceAt(new Position(from.X, 2)) == null)
            {
                return true; // Initial double move
            }
            if (to.Y == from.Y + 1 && to.X == from.X && board.GetPieceAt(to) == null)
            {
                return true; // Single forward move
            }
            if (to.Y == from.Y + 1 && Math.Abs(to.X - from.X) == 1 && board.GetPieceAt(to) != null)
            {
                return true; // Capture move
            }

            return false;
        }

        public override char GetSymbol()
        {
            return Color == ChessPieceColor.White ? 'p' : 'P';
        }
    }
}
