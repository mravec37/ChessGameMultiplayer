namespace ChessGameMultiplayer.Connection
{
    public class NewGameStartData
    {
        public TimeSpan ClockTime { get; set; }
        public List<object> PiecePositions { get; set; }

        public NewGameStartData() { }
    }
}
