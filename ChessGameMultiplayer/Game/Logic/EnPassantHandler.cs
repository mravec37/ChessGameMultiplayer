using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game.Logic
{
    public static class EnPassantHandler
    {
       /* private void HandleEnPassant(Position from, Position to, ChessPiece? movingPiece)
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
                        Position enPassantPos = kvp.Value;
                        if (to.Equals(enPassantPos) && Math.Abs(to.X - from.X) == 1 && to.Y == from.Y + (movingPawn.Color == ChessPieceColor.White ? -1 : 1))
                        {
                            Position capturedPawnPos = new Position(to.X, from.Y);
                            ChessPiece capturedPawn = Board.GetPieceAt(capturedPawnPos);
                            ClearCapturedPiece(capturedPawn);
                            Board.MovePiece(capturedPawnPos, new Position(-1, -1)); // Remove captured pawn from the board
                            break;
                        }
                    }
                }
                //Clear en passant positions for all pawns of the opposite color
                var pawnsToClear = Board.enPassantPos.Keys.Where(p => p.Color != movingPawn.Color).ToList();
                foreach (var pawn in pawnsToClear)
                {
                    Board.enPassantPos.Remove(pawn);
                }
            }
        }*/

    }
}
