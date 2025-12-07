using ChessGameMultiplayer.Game.Moves;
using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Dto;

namespace ChessGameMultiplayer.Game.ChessPieces
{
    public abstract class ChessPiece
    {
        public ChessPieceColor Color { get; }

        protected ChessPiece(ChessPieceColor color)
        {
            Color = color;
        }

        public abstract bool IsValidMove(ChessBoard board, Position from, Position to);

        public abstract char GetSymbol(); //'p' for pawn, 'K' for black king

        public abstract List<Square> GetAttackedSquares(ChessBoard board, Position position);
    }

}
