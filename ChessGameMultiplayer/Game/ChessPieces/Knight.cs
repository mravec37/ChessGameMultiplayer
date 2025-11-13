using ChessGameMultiplayer.Game.Board;

namespace ChessGameMultiplayer.Game.ChessPieces
{
    public class Knight : ChessPiece
    {
        public Knight(ChessPieceColor color) : base(color) { }

        public override bool IsValidMove(ChessBoard board, Position from, Position to)
        {
            if (board.GetPieceAt(to) != null && board.GetPieceAt(to).Color == Color)
            {
                return false; // Cannot capture own piece
            }

            // Knight moves in an "L" shape: two squares in one direction and one square perpendicular
            int dx = Math.Abs(from.X - to.X);
            int dy = Math.Abs(from.Y - to.Y);

            if ((dx == 2 && dy == 1) || (dx == 1 && dy == 2))
            {
                return true; // Valid knight move
            }
            return false; // Invalid move for knight
        }

        public override char GetSymbol() => Color == ChessPieceColor.White ? 'n' : 'N';

        public override List<Square> GetAttackedSquares(ChessBoard board, Position position)
        {
            List<Square> attackedSquares = new List<Square>();
            int[][] directions = new int[][]
          {
                new int[] {-1,2},
                new int [] {1,2},
                new int [] {-1,-2},
                new int [] {1,-2},
                new int [] {-2,1},
                new int [] {2,1},
                new int [] {-2,-1},
                new int [] {2,-1}
          };

            for (int i = 0; i < 8; i++)
            {
                int dx = position.X + directions[i][0];
                int dy = position.Y + directions[i][1];

                if (dx >= 0 && dx < 8 && dy >= 0 && dy < 8)
                {
                    attackedSquares.Add(board.GetSquare(new Position(dx, dy)));
                }
            }

            return attackedSquares;
        }
    }
}
