namespace ChessGameMultiplayer.Dto
{
    public class MoveEffectDto
    {
        public string Type { get; set; } // "Move", "Capture", etc.
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }
        public string? Piece { get; set; }
        public bool IsValid { get; set; }
    }

}
