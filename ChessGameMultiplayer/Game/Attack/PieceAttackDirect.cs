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
            Console.WriteLine("Updatujem attacked square pre piece: " + Piece.GetType().Name);
            RemoveFromAllSquares();

            Position PiecePosition = Board.GetPiecePosition(Piece);
            //tu treba ziskat vsetky policka, ktore su napadnute Piece-om a update Square

            //tu nebude piece ale square processor od ktoreho sa zoberu attacked squares
           // List<Square> updatedAttackedSquares = Piece.GetAttackedSquares(Board, PiecePosition);

            SquareProcessor squareProcessor = new SquareProcessor();
            squareProcessor.UpdatePieceAttackSequences(Board, PiecePosition);
            Squares = squareProcessor.AttackedSquares;

            foreach (var square in Squares)
            {
                square.AddAttackedSquaresSequence(this);
            }
        }
    }
}
