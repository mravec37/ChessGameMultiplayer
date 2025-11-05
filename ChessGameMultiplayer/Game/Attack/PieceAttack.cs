using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game.Attack
{
    public abstract class PieceAttack
    {
       public List<Square> Squares { get; protected set; }
       public ChessBoard Board { get; init; }

       public ChessPiece Piece { get; init; }  


        public PieceAttack()
        {
            Squares = new List<Square>();
        }

        public PieceAttack(ChessPiece piece, ChessBoard board)
        {
            Squares = new List<Square>();
            Piece = piece;
            Board = board;
        }


        /*public void UpdateAttackedSquares()
        {
            Console.WriteLine("Updatujem attacked square pre piece: " + Piece.GetType().Name);
            RemoveFromAllSquares();

            Position PiecePosition = Board.GetPiecePosition(Piece);
            //tu treba ziskat vsetky policka, ktore su napadnute Piece-om a update Squares

            //tu nebude piece ale square processor od ktoreho sa zoberu attacked squares
            List<Square> updatedAttackedSquares = Piece.GetAttackingSquares(Board, PiecePosition);
            Squares = updatedAttackedSquares;

            foreach (var square in Squares)
            {
                square.AddAttackedSquaresSequence(this);
            }
        }*/
        public abstract void UpdateAttackedSquares();

        public void RemoveFromAllSquares()
        {
            if (Squares != null)
            {
                foreach (var square in Squares)
                {
                    square.PieceAttacks.Remove(this);
                }
                Squares.Clear();
            }
        }
    }
}
