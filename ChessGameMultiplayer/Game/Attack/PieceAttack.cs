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

        public abstract void UpdateAttackedSquares();

        public void RemoveFromAllSquares()
        {
            if (Squares != null)
            {
                Squares.ForEach(Squares => Squares.PieceAttacks.Remove(this));
                Squares.Clear();
            }
        }
    }
}
