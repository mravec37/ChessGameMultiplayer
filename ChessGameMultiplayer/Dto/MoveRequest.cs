using ChessGameMultiplayer.Game;

namespace ChessGameMultiplayer.Dto
{
    public class MoveRequest
    {
        public Position From { get; set; }
        public Position To { get; set; }
        public string Piece { get; set; }
    }

}
