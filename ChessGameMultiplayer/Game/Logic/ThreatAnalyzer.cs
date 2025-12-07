using ChessGameMultiplayer.Game.Attack;
using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game.Logic
{
    public static class ThreatAnalyzer
    {
        public static int GetNumberOfEnemyAttacksOnPiece(ChessPiece piece, ChessBoard board)
        {
            Position position = board.GetPiecePosition(piece);
            Square square = board.GetSquare(position);
            ChessPieceColor pieceColor = piece.Color;
            int numberOfEnemyPieceAttacks = 0;

            foreach (PieceAttack pieceAttack in square.PieceAttacks)
            {
                if (pieceAttack.Piece.Color != pieceColor) numberOfEnemyPieceAttacks++;
            }

            return numberOfEnemyPieceAttacks;
        }

        public static bool IsSquareAttackedByEnemy(Position position, ChessPieceColor friendlyColor, ChessBoard board)
        {
            Square square = board.GetSquare(position);

            foreach (PieceAttack pieceAttack in square.PieceAttacks)
            {
                if (pieceAttack.Piece.Color != friendlyColor) return true;
            }

            return false;
        }

        public static bool InTheLineOfDefense(List<Square> squares, Position posTo, Position posAttacker, ChessBoard board)
        {
            if (posTo.X == posAttacker.X && posTo.Y == posAttacker.Y)
            {
                return true;
            }
            foreach (Square square in squares)
            {
                Position squarePos = board.squarePositions[square];
                if (squarePos.X == posTo.X && squarePos.Y == posTo.Y)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsAttackOnKingBlockable(King king, ChessBoard board, Dictionary<ChessPiece, PieceAttack> attackedSquaresByPiece)
        {

            bool blockable = false;

            PieceAttack pieceAttack = GetEnemyPieceAttack(king, board);
            Position attackingPiecePosition = board.GetPiecePosition(pieceAttack.Piece);
            Console.WriteLine("Piece attacking king: " + pieceAttack.Piece.GetType());

            List<Square> squaresToBlock = new List<Square>();
            squaresToBlock.Add(board.GetSquare(attackingPiecePosition));

            if (pieceAttack is PieceAttackSliding)
            {
                squaresToBlock.AddRange(((PieceAttackSliding)pieceAttack).KingAttackingSequence);
            }

            Console.WriteLine("Number of pieces is: " + attackedSquaresByPiece.Count);
            int count = 1;

            foreach (var (piece, attack) in attackedSquaresByPiece)
            {
                if (piece is King || piece.Color == pieceAttack.Piece.Color) continue;

                Console.WriteLine("Piece " + count + " : " + piece.GetType() + " on position " + board.GetPiecePosition(piece).X + ", " + board.GetPiecePosition(piece).Y);
                count++;

                List<Square> possibleMoves = attackedSquaresByPiece[piece].Squares;

                if (piece is Pawn)
                {
                    possibleMoves = ((Pawn)piece).GetPossibleMoves(board, board.GetPiecePosition(piece));
                }

                //traverse squares, check if piece attack can get onto one of these
                foreach (Square possibleMove in possibleMoves)
                {
                    foreach (Square squareToBlock in squaresToBlock)
                    {
                        if (possibleMove == squareToBlock)
                        {
                            //ak je pawn tak treba skontrolovat ci vyhodenie takisto blokne 
                            Console.WriteLine("Attack can be blocked by: " + piece.Color + " " + piece.GetType() + " at: " + board.squarePositions[possibleMove].X + ", " + board.squarePositions[possibleMove].Y);

                            //mozeme dat rovno return a nemusime pocitat dalsie
                            blockable = true;
                        }
                    }
                }
            }
            return blockable;
        }

        private static PieceAttack GetEnemyPieceAttack(ChessPiece piece,ChessBoard board)
        {
            Square square = board.GetSquare(board.GetPiecePosition(piece));
            foreach (PieceAttack pieceAttack in square.PieceAttacks)
            {
                if (pieceAttack.Piece.Color != piece.Color)
                {
                    return pieceAttack;
                }
            }

            return null;
        }


    }
}



