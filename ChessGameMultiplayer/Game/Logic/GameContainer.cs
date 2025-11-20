using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.Moves;

namespace ChessGameMultiplayer.Game.Logic
{
    public class GameContainer
    {
        public ChessEngine Game { get; }

        public GameContainer()
        {
            Game = new ChessEngine();
        }

        public MoveResult MoveIfValid(MoveRequest request)
        {
           
            return Game.MoveIfValid(request);
        }

        public void NewGame()
        {
            Game.NewGame();
        }

        public MoveResult PromotionChoice(PromotionRequest request)
        {
            return Game.PromotionChoice(request);
        }
    }
}
