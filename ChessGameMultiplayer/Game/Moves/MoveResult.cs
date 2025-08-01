namespace ChessGameMultiplayer.Game.Moves
{
    public class MoveResult
    {
        public bool IsValid { get; set; }
        public List<MoveEffect> Affected { get; set; } = new();
        public string ErrorMessage { get; internal set; }
    }
}
