using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;
using System.IO.Pipelines;

namespace ChessGameMultiplayer.Game.Attack
{
    public class SquareProcessor
    {
        public List<Square> AttackedSquares { get; private set; }
        public List<Square> KingAttackSequence { get; private set; }
        public List<Square> PotentialAttackSequence { get; private set; }
        public void UpdatePieceAttackSequences(ChessBoard board, Position piecePosition)
        {
            ChessPiece? piece = board.GetPieceAt(piecePosition);
            if (piece != null)
            {
                List<Square> attackedSquares = piece.GetAttackedSquares(board, piecePosition);
                AttackedSquares = GetAttackSequence(attackedSquares, piece, board);
                SetKingAttackSequence(attackedSquares, piece, board);
                SetPotentialAttackSequence(attackedSquares, piece, board);

                Console.WriteLine("King attack sequence in square processor");
                foreach (Square square in KingAttackSequence ?? new List<Square>())
                {
                    Position pos = board.squarePositions[square];
                    Console.WriteLine("X: " + pos.X + " Y: " + pos.Y);
                }
            }

           /* if (piece is Pawn)
            {
                Console.WriteLine("Square processor attacked squares for Pawn:");
                foreach (Square square in AttackedSquares)
                {
                    Position pos = board.squarePositions[square];
                    Console.WriteLine("X: " + pos.X + " Y: " + pos.Y);
                }
                if (KingAttackSequence != null)
                {
                    foreach (Square square in KingAttackSequence)
                    {
                        Position pos = board.squarePositions[square];
                        Console.WriteLine("King attack sequence X: " + pos.X + " Y: " + pos.Y);
                    }
                }
                if (PotentialAttackSequence != null)
                {
                    foreach (Square square in PotentialAttackSequence)
                    {
                        Position pos = board.squarePositions[square];
                        Console.WriteLine("Potential attack sequence X: " + pos.X + " Y: " + pos.Y);
                    }
                }
            }*/
        }

        private void SetPotentialAttackSequence(List<Square> attackedSquares, ChessPiece piece, ChessBoard board)
        {
            if (piece is ChessPieceSlidingAttacker slidingAttacker && slidingAttacker.AimsAtKing)
            {

                Console.WriteLine("Setting potential attack sequence for piece: " + piece.GetType().Name);
                PotentialAttackSequence = GetKingAttackingSquares(attackedSquares, piece, board);
            }
        }

        private void SetKingAttackSequence(List<Square> attackedSquares, ChessPiece piece, ChessBoard board)
        {
           
            if (piece is ChessPieceSlidingAttacker slidingAttacker)
            {
                if (slidingAttacker.AttacksKing)
                {
                    Console.WriteLine("Setting king attack sequence for piece: " + piece.GetType().Name);
                    KingAttackSequence = GetKingAttackingSquares(attackedSquares, piece, board);
                } else
                {
                    Console.WriteLine("Sliding Piece does not attack king: " + piece.GetType().Name);
                }
              
            } 
        }

        private List<Square> GetKingAttackingSquares(List<Square> attackedSquares, ChessPiece piece, ChessBoard board)
        {
            List<Square> squares = new List<Square>();
            if (piece is ChessPieceSlidingAttacker slidingAttacker)
            { 
                Position piecePosition = board.GetPiecePosition(piece);

                foreach (Square square in attackedSquares)
                {
                    Position squarePos = board.squarePositions[square];
                    int dx = squarePos.X - piecePosition.X;
                    int dy = squarePos.Y - piecePosition.Y;
                    dx = dx == 0 ? 0 : dx / Math.Abs(dx);
                    dy = dy == 0 ? 0 : dy / Math.Abs(dy);
                    ChessPiece? squarePiece = board.GetPieceAt(squarePos);
                    Console.WriteLine("Checking square X: " + squarePos.X + " Y: " + squarePos.Y);


                    if (dx == slidingAttacker.KingDirection[0] && dy == slidingAttacker.KingDirection[1])
                    {
                        Console.WriteLine("Square is in king direction");
                        //if we reached the enemy king, stop and return squares
                        if (squarePiece != null && squarePiece is King && squarePiece.Color != piece.Color)
                        {
                            Console.WriteLine("Reached enemy king at square X: " + squarePos.X + " Y: " + squarePos.Y); 
                            return squares;
                        }
                        else
                        {
                            squares.Add(square);
                        }
                    }
                }
            }
            return squares;
        }
        private List<Square> GetAttackSequence(List<Square> attackedSquares, ChessPiece piece, ChessBoard board)
        { 
            if (piece is not ChessPieceSlidingAttacker) return attackedSquares;

            List<Square> attackSequence = new List<Square>();
            Position piecePosition = board.GetPiecePosition(piece);

            //if piece attacks king or doesnt attack king while not aiming at king return attacked squares as is
            //if piece aims at king with the attack blocked we must process this further- remove squares after blocking piece

            if (piece is ChessPieceSlidingAttacker slidingAttacker && slidingAttacker.AimsAtKing)
            {
                bool blocked = false;
                foreach (Square square in attackedSquares)
                { 
                    Position squarePos = board.squarePositions[square];
                    int dx = squarePos.X - piecePosition.X;
                    int dy = squarePos.Y - piecePosition.Y;
                    dx = dx == 0 ? 0 : dx / Math.Abs(dx);
                    dy = dy == 0 ? 0 : dy / Math.Abs(dy);
                    ChessPiece? squarePiece = board.GetPieceAt(squarePos);
                    if (dx == slidingAttacker.KingDirection[0] && dy == slidingAttacker.KingDirection[1])
                    {
                        if (squarePiece != null && !blocked)
                        {
                            attackSequence.Add(square);
                            blocked = true;
                        }
                        else if (!blocked)
                        {
                            attackSequence.Add(square);
                        }
                    }
                    else
                    {
                        attackSequence.Add(square);
                    }
                }
                return attackSequence;
            } else
            {
                return attackedSquares;
            } 
        }       
    }
}
