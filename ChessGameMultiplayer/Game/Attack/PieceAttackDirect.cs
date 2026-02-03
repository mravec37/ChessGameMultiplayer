using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game.Attack
{
    public class PieceAttackDirect : PieceAttack
    {
        public PieceAttackDirect(ChessPiece piece, ChessBoard board) : base(piece, board)
        {
        }

        public override void UpdateAttackedSquares()
        {
            Console.WriteLine("Updatujem attacked square pre piece: " + Piece.GetType().Name + " na pozicii: " + Board.GetPiecePosition(Piece).X + ":" + Board.GetPiecePosition(Piece).Y);
            RemoveFromAllSquares();

            Position piecePosition = Board.GetPiecePosition(Piece);
          
            SquareProcessor squareProcessor = new SquareProcessor();
            squareProcessor.UpdatePieceAttackSequences(Board, piecePosition);
            Squares = squareProcessor.AttackedSquares;

            foreach (var square in Squares)
            {
                square.AddAttackedSquaresSequence(this);
            }
        }
    }
}
