using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.Attack;
using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game.Logic
{
    public static class MoveValidator
    {

        public static bool IsMovePossible(MoveRequest request, ChessBoard board)
        {
            Position from = request.From;
            Position to = request.To;
            var piece = board.GetPieceAt(from);
            if (piece == null)
            {
                return false;
            }
            if (!piece.IsValidMove(board, from, to))
            {
                return false;
            }

            var captured = board.GetPieceAt(to);

            if (captured is King)
            {
                return false;
            }

            return true;
        }

        public static bool MoveForbidden(MoveRequest request, ChessBoard board)
        {
            //ci je kral checknuty - jeho square je attacknuta
            ChessPiece movingPiece = board.GetPieceAt(request.From);
            King king = movingPiece.Color == ChessPieceColor.White ? (King)board.GetPieceAt(board.WhiteKingPos) : (King)board.GetPieceAt(board.BlackKingPos);
            int numberOfAttacksOnKing = ThreatAnalyzer.GetNumberOfEnemyAttackedSequencesOnPiece(king, board);
            if (numberOfAttacksOnKing == 0)
            {
                return false;
            }
            if (numberOfAttacksOnKing >= 2 && movingPiece is not King)
            {
                Console.WriteLine("Move forbidden: king is double checked.");
                return true;
            }
            if (movingPiece is King && !ThreatAnalyzer.IsSquareAttackedByEnemy(request.To, movingPiece.Color, board))
            {
                return false;
            }

            PieceAttack attack = null;
            foreach (PieceAttack pieceAttack in board.GetSquare(board.GetPiecePosition(king)).PieceAttacks)
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
                    Position attackerPos = board.GetPiecePosition(slidingAttack.Piece);
                    if (ThreatAnalyzer.InTheLineOfDefense(attackedSquares, request.To, attackerPos, board))
                    {
                        return false;
                    }
                    Console.WriteLine("Move forbidden: You must block or capture attacking piece");
                    return true;
                }
                else
                {
                    Position attackerPos = board.GetPiecePosition(attack.Piece);
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

        public static bool MoveEndangersOwnKing(MoveRequest request, ChessBoard board, Dictionary<ChessPiece, PieceAttack> attackedSquaresByPiece)
        {
            Console.WriteLine("Checking if move endangers own king...");
            ChessPiece movingPiece = board.GetPieceAt(request.From);

            if (movingPiece is King && ThreatAnalyzer.IsSquareAttackedByEnemy(request.To, movingPiece.Color, board))
            {
                Console.WriteLine("Move endangers own king because destination square is attacked by enemy.");
                return true;
            }

            foreach (var (piece, attack) in attackedSquaresByPiece)
            {
                if (piece is ChessPieceSlidingAttacker slidingAttacker)
                {
                    PieceAttackSliding pieceAttackSliding = (PieceAttackSliding)attack;
                    //piece color == current turn
                    if (pieceAttackSliding.AimsAtKing() && movingPiece.Color != pieceAttackSliding.Piece.Color)
                    {
                        //Check if moving piece from 'from' to 'to' blocks the attack sequence
                        Console.WriteLine("Piece: " + piece.GetType() + " aims at king from position X: " + board.GetPiecePosition(piece).X + " Y: " + board.GetPiecePosition(piece).Y);
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
                        if (blocksAttack && onlyDefender && !ThreatAnalyzer.InTheLineOfDefense(attackSequence, request.To, board.GetPiecePosition(piece), board))
                        {
                            Console.WriteLine("Move of piece: " + movingPiece.GetType() + " endangers own king.");
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}