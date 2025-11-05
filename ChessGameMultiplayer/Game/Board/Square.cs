using ChessGameMultiplayer.Game.ChessPieces;
using ChessGameMultiplayer.Game.Attack;

namespace ChessGameMultiplayer.Game.Board
{

    public class Square
    {
        public ChessPiece? Piece { get; set; }

        // A square can belong to multiple attacked-squares sequences (rays/sets)
       // private readonly HashSet<AttackedSquaresSequence> _whiteAttackedSequences = new();
        //private readonly HashSet<AttackedSquaresSequence> _blackAttackedSequences = new();

        // Read-only view for external code
        //public IReadOnlyCollection<AttackedSquaresSequence> WhiteAttackedSequences => _whiteAttackedSequences;
        //public IReadOnlyCollection<AttackedSquaresSequence> BlackAttackedSequences => _whiteAttackedSequences;

        public List<PieceAttack> PieceAttacks { get; } = new();

        public Square()
        {
            // Piece = null; // implicit default
        }


        /*public void UpdateAttackedSquareSequences()
        {
            
        }*/

        public bool IsOccupied => Piece != null;
        public bool IsOccupiedBy(ChessPieceColor color) => Piece?.Color == color;

        /// <summary>Add this square to an attacked-squares sequence (no duplicates).</summary>
        /*public void AddWhiteAttackedSequence(AttackedSquaresSequence sequence)
        {
            _whiteAttackedSequences.Add(sequence);
        }

        public void AddBlackAttackedSequence(AttackedSquaresSequence sequence)
        {
            _blackAttackedSequences.Add(sequence);
        }

        /// <summary>Remove this square from an attacked-squares sequence.</summary>
        /// <returns>true if it was present and removed.</returns>
        public bool RemoveWhiteAttackedSequence(AttackedSquaresSequence sequence)
        {
            return _whiteAttackedSequences.Remove(sequence);
        }

        public bool RemoveBlackAttackedSequence(AttackedSquaresSequence sequence)
        {
            return _blackAttackedSequences.Remove(sequence);
        }

        /// <summary>Remove this square from all attacked-squares sequences.</summary>
        public void ClearAttackedSequences() => _whiteAttackedSequences.Clear();*/

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
