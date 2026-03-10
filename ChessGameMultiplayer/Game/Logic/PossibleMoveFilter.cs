using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.Attack;
using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game.Logic
{
    public class PossibleMoveFilter
    {
        public static List<Square> filterPossibleMoves(ChessPiece piece, ChessBoard board, Dictionary<ChessPiece, PieceAttack> attackedSquaresByPiece, List<Square> possibleMoves)
        {
            var filteredMoves = new List<Square>();
            if (piece is King)
            {
                foreach (Square square in possibleMoves)
                {
                    if (!ThreatAnalyzer.IsSquareAttackedByEnemy(square, piece.Color))
                    {
                        filteredMoves.Add(square);
                    }
                }
            } else
            {
                return FilterNonKingPossibleMoves(piece, board, attackedSquaresByPiece, possibleMoves);
            }
            return filteredMoves;
        }

        public static List<Square> FilterNonKingPossibleMoves(ChessPiece movingPiece, ChessBoard board, Dictionary<ChessPiece, PieceAttack> attackedSquaresByPiece, List<Square> possibleMoves)
        {
            Console.WriteLine("Checking if move endangers own king...");

            foreach (var (piece, attack) in attackedSquaresByPiece)
            {
                if (piece is ChessPieceSlidingAttacker slidingAttacker)
                {
                    PieceAttackSliding pieceAttackSliding = (PieceAttackSliding)attack;
                    //piece color == current turn
                    if (pieceAttackSliding.AimsAtKing() && movingPiece.Color != pieceAttackSliding.Piece.Color)
                    {
                        //Check if moving piece from 'from' to 'to' blocks the attack sequence
                        List<Square> attackSequence = pieceAttackSliding.AimAtKingSequence;
                        bool blocksAttack = false;
                        bool onlyDefender = true;
                        foreach (Square square in attackSequence)
                        {
                            if (square.Piece == null) continue;

                            if (square.Piece == movingPiece)
                            {
                                blocksAttack = true;
                            }
                            else
                                onlyDefender = false;
                        }
                        //if (blocksAttack && onlyDefender && !ThreatAnalyzer.InTheLineOfDefense(attackSequence, request.To, board.GetPiecePosition(piece), board))
                        if (blocksAttack && onlyDefender)
                        {
                            var filteredMoves = new List<Square>();
                            Console.WriteLine("Move of piece: " + movingPiece.GetType() + " endangers own king.");
                            foreach (Square square in possibleMoves)
                            {
                                if(ThreatAnalyzer.InTheLineOfDefense(attackSequence, board.squarePositions[square], board.GetPiecePosition(piece), board))
                                {
                                    filteredMoves.Add(square);
                                }
                            }
                            return filteredMoves;
                        } 
                    }
                }
            }
            return possibleMoves;
        }

    }
}
