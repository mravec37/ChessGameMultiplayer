using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.Attack;
using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game.Logic
{
    public static class GameStateChecker
    {
        public static void CheckStaleMate(ChessPieceColor friendlyColor, ChessBoard board, Dictionary<ChessPiece, PieceAttack> attackedSquaresByPiece)
        {
            //enemy king shold not be in check
            King enemyKing = friendlyColor == ChessPieceColor.White ? (King)board.GetPieceAt(board.BlackKingPos) : (King)board.GetPieceAt(board.WhiteKingPos);
            if (ThreatAnalyzer.GetNumberOfEnemyAttackedSequencesOnPiece(enemyKing, board) > 0)
            {
                Console.WriteLine("Not stalemate, king is in check.");
                return;
            }

            //prejst vsetky pieces a pozriet ci maju validny move a zaroven neohrozia vlastneho krala
            foreach (var (piece, attack) in attackedSquaresByPiece)
            {
                if (piece.Color != friendlyColor)
                {
                    if (piece is Pawn)
                    {
                        List<Square> moves = ((Pawn)piece).GetPossibleMoves(board, board.GetPiecePosition(piece));
                        foreach (Square square in moves)
                        {
                            Position piecePos = board.GetPiecePosition(piece);
                            Position targetPos = board.squarePositions[square];
                            MoveRequest moveRequest = new MoveRequest
                            {
                                From = piecePos,
                                To = targetPos
                            };
                            if (piece.IsValidMove(board, piecePos, targetPos) && !MoveValidator.MoveEndangersOwnKing(moveRequest, board, attackedSquaresByPiece))
                            {
                                Console.WriteLine("Not stalemate, piece can move.");
                                return;
                            }
                        }
                        continue;
                    }
                    List<Square> possibleMoves = attack.Squares;
                    foreach (Square square in possibleMoves)
                    {
                        Position piecePos = board.GetPiecePosition(piece);
                        Position targetPos = board.squarePositions[square];
                        MoveRequest moveRequest = new MoveRequest
                        {
                            From = piecePos,
                            To = targetPos
                        };
                        if (piece.IsValidMove(board, piecePos, targetPos) && !MoveValidator.MoveEndangersOwnKing(moveRequest, board, attackedSquaresByPiece))
                        {
                            Console.WriteLine("Not stalemate, piece can move.");
                            return;
                        }
                    }
                }
            }
            Console.WriteLine("STALEMATE!");
        }
        public static void CheckForKingsCheckOrCheckmate(ChessBoard board, Dictionary<ChessPiece, PieceAttack> attackedSquaresByPiece)
        {
            //Check if the move puts the king in check or causes a checkmate

            Square whiteKingSquare = board.GetSquare(board.WhiteKingPos);
            foreach (PieceAttack sequences in whiteKingSquare.PieceAttacks)
            {
                if (sequences.Piece.Color == ChessPieceColor.Black)
                {
                    Console.WriteLine("White king is in check!");
                    CheckForCheckmate(board.WhiteKingPos, board, attackedSquaresByPiece);
                    //if its checkmate, send it to the client
                    //if not, send check to client
                }
            }

            Square blackKingSquare = board.GetSquare(board.BlackKingPos);
            Console.WriteLine("Black king position X: " + board.BlackKingPos.X + " Y: " + board.BlackKingPos.Y);
            foreach (PieceAttack sequences in blackKingSquare.PieceAttacks)
            {

                if (sequences.Piece.Color == ChessPieceColor.White)
                {
                    Console.WriteLine("Black king is in check!");
                    CheckForCheckmate(board.BlackKingPos, board, attackedSquaresByPiece);
                }
            }
        }

        public static void CheckForCheckmate(Position kingPosition, ChessBoard board, Dictionary<ChessPiece, PieceAttack> attackedSquaresByPiece)
        {
            King king = (King)board.GetPieceAt(kingPosition);
            List<Square> possibleMoves = king.GetPossibleMoves(board, kingPosition);
            bool kingCanMove = false;
            //we check each square where king can move if it is attacked by opposite color piece
            foreach (Square square in possibleMoves)
            {
                List<PieceAttack> attackedSquares = square.PieceAttacks;
                bool safeSquare = true;
                foreach (PieceAttack attackedSquaresSequence in attackedSquares)
                {
                    if (attackedSquaresSequence.Piece.Color != king.Color)
                    {
                        Console.WriteLine("Possible move at is attacked by: " + attackedSquaresSequence.Piece.GetType());
                        Console.WriteLine("At X: " + board.squarePositions[square].X + board.squarePositions[square].Y + " Y");
                        safeSquare = false;

                        //
                        //break;
                    }
                }
                if (safeSquare)
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
            if (ThreatAnalyzer.GetNumberOfEnemyAttackedSequencesOnPiece(king, board) <= 1 && ThreatAnalyzer.IsAttackOnKingBlockable(king, board, attackedSquaresByPiece))
            {
                Console.WriteLine("ATTACK BLOCKABLE");
                attackBlockable = true;
            }
            else
            {
                Console.WriteLine("ATTACK NOT BLOCKABLE");
            }

            if (!kingCanMove && !attackBlockable) { Console.WriteLine("CHECKMATE"); }
            else { Console.WriteLine("NOT CHECKMATE"); }
            //  }

            //tu potom treba najst ci existuje friendly piece, ktory sa moze postavit na square 
            //ktory je v attacked square sequence piecu ktory ho attackuje a pritom je blizsie k
            //attacking piecu ako friendly kral, chebysevova vzdialenost, ak je, neni checkmate
            //takisto treba skontrolovat ci sa attacking piece neda vyhodit, ak hej, neni checkmate
        }

    }
}
