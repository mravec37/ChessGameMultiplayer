using ChessGameMultiplayer.Connection.Hubs;
using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game;
using ChessGameMultiplayer.Game.ChessPieces;
using ChessGameMultiplayer.Game.Logic;
using ChessGameMultiplayer.Game.Moves;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using System.Threading.Tasks;

namespace ChessGameMultiplayer.Connection
{
    public class GameManager
    {
        public GameContainer GameContainer { get; set; }

        private readonly IHubContext<GameHub> hubContext;
        private readonly ConnectionStore connections;

        public GameManager(GameContainer gameContainer, IHubContext<GameHub> hubContext, ConnectionStore connections)
        {
            GameContainer = gameContainer;
            this.hubContext = hubContext;
            this.connections = connections;
            gameContainer.gameClock.TimeExpired += GameClock_TimeExpired;
            gameContainer.CheckmateEvent += GameContainer_CheckmateEvent;
            gameContainer.StalemateEvent += GameContainer_StalemateEvent;
        }

        private async void GameContainer_StalemateEvent(object? sender, EventArgs e)
        {
            var gameEndData = new
            {
                gameEndEvent = "Stalemate"
            };

            if (connections.PlayerOne != null)
            {
                await hubContext.Clients
                    .Client(connections.PlayerOne)
                    .SendAsync("GameEnded", gameEndData);
            }

            if (connections.PlayerTwo != null)
            {
                await hubContext.Clients
                    .Client(connections.PlayerTwo)
                    .SendAsync("GameEnded", gameEndData);
            }
        }

        private async void GameContainer_CheckmateEvent(object? sender, EventArgs e)
        {
            Console.WriteLine("Checkmate event triggered");
            if (e is not CheckmateEventArgs args)
                return;

            var gameEndData = new
            {
                winner = args.Winner,
                gameEndEvent = "Checkmate"
            };

            if (connections.PlayerOne != null)
            {
                await hubContext.Clients
                    .Client(connections.PlayerOne)
                    .SendAsync("GameEnded", gameEndData);
            }

            if (connections.PlayerTwo != null)
            {
                await hubContext.Clients
                    .Client(connections.PlayerTwo)
                    .SendAsync("GameEnded", gameEndData);
            }
        }

        private async void GameClock_TimeExpired(object? sender, EventArgs e)
        {
            if (e is not TimeExpiredEventArgs args)
                return;

            if(GameContainer.gameEnd) 
                return; 

            ChessPieceColor winner =
                args.ExpiredTimePlayerColor == ChessPieceColor.White
                    ? ChessPieceColor.Black
                    : ChessPieceColor.White;

            var gameEndData = new
            {
                winner = winner.ToString(),
                gameEndEvent = "Time"
            };

            Console.WriteLine(winner.ToString() + " player won!");

            if (connections.PlayerOne != null)
            {
                await hubContext.Clients
                    .Client(connections.PlayerOne)
                    .SendAsync("GameEnded", gameEndData);
            }

            if (connections.PlayerTwo != null)
            {
                await hubContext.Clients
                    .Client(connections.PlayerTwo)
                    .SendAsync("GameEnded", gameEndData);
            }
        }

        public async Task PromotionChoice(PromotionRequest request)
        {
            MoveResult promotionResult = GameContainer.PromotionChoice(request);

            List<MoveEffectDto> dtoList = MoveConverter.ConvertEffectsToDto(promotionResult);
            MoveResultDto resultDto = new MoveResultDto();
            resultDto.Affected = dtoList;
            resultDto.IsValid = promotionResult.IsValid;
            resultDto.ErrorMessage = promotionResult.ErrorMessage;
            resultDto.RemainingTime = (int)promotionResult.RemainingTime.TotalMilliseconds;

            await hubContext.Clients
                   .All
                   .SendAsync("Promoted", resultDto);


        }

        //game starter class
        public NewGameStartData StartNewGame()
        {
           NewGameStartData startData = GameContainer.NewGame();


            // HARD-CODED START POSITION
            var piecePositions = new List<object>
{
    // Black pieces
    new { x = 0, y = 0, piece = "K" }, // black king
    new { x = 5, y = 0, piece = "Q" }, // black queen
    new { x = 5, y = 1, piece = "P" }, // black pawn

    // White king
    new { x = 6, y = 4, piece = "k" },

    // White pawns
    new { x = 6, y = 3, piece = "p" },
    new { x = 7, y = 3, piece = "p" },

    new { x = 5, y = 4, piece = "p" },
    new { x = 7, y = 4, piece = "p" },

    new { x = 5, y = 5, piece = "p" },
    new { x = 6, y = 5, piece = "p" },
    new { x = 7, y = 5, piece = "p" },

    new { x = 0, y = 6, piece = "p" }
};

            startData.PiecePositions = piecePositions;
            return startData;

            /*   var piecePositions = new List<object>
   {
       // Rank 8 (y = 0)
       new { x = 6, y = 0, piece = "K" },

       // Rank 7 (y = 1)
       new { x = 4, y = 1, piece = "B" },
       new { x = 5, y = 1, piece = "P" },
       new { x = 6, y = 1, piece = "P" },
       new { x = 7, y = 1, piece = "P" },

       // Rank 1 (y = 7)
       new { x = 1, y = 7, piece = "k" },
       new { x = 6, y = 7, piece = "r" }
   };

               startData.PiecePositions = piecePositions;
               return startData;*/

        }
    }

}
