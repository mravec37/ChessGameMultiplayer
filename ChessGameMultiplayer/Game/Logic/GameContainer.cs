using ChessGameMultiplayer.Connection;
using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.ChessPieces;
using ChessGameMultiplayer.Game.Moves;

namespace ChessGameMultiplayer.Game.Logic
{
    public class GameContainer
    {
        private ChessEngine Game;
        private ChessPieceColor currentTurn = ChessPieceColor.White;
        private bool promotion = false;
        public GameClock gameClock;
        public event EventHandler? CheckmateEvent;
        private bool gameEnd = false;
        public GameContainer()
        {
           Game = new ChessEngine();
           gameClock = new GameClock(); 
        }

        public MoveResult MoveIfValid(MoveRequest request)
        {   if(promotion || gameEnd) {
                return new MoveResult { IsValid = false, ErrorMessage = "Cant move in this game state" };
            }
            Console.WriteLine("Current players turn: " + currentTurn.ToString());
            if(!MoveValidator.CurrentTurnPlayerMove(request, currentTurn)) 
            {
                return new MoveResult { IsValid = false, ErrorMessage = "Not player's turn" };
            }

            var moveResult = Game.MoveIfValid(request);
           
            if (moveResult.IsValid)
            {
                if (GameEnd(moveResult))
                {
                    gameEnd = true;
                }
                if (moveResult.Affected[0].Type == MoveEffectType.PROMOTION)
                {
                    promotion = true;
                }
                else
                {
                    moveResult.RemainingTime = gameClock.SwitchTurnCountdown();
                    currentTurn = MoveConverter.GetOppositeColor(currentTurn);
                }
            }
            return moveResult;
        }

        private bool GameEnd(MoveResult moveResult)
        {
            foreach (var moveEffect in moveResult.Affected)
            {
                if(moveEffect.Type == MoveEffectType.CHECKMATE)
                {
                    Console.WriteLine("Checkmate move effect type detected");
                    CheckmateEvent?.Invoke(this, new CheckmateEventArgs(currentTurn));
                    return true;
                }
            }
            return false;
        }

        public NewGameStartData NewGame()
        {
            Game.NewGame();
            gameClock.StartGameCountdown();
            return new NewGameStartData {ClockTime = gameClock.ClockStartTime};
        }

        public MoveResult PromotionChoice(PromotionRequest request)
        {
            var moveResult = Game.PromotionChoice(request);
            moveResult.RemainingTime = gameClock.SwitchTurnCountdown();
            currentTurn = MoveConverter.GetOppositeColor(currentTurn);
            Console.WriteLine("Current turn: " + currentTurn.ToString());
            promotion = false;
            return moveResult;
        }
    }
}
