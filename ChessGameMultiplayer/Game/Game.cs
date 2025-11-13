using ChessGameMultiplayer.Controllers;
using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.Attack;
using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;
using ChessGameMultiplayer.Game.Moves;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Security.Principal;

namespace ChessGameMultiplayer.Game
{
    public class Game
    {
        public ChessBoard Board { get; }
        public Dictionary<ChessPiece, PieceAttack> AttackedSquaresByPiece { get; private set; }

        ChessPieceColor currentTurn = ChessPieceColor.White;

        public Game()
        {
            Board = new ChessBoard();
            AttackedSquaresByPiece = new Dictionary<ChessPiece, PieceAttack>();

        }

        public virtual MoveResult MoveIfValid(MoveRequest request)
        {

            if (!IsMovePossible(request, out string errorMessage) || MoveForbidden(request))
            {
                Console.WriteLine(errorMessage);
                return new MoveResult
                {
                    IsValid = false,
                    ErrorMessage = errorMessage
                };
            }

            var from = request.From;
            var to = request.To;
            var captured = Board.GetPieceAt(to);
            ChessPiece movingPiece = Board.GetPieceAt(from);

            if (MoveNotEndangersOwnKing(from, to))
            {
                Board.MovePiece(from, to);
                //treba najprv removnut piece attack captured piece pred tym nez sa vykona update afeknutych piece attacks lebo ked je piece vyhodeny piecom ktory attackoval tak sa updatne jeho piece attack aj ked bol captured
                ClearCapturedPiece(captured);
                Board.UpdateAllPieceAttacksOnSquare(from);
                Board.UpdateAllPieceAttacksOnSquare(to);
                UpdatePawnAttackedSquares(to, captured);

                CheckStaleMate(movingPiece.Color);
                if (movingPiece is King)
                {
                    UpdatePieceAttackSliding();
                }
                else
                {
                    CheckForKingsCheckOrCheckmate();
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

        private void CheckStaleMate(ChessPieceColor friendlyColor)
        {
            //enemy king shold not be in check
            King enemyKing = friendlyColor == ChessPieceColor.White ? (King)Board.GetPieceAt(Board.BlackKingPos) : (King)Board.GetPieceAt(Board.WhiteKingPos);
            if(GetNumberOfEnemyAttackedSequencesOnPiece(enemyKing) > 0)
            {
                Console.WriteLine("Not stalemate, king is in check.");
                return;
            }

            //prejst vsetky pieces a pozriet ci maju validny move a zaroven neohrozia vlastneho krala
            foreach (var (piece, attack) in AttackedSquaresByPiece)
            {
               if(piece.Color != friendlyColor)
                {
                    if(piece is Pawn) {                         
                        List<Square> moves = ((Pawn)piece).GetPossibleMoves(Board, Board.GetPiecePosition(piece));
                        foreach(Square square in moves)
                        {
                            Position piecePos = Board.GetPiecePosition(piece);
                            Position targetPos = Board.squarePositions[square];
                            if(piece.IsValidMove(Board, piecePos, targetPos) && MoveNotEndangersOwnKing(piecePos, targetPos))
                            {
                                return;
                            }
                        }
                        continue;
                    }
                    List<Square> possibleMoves = attack.Squares;
                    foreach(Square square in possibleMoves)
                    {
                        Position piecePos = Board.GetPiecePosition(piece);
                        Position targetPos = Board.squarePositions[square];
                        if(piece.IsValidMove(Board, piecePos, targetPos) && MoveNotEndangersOwnKing(piecePos, targetPos))
                        {
                            return;
                        }
                    }
                }
            }
            Console.WriteLine("STALEMATE!");
        }

        private bool MoveForbidden(MoveRequest request)
        {
            //ci je kral checknuty - jeho square je attacknuta
            ChessPiece movingPiece = Board.GetPieceAt(request.From);
            King king = movingPiece.Color == ChessPieceColor.White ? (King)Board.GetPieceAt(Board.WhiteKingPos) : (King)Board.GetPieceAt(Board.BlackKingPos);
            int numberOfAttacksOnKing = GetNumberOfEnemyAttackedSequencesOnPiece(king);
            if (numberOfAttacksOnKing == 0)
            {
                return false;
            }
            if(numberOfAttacksOnKing >= 2 && movingPiece is not King)
            {
                Console.WriteLine("Move forbidden: king is double checked.");
                return true;
            }
            if(movingPiece is King && !IsSquareAttackedByEnemy(request.To, movingPiece.Color))
            {
                return false;
            }
            
            PieceAttack attack = null;
            foreach (PieceAttack pieceAttack in Board.GetSquare(Board.GetPiecePosition(king)).PieceAttacks)
            {
                if (pieceAttack.Piece.Color != movingPiece.Color)
                {
                    attack = pieceAttack;
                    break;
                }
            }
            if (movingPiece is not King && attack != null)
            {
                if (attack is PieceAttackSliding slidingAttack)
                {
                    List<Square> attackedSquares = slidingAttack.KingAttackingSequence;
                    Position attackerPos = Board.GetPiecePosition(slidingAttack.Piece); 
                    if(InTheLineOfDefense(attackedSquares, request.To, attackerPos))
                    {
                        return false;
                    }
                    Console.WriteLine("Move forbidden: You must block or capture attacking piece");
                    return true;
                } else
                {
                    Position attackerPos = Board.GetPiecePosition(attack.Piece);
                    if (request.To.X == attackerPos.X && request.To.Y == attackerPos.Y)
                    {
                        return false;
                    }
                    Console.WriteLine("Move forbidden: You must capture attacking piece");
                    return true;
                }
            }
            Console.WriteLine("Move forbidden: check is not resolved");
            return true;

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


        private bool MoveNotEndangersOwnKing(Position from, Position to)
        {
            Console.WriteLine("Checking if move endangers own king...");
            ChessPiece movingPiece = Board.GetPieceAt(from);

            if(movingPiece is King && IsSquareAttackedByEnemy(to, movingPiece.Color))
            {
                Console.WriteLine("Move endangers own king because destination square is attacked by enemy.");
                return false;
            }

            foreach (var (piece, attack) in AttackedSquaresByPiece)
            {
                if (piece is ChessPieceSlidingAttacker slidingAttacker)
                {
                    PieceAttackSliding pieceAttackSliding = (PieceAttackSliding)attack;
                    //piece color == current turn
                    if (pieceAttackSliding.AimsAtKing() && movingPiece.Color != pieceAttackSliding.Piece.Color)
                    {
                        //Check if moving piece from 'from' to 'to' blocks the attack sequence
                        Console.WriteLine("Piece: " + piece.GetType() + " aims at king.");
                        List<Square> attackSequence = pieceAttackSliding.AimAtKingSequence;
                        bool blocksAttack = false;
                        bool onlyDefender = true;
                        foreach (Square square in attackSequence)
                        {
                            if (square.Piece != null)
                            {
                                if (square.Piece == movingPiece)
                                {
                                    blocksAttack = true;
                                }
                                else
                                {
                                    onlyDefender = false;
                                }
                            }
                        }
                        if (blocksAttack && onlyDefender && !InTheLineOfDefense(attackSequence, to, Board.GetPiecePosition(piece)))
                        {
                            Console.WriteLine("Move of piece: " + movingPiece.GetType() + " endangers own king.");
                            return false;
                        }
                        /*else if (blocksAttack)
                        {
                            Console.WriteLine("Move of piece: " + movingPiece.GetType() + " does not endanger own king.");
                            return true;
                        }*/
                    }
                }
            }
            return true;
        }

        private bool InTheLineOfDefense(List<Square> squares, Position posTo, Position posAttacker)
        {
            if(posTo.X == posAttacker.X && posTo.Y == posAttacker.Y)
            {
                return true;
            }
            foreach (Square square in squares)
            {
                Position squarePos = Board.squarePositions[square];
                if (squarePos.X == posTo.X && squarePos.Y == posTo.Y)
                {
                    return true;
                }
            }
            return false;
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

        private void CheckForKingsCheckOrCheckmate()
        {
            //Check if the move puts the king in check or causes a checkmate

            Square whiteKingSquare = Board.GetSquare(Board.WhiteKingPos);
            foreach (PieceAttack sequences in whiteKingSquare.PieceAttacks)
            {
                if (sequences.Piece.Color == ChessPieceColor.Black)
                {
                    Console.WriteLine("White king is in check!");
                    CheckForCheckmate(Board.WhiteKingPos);
                    //if its checkmate, send it to the client
                    //if not, send check to client
                }
            }

            Square blackKingSquare = Board.GetSquare(Board.BlackKingPos);
            Console.WriteLine("Black king position X: " + Board.BlackKingPos.X + " Y: " + Board.BlackKingPos.Y);
            foreach (PieceAttack sequences in blackKingSquare.PieceAttacks)
            {

                if (sequences.Piece.Color == ChessPieceColor.White)
                {
                    Console.WriteLine("Black king is in check!");
                    CheckForCheckmate(Board.BlackKingPos);
                }
            }
        }

        private void CheckForCheckmate(Position kingPosition)
        {
            King king = (King) Board.GetPieceAt(kingPosition);
            List<Square> possibleMoves = king.GetPossibleMoves(Board, kingPosition);
            bool kingCanMove = false;
            //we check each square where king can move if it is attacked by opposite color piece
            foreach(Square square in possibleMoves)
            {
                List<PieceAttack> attackedSquares = square.PieceAttacks;
                bool safeSquare = true;
                foreach(PieceAttack attackedSquaresSequence in attackedSquares)
                {
                    if(attackedSquaresSequence.Piece.Color != king.Color)
                    {
                        Console.WriteLine("Possible move at is attacked by: " + attackedSquaresSequence.Piece.GetType());
                        Console.WriteLine("At X: " + Board.squarePositions[square].X + Board.squarePositions[square].Y + " Y");
                        safeSquare = false;
                        
                        //
                        //break;
                    }
                }
                if(safeSquare)
                {
                    Console.WriteLine("Possible move, square is safe");
                    kingCanMove = true;
                    break;
                }
            }

            bool attackBlockable = false;
            //kingCanMove && GetNumberOfEnemyAttackedSequencesOnSquare(Board.getSquareAt(kingPoisition) > 1 -> checkmate lebo 1 move nemoze blocknut 2+ attackov
           // if(!kingCanMove)
            //{
                if (GetNumberOfEnemyAttackedSequencesOnPiece(king) <= 1 && IsAttackOnKingBlockable(king))
                {
                    Console.WriteLine("ATTACK BLOCKABLE");
                    attackBlockable = true;
                } else {
                    Console.WriteLine("ATTACK NOT BLOCKABLE");
                }

                if(!kingCanMove && !attackBlockable) { Console.WriteLine("CHECKMATE"); }
                else { Console.WriteLine("NOT CHECKMATE"); }
          //  }

            //tu potom treba najst ci existuje friendly piece, ktory sa moze postavit na square 
            //ktory je v attacked square sequence piecu ktory ho attackuje a pritom je blizsie k
            //attacking piecu ako friendly kral, chebysevova vzdialenost, ak je, neni checkmate
            //takisto treba skontrolovat ci sa attacking piece neda vyhodit, ak hej, neni checkmate
        }

        private bool IsAttackOnKingBlockable(King king)
        {

            bool blockable = false;

            PieceAttack pieceAttack = GetEnemyPieceAttack(king);
            Position attackingPiecePosition = Board.GetPiecePosition(pieceAttack.Piece);
            Console.WriteLine("Piece attacking king: " + pieceAttack.Piece.GetType());

            List<Square> squaresToBlock = new List<Square>();
            squaresToBlock.Add(Board.GetSquare(attackingPiecePosition));

            if(pieceAttack is PieceAttackSliding)
            {
               squaresToBlock.AddRange(((PieceAttackSliding)pieceAttack).KingAttackingSequence);
            }

            Console.WriteLine("Number of pieces is: " + AttackedSquaresByPiece.Count);
            int count = 1;

            foreach (var (piece, attack) in AttackedSquaresByPiece)
            {
                if (piece is King || piece.Color == pieceAttack.Piece.Color) continue;

                Console.WriteLine("Piece " + count + " : " + piece.GetType() + " on position " + Board.GetPiecePosition(piece).X + ", " + Board.GetPiecePosition(piece).Y);
                count++;

                List<Square> possibleMoves = AttackedSquaresByPiece[piece].Squares;

                if (piece is Pawn)
                {
                    possibleMoves = ((Pawn)piece).GetPossibleMoves(Board, Board.GetPiecePosition(piece));
                }

                //traverse squares, check if piece attack can get onto one of these
                foreach(Square possibleMove in possibleMoves)
                {
                    foreach (Square squareToBlock in squaresToBlock)
                    {
                        if(possibleMove == squareToBlock)
                        {
                            //ak je pawn tak treba skontrolovat ci vyhodenie takisto blokne 
                            Console.WriteLine("Attack can be blocked by: " + piece.Color + " " + piece.GetType() + " at: " + Board.squarePositions[possibleMove].X + ", " + Board.squarePositions[possibleMove].Y);

                            //mozeme dat rovno return a nemusime pocitat dalsie
                            blockable = true;
                        }
                    }
                }
            }
            return blockable;


        }

        private PieceAttack GetEnemyPieceAttack(ChessPiece piece)
        {
            Square square = Board.GetSquare(Board.GetPiecePosition(piece));
            foreach (PieceAttack pieceAttack in square.PieceAttacks)
            {
                if(pieceAttack.Piece.Color != piece.Color)
                {
                    return pieceAttack;
                }
            }

            return null;
        }

        private bool IsSquareAttackedByEnemy(Position position, ChessPieceColor friendlyColor)
        {
            Square square = Board.GetSquare(position);

            foreach (PieceAttack pieceAttack in square.PieceAttacks)
            {
                if (pieceAttack.Piece.Color != friendlyColor) return true;
            }

            return false;
        }
        private int GetNumberOfEnemyAttackedSequencesOnPiece(ChessPiece piece)
        {
            Position position = Board.GetPiecePosition(piece);
            Square square = Board.GetSquare(position);
            ChessPieceColor pieceColor = piece.Color;
            int numberOfEnemyPieceAttacks = 0;

            foreach(PieceAttack pieceAttack in square.PieceAttacks)
            {
                if(pieceAttack.Piece.Color != pieceColor) numberOfEnemyPieceAttacks++;
            }

            return numberOfEnemyPieceAttacks;
        }

        private void RemoveCapturedPieceAttackingSequences(ChessPiece captured)
        {
            //If there was a capture, remove attacked squares sequence of the captured piece from all squares
            if (captured != null)
            {
                AttackedSquaresByPiece[captured].RemoveFromAllSquares();
                AttackedSquaresByPiece.Remove(captured);
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

        private bool IsMovePossible(MoveRequest request, out string errorMessage)
        {
            errorMessage = string.Empty;
            var from = request.From;
            var to = request.To;
            var piece = Board.GetPieceAt(from);
            if (piece == null)
            {
                errorMessage = "No piece at the starting position.";
                return false;
            }
            if (!piece.IsValidMove(Board, from, to))
            {
                errorMessage = "Invalid move for this piece type.";
                return false;
            }

            var captured = Board.GetPieceAt(to);

            if (captured is King)
            {
                errorMessage = "Cannot capture the king.";
                return false;
            }

            return true;
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
