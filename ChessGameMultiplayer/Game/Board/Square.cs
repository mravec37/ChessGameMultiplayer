using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game.Board
{
    public class Square
    {
        //public Position Position { get; }
        public ChessPiece? Piece { get; set; }

        public Square()
        {
            //Position = new Position(x, y);
            Piece = null;
        }

        public bool IsOccupied => Piece != null;
        public bool IsOccupiedBy(ChessPieceColor color) => Piece?.Color == color ? true : false; //Moze byt zle
    }

}
