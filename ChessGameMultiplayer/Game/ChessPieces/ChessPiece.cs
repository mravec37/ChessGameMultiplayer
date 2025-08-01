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

        // Check if the move is legal for this piece type
        public abstract bool IsValidMove(ChessBoard board, Position from, Position to);

        // Validate and optionally apply the move
        public virtual MoveResult MoveIfValid(MoveRequest request, ChessBoard board)
        {
            var from = request.From;
            var to = request.To;
            var piece = board.GetPieceAt(from);
            if (!IsValidMove(board, from, to))
            {
                return new MoveResult
                {
                    IsValid = false,
                    ErrorMessage = "Invalid move for this piece type."
                };
            }

            var captured = board.GetPieceAt(to);
            board.MovePiece(from, to);

            var effects = CreateMoveEffects(board, from, to, captured);

            return new MoveResult
            {
                IsValid = true,
                Affected = effects
            };
        }


        protected virtual List<MoveEffect> CreateMoveEffects(ChessBoard board, Position from, Position to, ChessPiece? captured)
        {
            var effects = new List<MoveEffect>();

            if (captured != null)
            {
                effects.Add(new MoveEffect
                {
                    Type = MoveEffectType.Capture,
                    to = to,
                    Piece = captured
                });
            }

            effects.Add(new MoveEffect
            {
                Type = MoveEffectType.Move,
                to = to,
                from = from,
                Piece = this
            });

            return effects;
        }


        public abstract char GetSymbol(); // e.g. 'p' for pawn, 'K' for black king
    }

}
