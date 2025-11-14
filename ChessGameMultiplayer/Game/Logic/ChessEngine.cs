using ChessGameMultiplayer.Controllers;
using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.Attack;
using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;
using ChessGameMultiplayer.Game.Moves;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Security.Principal;

namespace ChessGameMultiplayer.Game.Logic
{
    public class ChessEngine
    {
        public ChessBoard Board { get; }
        public Dictionary<ChessPiece, PieceAttack> AttackedSquaresByPiece { get; private set; }

        ChessPieceColor currentTurn = ChessPieceColor.White;

        public ChessEngine()
        {
            Board = new ChessBoard();
            AttackedSquaresByPiece = new Dictionary<ChessPiece, PieceAttack>();

        }

        public virtual MoveResult MoveIfValid(MoveRequest request)
        {


            //moves budu checknute move validator ktory bude vyuzivat threat analyzer
            //dalsia trieda bude checkovat game state ako check, checkmate, stalemate


            if (!MoveValidator.IsMovePossible(request, Board) || MoveValidator.MoveForbidden(request, Board))
            {
                return new MoveResult
                {
                    IsValid = false,
                    ErrorMessage = "Invalid move"
                };
            }

            var from = request.From;
            var to = request.To;
            var captured = Board.GetPieceAt(to);
            ChessPiece movingPiece = Board.GetPieceAt(from);

            if (MoveValidator.MoveNotEndangersOwnKing(request, Board, AttackedSquaresByPiece))
            {
                Board.MovePiece(from, to);
                //treba najprv removnut piece attack captured piece pred tym nez sa vykona update afeknutych piece attacks lebo ked je piece vyhodeny piecom ktory attackoval tak sa updatne jeho piece attack aj ked bol captured
                ClearCapturedPiece(captured);
                Board.UpdateAllPieceAttacksOnSquare(from);
                Board.UpdateAllPieceAttacksOnSquare(to);
                UpdatePawnAttackedSquares(to, captured);

                GameStateChecker.CheckStaleMate(movingPiece.Color, Board, AttackedSquaresByPiece);
                if (movingPiece is King)
                {
                    UpdatePieceAttackSliding();
                }
                else
                {
                    GameStateChecker.CheckForKingsCheckOrCheckmate(Board, AttackedSquaresByPiece);
                }

                var effects = CreateMoveEffects(from, to, captured);

                return new MoveResult
                {
                    IsValid = true,
                    Affected = effects
                };
            }
            else
            {
                return new MoveResult
                {
                    IsValid = false,
                    ErrorMessage = "Move endangers own king."
                };
            }
        }

        private void UpdatePieceAttackSliding()
        {
            foreach (var (piece, attack) in AttackedSquaresByPiece)
            {
                if (attack is PieceAttackSliding slidingAttack)
                {
                    slidingAttack.UpdateAttackedSquares();
                }
            }
        }
        private void ClearCapturedPiece(ChessPiece? captured)
        {
            if (captured != null)
            {
                PieceAttack capturedPieceAttack = AttackedSquaresByPiece[captured];
                capturedPieceAttack.RemoveFromAllSquares();
                AttackedSquaresByPiece.Remove(captured);

                Board.RemoveCapturedPiece(captured);
            }
        }
        private void UpdatePawnAttackedSquares(Position to, ChessPiece captured)
        {
            ChessPiece piece = Board.GetPieceAt(to);
            //If move is allowed for pawn and it didnt capture, we must manually trigger update for attacked squares 
            //because it moves to a position which it didnt attack
            if (piece is Pawn && captured == null)
            {
                AttackedSquaresByPiece[piece].UpdateAttackedSquares();
            }
        }

        protected virtual List<MoveEffect> CreateMoveEffects(Position from, Position to, ChessPiece? captured)
        {
            var effects = new List<MoveEffect>();

            if (captured != null)
            {
                effects.Add(new MoveEffect
                {
                    Type = MoveEffectType.Capture,
                    to = to,
                    Piece = captured
                });
            }

            effects.Add(new MoveEffect
            {
                Type = MoveEffectType.Move,
                to = to,
                from = from,
                //Piece = this
            });
            //if king is checked, add a check effect

            return effects;
        }

        public void NewGame()
        {
            List<ChessPiece> pieces = Board.InitializeStartingPositions();
            foreach (var piece in pieces)
            {
                PieceAttack pieceAttack = null;
                if (piece is ChessPieceSlidingAttacker)
                {
                    pieceAttack = new PieceAttackSliding(piece, Board);
                    ((ChessPieceSlidingAttacker)piece).SetSlidingPieceAttack((PieceAttackSliding) pieceAttack);
                }
                else
                {
                   pieceAttack = new PieceAttackDirect(piece, Board);
                }

                AttackedSquaresByPiece[piece] = pieceAttack;
                pieceAttack.UpdateAttackedSquares();
            }
        }


        /*public void NewGame()
         {
            List<ChessPiece> pieces = Board.InitializeStartingPositions();
            foreach (var piece in pieces)
            {
                if(piece.Color == ChessPieceColor.White)
                {
                    var sequence = new AttackedSquaresSequence(piece, Board);
                    WhiteAttackedSquaresSequences.Add(sequence);
                    sequence.UpdateAttackedSquares();

                }
                else
                {
                    var sequence = new AttackedSquaresSequence(piece, Board);
                    BlackAttackedSquaresSequences.Add(sequence);
                    sequence.UpdateAttackedSquares();
                }
            }
             
         }*/

    }
}
