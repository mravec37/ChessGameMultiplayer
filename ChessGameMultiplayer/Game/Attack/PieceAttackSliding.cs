using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game.Attack
{
    public class PieceAttackSliding : PieceAttack
    {
        public List<Square> KingAttackingSequence { get; private set; }
        public PieceAttackSliding()
        {
            KingAttackingSequence = new List<Square>();
        }

        public PieceAttackSliding(ChessPiece piece, ChessBoard board) : base(piece, board)
        {
        }

        public void SetKingAttackingSequence(List<Square> kingAttackingSequence)
        {
            KingAttackingSequence = kingAttackingSequence; 
            foreach (Square square in KingAttackingSequence)
            {
                Console.WriteLine("King attacking sequence: X: " + Board.squarePositions[square].X + " Y: " + Board.squarePositions[square].Y);
            }
        }

        public override void UpdateAttackedSquares()
        {
            Console.WriteLine("Updatujem attacked square pre piece: " + Piece.GetType().Name);
            RemoveFromAllSquares();

            Position PiecePosition = Board.GetPiecePosition(Piece);
            //tu treba ziskat vsetky policka, ktore su napadnute Piece-om a update Square

            //tu nebude piece ale square processor od ktoreho sa zoberu attacked squares
            List<Square> updatedAttackedSquares = Piece.GetAttackingSquares(Board, PiecePosition);
            Squares = updatedAttackedSquares;

            foreach (var square in Squares)
            {
                square.AddAttackedSquaresSequence(this);
            }
        }
    }
}
