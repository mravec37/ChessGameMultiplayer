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

            if (Math.Abs(from.X - to.X) <= 1 && Math.Abs(from.Y - to.Y) <= 1)
            {
                return true; // King can move one square in any direction
            }
            return false; // Invalid move for king;
        }

        public override char GetSymbol() => Color == ChessPieceColor.White ? 'k' : 'K';

        public override List<Square> GetAttackingSquares(ChessBoard board, Position position)
        {
            List<Square> attackedSquares = new List<Square>();
            int[][] directions = new int[][]
           {
                new int[] {1,1},
                new int [] {1,-1},
                new int [] {-1,1 },
                new int [] {-1,-1 },
                new int [] {-1,0 },
                new int [] {1,0 },
                new int [] {0,-1 },
                new int [] {0,1 }
           };

            for (int i = 0; i < 8; i++)
            {
                int dx = position.X + directions[i][0];
                int dy = position.Y + directions[i][1];

                if (dx >= 0 && dx < 8 && dy >= 0 && dy < 8)
                {
                    attackedSquares.Add(board.GetSquare(new Position(dx, dy)));
                   // Console.WriteLine("Square on position X: " + dx + " Y: " + dy + " added");
                }
            }

            return attackedSquares;
        }

        public List<Square> GetPossibleMoves(ChessBoard board, Position position)
        {
            List<Square> attackedSquares = GetAttackingSquares(board, position);
            List<Square> possibleMoves = new List<Square>();
            foreach (Square square in attackedSquares)
            {
                ChessPiece piece = square.Piece;
                if (piece == null || piece.Color != Color && piece is not King)
                {
                    possibleMoves.Add(square);
                    //Console.WriteLine("Possible move ");
                }
            }
            return possibleMoves;
        }
    }
}
