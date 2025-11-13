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

        public override List<Square> GetAttackedSquares(ChessBoard board, Position position)
        {
            List<Square> attackedSquares = new List<Square>();

            int coeficientY= 0;
            if(Color == ChessPieceColor.White) coeficientY = -1;
            else coeficientY = 1; 

            int dx = position.X + 1;
            int dy = position.Y + coeficientY;

            if (dx >= 0 && dx < 8 && dy >= 0 && dy < 8)
            {
                Console.WriteLine("pawn attacked square1 X: " + dx + " Y: " + dy);
                attackedSquares.Add(board.GetSquare(new Position(dx, dy)));
            }
            dx = position.X - 1;
            if (dx >= 0 && dx < 8 && dy >= 0 && dy < 8)
            {
                Console.WriteLine("pawn attacked square2 X: " + dx + " Y: " + dy);
                attackedSquares.Add(board.GetSquare(new Position(dx, dy)));
            }
            //return attackedSquares;
            return new List<Square>();
        }


        public List<Square> GetPossibleMoves(ChessBoard board, Position position)
        {
            List<Square> possibleMoves = new List<Square>();

            int coeficientY = 0;
            if (Color == ChessPieceColor.White) coeficientY = -1;
            else coeficientY = 1;

            int dx = position.X + 1;
            int dy = position.Y + coeficientY;

            Console.WriteLine("Pawn position X: " + position.X + " Y: " + position.Y);

            //check if theres a piece on attacking positions to check if the pawn can move here
            if (dx >= 0 && dx < 8 && dy >= 0 && dy < 8 && board.GetSquare(new Position(dx, dy)).Piece != null)
            {
                Console.WriteLine("pawn can move to X: " + dx + " Y: " + dy);
                possibleMoves.Add(board.GetSquare(new Position(dx, dy)));
            }
            dx = position.X - 1;
            if (dx >= 0 && dx < 8 && dy >= 0 && dy < 8 && board.GetSquare(new Position(dx, dy)).Piece != null)
            {
                Console.WriteLine("pawn can move to X: " + dx + " Y: " + dy);
                possibleMoves.Add(board.GetSquare(new Position(dx, dy)));
            }

            dx = position.X;

            //check if pawn can move forward
            if (dx >= 0 && dx < 8 && dy >= 0 && dy < 8 && board.GetSquare(new Position(dx, dy)).Piece == null)
            {
                Console.WriteLine("pawn can move to X: " + dx + " Y: " + dy);
                possibleMoves.Add(board.GetSquare(new Position(dx, dy)));
            }

            //doublemove check 
            dy += coeficientY;
            int doubleMoveY = 0;

            if (coeficientY < 0) {
                doubleMoveY = 6;
            }
            else {
                doubleMoveY = 1;
            }

            if (dx >= 0 && dx < 8 && position.Y == doubleMoveY && board.GetSquare(new Position(dx, dy)).Piece == null)
            {
                Console.WriteLine("pawn can move to X: " + dx + " Y: " + dy);
                possibleMoves.Add(board.GetSquare(new Position(dx, dy)));
            }

            return possibleMoves;
        }

    }
}
