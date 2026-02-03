namespace ChessGameMultiplayer.Connection.Hubs
{
    using ChessGameMultiplayer.Dto;
    using ChessGameMultiplayer.Game;
    using ChessGameMultiplayer.Game.ChessPieces;
    using ChessGameMultiplayer.Game.Moves;
    using Microsoft.AspNetCore.SignalR;
    public class GameHub : Hub
    {
        private ConnectionStore _connections;
        private GameManager gameManager;

        public GameHub(ConnectionStore connections, GameManager gameManager)
        {
            _connections = connections;
            this.gameManager = gameManager;
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");

            _connections.Add(Context.ConnectionId);

            if (_connections.BothPlayersConnected)
            {
                NewGameStartData startData = gameManager.StartNewGame();
                var pieces = startData.PiecePositions;
                await Clients.Client(_connections.PlayerOne)
                    .SendAsync("GameStarted", new
                    {
                        color = "White",
                        pieces,
                        clockTime = startData.ClockTime.TotalMilliseconds
                    });

                // Notify Player 2
                await Clients.Client(_connections.PlayerTwo)
                    .SendAsync("GameStarted", new
                    {
                        color = "Black",
                        pieces,
                        clockTime = startData.ClockTime.TotalMilliseconds
                    });
            }
            await base.OnConnectedAsync();
        }

       /* private async void GameClock_TimeExpired(object? sender, EventArgs e)
        {
            var timeExpiredArgs = e as TimeExpiredEventArgs;
            if (timeExpiredArgs == null) return;

            ChessPieceColor winner = timeExpiredArgs.ExpiredPlayerColor == ChessPieceColor.Black ? ChessPieceColor.White : ChessPieceColor.Black;
            var gameEndData = new
            {
                winner = winner.ToString(),
                gameEndEvent = "Time"
            };
            if (_connections.PlayerOne != null)
            {
                //mozno hub uz neexistuje treba to spravit nejak inak
                await Clients.Client(_connections.PlayerOne)
                    .SendAsync("GameEnded", gameEndData);
            }

            if (_connections.PlayerTwo != null)
            {
                await Clients.Client(_connections.PlayerTwo)
                    .SendAsync("GameEnded", gameEndData);
            }
        }*/

        public async Task SendMove(MoveRequest moveRequest)
        {
            var result = gameManager.GameContainer.MoveIfValid(moveRequest);
          
            if (!result.IsValid)
            {
                // Only notify the sender
                await Clients.Caller.SendAsync("MoveRejected", result.ErrorMessage);
                return;
            }

            List<MoveEffectDto> dtoList = MoveConverter.ConvertEffectsToDto(result);
            MoveResultDto resultDto = new MoveResultDto();
            resultDto.Affected = dtoList;
            resultDto.IsValid = result.IsValid;
            resultDto.ErrorMessage = result.ErrorMessage;
            resultDto.RemainingTime = (int) result.RemainingTime.TotalMilliseconds;
            //stop timer for current player and start timer for other player and send time information with move applied
            // Broadcast to both players explicitly
            if (_connections.PlayerOne != null)
            {
                await Clients.Client(_connections.PlayerOne)
                    .SendAsync("MoveApplied", resultDto);
            }

            if (_connections.PlayerTwo != null)
            {
                await Clients.Client(_connections.PlayerTwo)
                    .SendAsync("MoveApplied", resultDto);
            }
        }


        public async Task PromotionChoice(PromotionRequest promotionRequest)
        {
            Console.WriteLine("Promotion choice");
            Console.WriteLine("Promotion type: " + promotionRequest.promotionType);
            gameManager.PromotionChoice(promotionRequest);
        }
    }

}
