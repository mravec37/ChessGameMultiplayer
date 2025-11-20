using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;
using ChessGameMultiplayer.Game.Moves;

namespace ChessGameMultiplayer.Game.Logic
{
    public static class CastlingHandler
    {
        public static MoveResult HandleCastling(ChessBoard board, Position from, Position to)
        {
             if(CanCastle(board, from, to))
             {
                return PerformCastling(board, from, to);
             }
             else
             {
                Console.WriteLine("Invalid castling move attempted.");
                return new MoveResult
                {
                    IsValid = false,
                    ErrorMessage = "Invalid castling move"
                };
             }
        }

        private static MoveResult PerformCastling(ChessBoard board, Position from, Position to)
        {
            ChessPiece king = board.GetPieceAt(from);
            ChessPiece rook = board.GetPieceAt(to);
            int dx = Math.Sign(to.X - from.X);
            // Move King
            Position newKingPos = new Position(from.X + 2 * dx, from.Y);
            board.MovePiece(from, newKingPos);
            // Move Rook
            Position newRookPos = new Position(newKingPos.X - dx, from.Y);
            board.MovePiece(to, newRookPos);
            MoveResult moveResult = new MoveResult
            {
                IsValid = true,
                Affected = new List<MoveEffect>
                    {
                        new MoveEffect
                        {
                            from = from,
                            to = newKingPos,
                            Piece = king,
                            Type = MoveEffectType.Castling
                        },
                        new MoveEffect
                        {
                            from = to,
                            to = newRookPos,
                            Piece = rook,
                            Type = MoveEffectType.Castling
                        }
                    }
            };
            return moveResult;
        }

        private static bool CastlingSquaresAttacked(ChessBoard board, Position from, Position to)
        {
            int dx = Math.Sign(to.X - from.X);
            int x = from.X;
            for (int i = 0; i < 2; i++)
            {
                x += dx;
                Position pos = new Position(x, from.Y);
                if (ThreatAnalyzer.IsSquareAttackedByEnemy(pos, board.GetPieceAt(from).Color, board))
                {
                    return true;
                }
            }
            return false;
        }


        private static bool SquaresBetweenEmpty(ChessBoard board, Position from, Position to)
        {
            int dx = Math.Sign(to.X - from.X);
            for (int x = from.X + dx; x != to.X; x += dx)
            {
                if (board.GetPieceAt(new Position(x, from.Y)) != null)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CanCastle(ChessBoard board, Position from, Position to)
        {
            King king = board.GetPieceAt(from) as King;
            Rook rook = board.GetPieceAt(to) as Rook;
            if (king == null || rook == null)
            {
                throw new InvalidOperationException("Castling move must involve a King and a Rook.");
            }
            if (!IsCastlingMove(board, from, to))
            {
                return false;
            }
            if (((IMoveState)king).HasMoved() || ((IMoveState)rook).HasMoved())
            {
                Console.WriteLine("Cannot castle with a piece that has already moved.");
                return false;
            }
            if (ThreatAnalyzer.IsSquareAttackedByEnemy(from, king.Color, board))
            {
                Console.WriteLine("Cannot castle while in check.");
                return false;
            }
            if (!SquaresBetweenEmpty(board, from, to))
            {
                Console.WriteLine("Cannot castle, squares between King and Rook are not empty.");
                return false;
            }
            if (CastlingSquaresAttacked(board, from, to))
            {
                Console.WriteLine("Cannot castle, squares the King passes through are under attack.");
                return false;
            }
            return true;
        }

        public static bool IsCastlingMove(ChessBoard board, Position from, Position to)
        {
            ChessPiece pieceFrom = board.GetPieceAt(from);
            ChessPiece pieceTo = board.GetPieceAt(to);

            if (pieceFrom is King && pieceTo is Rook && pieceFrom.Color == pieceTo.Color)
            {
                return true;
            }

            return false;
        }
    }

}
