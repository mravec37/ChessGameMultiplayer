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
            

            // HARD-CODED START POSITION (your current one)
            var piecePositions =  new List<object>
            {
            // Rank 8 (y = 0)
            new { x = 0, y = 0, piece = "B" },
            new { x = 5, y = 0, piece = "R" },
            new { x = 6, y = 0, piece = "K" },

            // Rank 7 (y = 1)
            new { x = 4, y = 1, piece = "B" },
            new { x = 5, y = 1, piece = "P" },
            new { x = 6, y = 1, piece = "P" },
            new { x = 7, y = 1, piece = "P" },

            // Rank 6 (y = 2)
            new { x = 0, y = 2, piece = "P" },
            new { x = 4, y = 2, piece = "Q" },
            new { x = 5, y = 2, piece = "N" },

            // Rank 4 (y = 4)
            new { x = 2, y = 4, piece = "n" },

            // Rank 3 (y = 5)
            new { x = 2, y = 5, piece = "p" },
            new { x = 3, y = 5, piece = "R" },
            new { x = 4, y = 5, piece = "b" },
            new { x = 6, y = 5, piece = "q" },

            // Rank 2 (y = 6)
            new { x = 0, y = 6, piece = "p" },
            new { x = 1, y = 1, piece = "p" },
            new { x = 4, y = 6, piece = "p" },
            new { x = 5, y = 6, piece = "p" },
            new { x = 7, y = 6, piece = "p" },

            // Rank 1 (y = 7)
            new { x = 2, y = 7, piece = "k" },
            new { x = 4, y = 7, piece = "r" },
            new { x = 6, y = 7, piece = "r" }
            };

            startData.PiecePositions = piecePositions;
            return startData;
        }
    }

}
