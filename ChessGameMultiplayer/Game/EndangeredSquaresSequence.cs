using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game
{
    public class EndangeredSquaresSequence
    {
       public List<Square> Squares { get; private set; }
        public ChessPiece Piece { get; init; }  


        public EndangeredSquaresSequence()
        {
            Squares = new List<Square>();
        }
      
        public void AddSquare(Square square)
        {
            if (!Squares.Contains(square))
            {
                Squares.Add(square);
            }
        }
      
        public void Clear()
        {
            Squares.Clear();
        }
        public bool Contains(Square square)
        {
            return Squares.Contains(square);
        }
    }
}
