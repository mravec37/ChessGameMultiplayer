namespace ChessGameMultiplayer.Game.Moves
{
    public enum MoveEffectType
    {
        Move,
        Capture,
        Castling,
        Promotion,
        EnPassant,
        //Check,
        Clear, // optional: explicitly clearing a square
    }
}
