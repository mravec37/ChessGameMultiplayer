using ChessGameMultiplayer.Game;
using ChessGameMultiplayer.Game.Moves;

namespace ChessGameMultiplayer.Dto
{
    public class MoveResultDto
    {
        public bool IsValid { get; set; }
        public List<MoveEffectDto> Affected { get; set; } = new();
        public string ErrorMessage { get; internal set; }
        public int RemainingTime { get; set; }

        public List<Position> MoveSquares { get; set; } = new();
    }
}
