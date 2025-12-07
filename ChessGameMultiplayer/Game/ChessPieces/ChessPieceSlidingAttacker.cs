using ChessGameMultiplayer.Game.Attack;

namespace ChessGameMultiplayer.Game.ChessPieces
{
    public abstract class ChessPieceSlidingAttacker : ChessPiece
    {
     
        public bool AimsAtKing;
        public bool AttacksKing;
        public int[] KingDirection;

        protected ChessPieceSlidingAttacker(ChessPieceColor color) : base(color)
        {
            AimsAtKing = false;
            KingDirection = new int[2] { 0, 0 };
        }
    }
}
