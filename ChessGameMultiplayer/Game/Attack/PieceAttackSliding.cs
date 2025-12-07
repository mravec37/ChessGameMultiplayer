using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game.Attack
{
    public class PieceAttackSliding : PieceAttack
    {
        public List<Square> KingAttackingSequence { get; private set; }
        public List<Square> AimAtKingSequence { get; private set; }

       

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
           
            SquareProcessor squareProcessor = new SquareProcessor();
            squareProcessor.UpdatePieceAttackSequences(Board, PiecePosition);

            Squares = squareProcessor.AttackedSquares;
            KingAttackingSequence = squareProcessor.KingAttackSequence;
            AimAtKingSequence = squareProcessor.PotentialAttackSequence;

            Console.WriteLine("King attack sequence in PieceAttackSliding");
            foreach (Square square in KingAttackingSequence ?? new List<Square>())
            {
                Position pos = Board.squarePositions[square];
                Console.WriteLine("X: " + pos.X + " Y: " + pos.Y);
            }

            Squares.ForEach(square => square.AddAttackedSquaresSequence(this));
        }
        public bool AttacksKing()
        {
            return KingAttackingSequence != null && KingAttackingSequence.Count > 0;
        }

        public bool AimsAtKing()
        {
            return AimAtKingSequence != null && AimAtKingSequence.Count > 0;
        }
    }
}
