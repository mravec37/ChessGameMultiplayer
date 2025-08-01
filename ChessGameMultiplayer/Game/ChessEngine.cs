using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.Moves;

namespace ChessGameMultiplayer.Game
{
    public class ChessEngine
    {
        public Game Game { get; }

        public ChessEngine()
        {
            Game = new Game();
        }

        public MoveResult MoveIfValid(MoveRequest request)
        {
           
            return Game.MoveIfValid(request);
        }

        public void NewGame()
        {
            Game.NewGame();
        }
    }
}
