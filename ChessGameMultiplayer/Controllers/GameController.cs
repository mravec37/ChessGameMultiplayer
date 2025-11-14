using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game;
using ChessGameMultiplayer.Game.Logic;
using ChessGameMultiplayer.Game.Moves;
using Microsoft.AspNetCore.Mvc;

namespace ChessGameMultiplayer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {

        private readonly GameContainer _chessEngine;
        private readonly ILogger<HomeController> _logger;
        int numOfCalls = 0;

        public GameController(GameContainer chessEngine, ILogger<HomeController> logger)
        {
            _chessEngine = chessEngine;
            _logger = logger;
        }

        /*[HttpGet("NewGame")]
        public IActionResult NewGame()
        {
            _logger.LogInformation("New game started");
            _chessEngine.NewGame();
            var pieces = new List<object>
            {
                // Black major pieces
                new { x = 0, y = 0, piece = "R" },
                new { x = 1, y = 0, piece = "N" },
                new { x = 2, y = 0, piece = "B" },
                new { x = 3, y = 0, piece = "Q" },
                new { x = 4, y = 0, piece = "K" },
                new { x = 5, y = 0, piece = "B" },
                new { x = 6, y = 0, piece = "N" },
                new { x = 7, y = 0, piece = "R" },

                // White major pieces
                new { x = 0, y = 7, piece = "r" },
                new { x = 1, y = 7, piece = "n" },
                new { x = 2, y = 7, piece = "b" },
                new { x = 3, y = 7, piece = "q" },
                new { x = 4, y = 7, piece = "k" },
                new { x = 5, y = 7, piece = "b" },
                new { x = 6, y = 7, piece = "n" },
                new { x = 7, y = 7, piece = "r" }
            };

            // Add pawns using AddRange
            pieces.AddRange(Enumerable.Range(0, 8).Select(i => new { x = i, y = 1, piece = "P" })); // Black pawns
            pieces.AddRange(Enumerable.Range(0, 8).Select(i => new { x = i, y = 6, piece = "p" })); // White pawns*/

        //return Ok(pieces);
        //}

        [HttpGet("NewGame")]
        public IActionResult NewGame()
        {
            _logger.LogInformation("New custom game started");
            _chessEngine.NewGame();

            var pieces = new List<object>
    {
        // --- Black pieces ---
        new { x = 7, y = 0, piece = "K" }, // Black King
        new { x = 1, y = 0, piece = "B" }, // Black Bishop
        new { x = 0, y = 1, piece = "P" }, // Black Pawn
        new { x = 2, y = 1, piece = "P" }, // Black Pawn

        // --- White pieces ---
        new { x = 1, y = 5, piece = "k" }, // White King
        new { x = 3, y = 2, piece = "q" }, // White Queen
        new { x = 6, y = 2, piece = "q" }, // White Queen
        new { x = 0, y = 2, piece = "p" }, // White Pawn
        new { x = 2, y = 2, piece = "p" }  // White Pawn
    };

            return Ok(pieces);
        }




        //is called 2 times for some reason, maybe razor does hot reload

        /* [HttpGet("NewGame")]
         public IActionResult NewGame()
         {
             numOfCalls++;
             Console.WriteLine("Number of new game call: " + numOfCalls);

             _logger.LogInformation("Custom test game started");
             _chessEngine.NewGame();

             var pieces = new List<object>
     {
         new { x = 2, y = 0, piece = "K" }, // Black King
         new { x = 7, y = 1, piece = "B" }, // Black Bishop
         new { x = 6, y = 3, piece = "B" }, // Black Bishop

         new { x = 2, y = 7, piece = "k" }, // White King
         new { x = 1, y = 7, piece = "r" }, // White Rook
         new { x = 3, y = 7, piece = "b" }, // White Bishop
         new { x = 1, y = 6, piece = "p" }, // White Pawn
         new { x = 7, y = 4, piece = "p" }  // White Pawn
     };

             return Ok(pieces);
         }
        */




        [HttpPost("MovePiece")]
        public IActionResult MovePiece([FromBody] MoveRequest move)
        {
            _logger.LogInformation("Move piece endpoint");
            LogMoveRequest(move);
            MoveResult moveResult =  _chessEngine.MoveIfValid(move);
            LogMoveResult(moveResult);
            var dtoList = MoveConverter.ConvertToDtoList(moveResult);
            return Ok(dtoList);
        }



        public void LogMoveResult(MoveResult result)
        {
            _logger.LogInformation($"Move Valid: {result.IsValid}");

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                _logger.LogInformation($"Error: {result.ErrorMessage}");
            }

            if (result.Affected == null || result.Affected.Count == 0)
            {
                _logger.LogInformation("No affected squares.");
                return;
            }

            _logger.LogInformation("Affected Squares:");
            foreach (var effect in result.Affected)
            {
                var pieceName = effect.Piece?.GetType().Name ?? "None";
                var pieceColor = effect.Piece?.Color.ToString() ?? "-";
                _logger.LogInformation($"  - Type: {effect.Type}, From: ({effect.from?.X},{effect.from?.Y}), To: ({effect.to?.X},{effect.to?.Y}), Piece: {pieceColor} {pieceName}");
            }
        }
        public void LogMoveRequest(MoveRequest move)
        {
            if (move == null)
            {
                _logger.LogWarning("MoveRequest is null.");
                return;
            }

            _logger.LogInformation("♟️ MoveRequest received:");
            _logger.LogInformation("  Piece: {Piece}", move.Piece);
            _logger.LogInformation("  From: ({FromX}, {FromY})", move.From?.X, move.From?.Y);
            _logger.LogInformation("  To:   ({ToX}, {ToY})", move.To?.X, move.To?.Y);
        }
    }
}
