using ChessGameMultiplayer.Game.ChessPieces;
using ChessGameMultiplayer.Game.Attack;

namespace ChessGameMultiplayer.Game.Board
{

    public class Square
    {
        public ChessPiece? Piece { get; set; }

        public List<PieceAttack> PieceAttacks { get; } = new();



        public bool IsOccupied => Piece != null;
        public bool IsOccupiedBy(ChessPieceColor color) => Piece?.Color == color;


        public void AddAttackedSquaresSequence(PieceAttack sequence)
        {
            if (!PieceAttacks.Contains(sequence))
            {
                PieceAttacks.Add(sequence);
            }
        }

        internal void RemovePiece()
        {
            if(Piece == null)
            {
                throw new InvalidOperationException("Cannot remove piece from an empty square.");
            }

            Piece = null;
        }

        internal ChessPiece SetPiece(ChessPiece? piece)
        {
            //if there is already a piece, we dont need to update attacked squares sequences
            if (Piece != null) {
                ChessPiece captured = Piece;
                Piece = piece;
                return captured;
            }
            Piece = piece;
            return null;
        }

        public void UpdateAllAttackedSquaresSequences()
        {
            List<PieceAttack> attackedSquaresSequencesCopy = new List<PieceAttack>(PieceAttacks);
            foreach (var sequence in attackedSquaresSequencesCopy)
            {
                Console.WriteLine("Updatujem attacked square sequences v square");
                sequence.UpdateAttackedSquares();
            }
        }
    }


}
