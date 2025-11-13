using ChessGameMultiplayer.Game.Attack;

namespace ChessGameMultiplayer.Game.ChessPieces
{
    public abstract class ChessPieceSlidingAttacker : ChessPiece
    {
        protected PieceAttackSliding SlidingPieceAttack;

        //aims at king but attack is blocked
        public bool AimsAtKing;
        //attacks king directly
        public bool AttacksKing;
        public int[] KingDirection;

        protected ChessPieceSlidingAttacker(ChessPieceColor color) : base(color)
        {
            AimsAtKing = false;
            KingDirection = new int[2] { 0, 0 };
        }

        public void SetSlidingPieceAttack(PieceAttackSliding attack)
        {
            SlidingPieceAttack = attack;
        }


    }
}
