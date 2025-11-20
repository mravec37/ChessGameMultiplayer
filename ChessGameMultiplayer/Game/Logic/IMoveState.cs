namespace ChessGameMultiplayer.Game.Logic
{
    public interface IMoveState
    {
        public bool HasMoved();
        public void MarkAsMoved();
    }
}
