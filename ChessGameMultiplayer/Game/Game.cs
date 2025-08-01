using ChessGameMultiplayer.Controllers;
using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.Moves;

namespace ChessGameMultiplayer.Game
{
    public class Game
    {
        public ChessBoard Board { get; }
        //  private readonly ILogger<Game> _logger;
        /*public Game(ILogger<Game> logger)
        {
            _logger = logger;
            Board = new ChessBoard();
        }*/
        public Game()
        {
            Board = new ChessBoard();
        }

        internal MoveResult MoveIfValid(MoveRequest request)
        {
            //_logger.LogInformation("MoveIfValid called with request: {Request}", request);
            var from = request.From;
            var to = request.To;
            var board = Board;
            var piece = board.GetPieceAt(from);

            if (piece == null)
            {
               // _logger.LogInformation("piece is null");
                return new MoveResult
                {
                    IsValid = false,
                    ErrorMessage = "No piece at the starting position."
                };
            }
            //_logger.LogInformation("piece is not null");
            return piece.MoveIfValid(request, board);
        }
        public void NewGame()
        {
            Board.InitializeStartingPositions();
        }
    }

}
