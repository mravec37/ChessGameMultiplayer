using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.Attack;
using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;
using ChessGameMultiplayer.Game.Moves;
using System.IO.Pipelines;
using System.Text.RegularExpressions;

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
            (bool flowControl, MoveResult value) = HandleCastling(request);
            if (!flowControl)
            {
                return value;
            }

            if (!MoveValidator.IsMovePossible(request, Board) || MoveValidator.MoveForbidden(request, Board) || MoveValidator.MoveEndangersOwnKing(request, Board, AttackedSquaresByPiece))
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

            Board.MovePiece(from, to);
            // treba najprv removnut piece attack captured piece pred tym nez sa vykona update afeknutych piece attacks lebo ked je piece vyhodeny piecom ktory attackoval tak sa updatne jeho piece attack aj ked bol captured
            ClearCapturedPiece(captured);
            Board.UpdateAllPieceAttacksOnSquare(from);
            Board.UpdateAllPieceAttacksOnSquare(to);
            UpdatePawnAttackedSquares(to, captured);
            ChessPiece enPassantCapture = HandleEnPassant(from, to, movingPiece);
            PieceAttack test = AttackedSquaresByPiece[movingPiece];
            Console.WriteLine("Is moving piece in attacked squares by piece: " + test.Piece.GetType() + " Pos: " + Board.PiecesPos[test.Piece].X + ", " + Board.PiecesPos[test.Piece].Y);

            if (IsPromotionPosition(movingPiece, to))
            {
                return HandlePromotion(movingPiece, from, to);
            }
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
            if (enPassantCapture != null)
            {
                Position capturedPos = new Position(to.X, from.Y);
                effects.Add(new MoveEffect
                {
                    Type = MoveEffectType.Capture,
                    to = capturedPos,
                    Piece = enPassantCapture
                });
            }

            PrintMoveEffects(effects);
            return new MoveResult
            {
                IsValid = true,
                Affected = effects
            };
        }

        public MoveResult PromotionChoice(PromotionRequest request)
        {
            ChessPiece pawn = Board.GetPieceAt(request.pawnPosition);
            if (pawn == null || !(pawn is Pawn))
            {
                return new MoveResult
                {
                    IsValid = false,
                    ErrorMessage = "No pawn at the specified position for promotion."
                };
            }
            //toto asi netreba
            PieceAttack capturedPieceAttack = AttackedSquaresByPiece[pawn];
            capturedPieceAttack.RemoveFromAllSquares();

            AttackedSquaresByPiece.Remove(pawn);
            Board.RemovePiece(pawn);
            
            ChessPiece promotedPiece = GetPromotedPiece(request.promotionType, pawn.Color);
            Board.MovePieceToSquare(request.pawnPosition, promotedPiece);
            Board.PiecesPos[promotedPiece] = request.pawnPosition;

            PieceAttack pieceAttack = null;
            if (promotedPiece is ChessPieceSlidingAttacker)
            {
                pieceAttack = new PieceAttackSliding(promotedPiece, Board);
                //((ChessPieceSlidingAttacker)promotedPiece).SetSlidingPieceAttack((PieceAttackSliding)pieceAttack);
            }
            else
            {
                pieceAttack = new PieceAttackDirect(promotedPiece, Board);
            }

            AttackedSquaresByPiece[promotedPiece] = pieceAttack;
            pieceAttack.UpdateAttackedSquares();

            GameStateChecker.CheckStaleMate(promotedPiece.Color, Board, AttackedSquaresByPiece);
            GameStateChecker.CheckForKingsCheckOrCheckmate(Board, AttackedSquaresByPiece);

            return new MoveResult
            {
                IsValid = true,
                Affected = new List<MoveEffect>
                {
                    new MoveEffect
                    {
                        Type = MoveEffectType.Promoted,
                        to = request.pawnPosition,
                        Piece = promotedPiece
                    }
                }
            };
        }

        private ChessPiece GetPromotedPiece(String promotionType, ChessPieceColor color)
        {
            return promotionType switch
            {
               
                "Rook" => new Rook(color),
                "Knight" => new Knight(color),
                "Bishop" => new Bishop(color),
                "Queen" => new Queen(color),
            };
        }

        private (bool flowControl, MoveResult value) HandleCastling(MoveRequest request)
        {
            if (CastlingHandler.IsCastlingMove(Board, request.From, request.To))
            {
                ChessPiece rook = Board.GetPieceAt(request.To);
                MoveResult moveResult = CastlingHandler.HandleCastling(Board, request.From, request.To);
                UpdatePieceAttackSliding();
                AttackedSquaresByPiece[rook].UpdateAttackedSquares();
                return (flowControl: false, value: moveResult);
            }

            return (flowControl: true, value: null);
        }

        private MoveResult HandlePromotion(ChessPiece movingPiece, Position from, Position to)
        {
            return new MoveResult
            {
                IsValid = true,
                Affected = new List<MoveEffect>
                {
                    new MoveEffect
                    {
                        Type = MoveEffectType.Promotion,
                        to = to,
                        from = from,
                        Piece = movingPiece
                    }
                },
            };
        }

        private bool IsPromotionPosition(ChessPiece piece, Position to)
        {
            if (piece is Pawn)
            {
                if ((piece.Color == ChessPieceColor.White && to.Y == 0) || (piece.Color == ChessPieceColor.Black && to.Y == 7))
                {
                    Console.WriteLine("Pawn promotion detected for piece: " + piece.GetType() + " " + piece.Color + " at position: " + to.X + ", " + to.Y);
                    return true;
                }
            }
            return false;
        }

        private void PrintMoveEffects(List<MoveEffect> effects)
        {
            foreach (var effect in effects)
            {
                Console.WriteLine("Move Effect - Type: " + effect.Type + ", From: (" + effect.from?.X + ", " + effect.from?.Y + "), To: (" + effect.to.X + ", " + effect.to.Y + "), Piece: " + effect.Piece?.GetType().Name + " " + effect.Piece?.Color);
            }
        }

        private ChessPiece HandleEnPassant(Position from, Position to, ChessPiece? movingPiece)
        {
            if (movingPiece is Pawn movingPawn)
            {
                //Check if pawn moved two squares forward
                if (Math.Abs(to.Y - from.Y) == 2)
                {
                    Board.enPassantPos[movingPawn] = new Position(to.X, (from.Y + to.Y) / 2);
                }
                else
                {
                    //Check if pawn captured en passant
                    foreach (var kvp in Board.enPassantPos)
                    {
                        Pawn enPassantPawn = kvp.Key;
                        Position enPassantGhostPos = kvp.Value;
                        if (to.X == enPassantGhostPos.X && to.Y == enPassantGhostPos.Y && Math.Abs(to.X - from.X) == 1 && to.Y == from.Y + (movingPawn.Color == ChessPieceColor.White ? -1 : 1))
                        {
                            Console.WriteLine("En passant capture of piece: " + enPassantPawn.GetType() + " " + enPassantPawn.Color + " at position: " + Board.GetPiecePosition(enPassantPawn).X + ", " + Board.GetPiecePosition(enPassantPawn).Y);
                            Position capturedPawnPos = new Position(to.X, from.Y);
                            //ChessPiece capturedPawn = Board.GetPieceAt(capturedPawnPos);
                            ClearCapturedPiece(enPassantPawn);
                            //Board.MovePiece(capturedPawnPos, new Position(-1, -1)); // Remove captured pawn from the board
                            Board.RemovePieceFromSquare(capturedPawnPos);
                            return enPassantPawn;
                        }
                    }
                }
            }
            //Clear en passant positions for all pawns of the opposite color
            var pawnsToClear = Board.enPassantPos.Keys.Where(p => p.Color != movingPiece.Color).ToList();
            foreach (var pawn in pawnsToClear)
            {
                Board.enPassantPos.Remove(pawn);
            }
            return null;
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

                Board.RemovePiece(captured);
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
                    ((ChessPieceSlidingAttacker)piece).SetSlidingPieceAttack((PieceAttackSliding)pieceAttack);
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
