using ChessGameMultiplayer.Game.Attack;

namespace ChessGameMultiplayer.Game.ChessPieces
{
    public abstract class ChessPieceSlidingAttacker : ChessPiece
    {
        protected PieceAttackSliding SlidingPieceAttack;

        protected ChessPieceSlidingAttacker(ChessPieceColor color) : base(color)
        {
        }

        public void SetSlidingPieceAttack(PieceAttackSliding attack)
        {
            SlidingPieceAttack = attack;
        }


    }
}
