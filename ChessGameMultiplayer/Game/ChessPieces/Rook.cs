using ChessGameMultiplayer.Game.Attack;
using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.Logic;

namespace ChessGameMultiplayer.Game.ChessPieces
{
    public class Rook : ChessPieceSlidingAttacker, IMoveState
    {
        public Rook(ChessPieceColor color) : base(color) { }
        private bool Moved {get; set; } = false;

        public override bool IsValidMove(ChessBoard board, Position from, Position to)
        {
            if (board.GetPieceAt(to) != null && board.GetPieceAt(to).Color == Color)
            {
                return false; // Cannot capture own piece
            }
            if (from.X != to.X && from.Y != to.Y) {
                return false; // Rook can only move in straight lines
            }

            int dx = Math.Sign(to.X - from.X);
            int dy = Math.Sign(to.Y - from.Y);

            int x = from.X + dx;
            int y = from.Y + dy;

            while (x != to.X || y != to.Y)
            {
                //Dokonci
                if (board.GetPieceAt(new Position(x, y)) != null)
                {
                    return false; // Path is blocked
                }
                x += dx;
                y += dy;
            }

            return true;
        }

        public override char GetSymbol() => Color == ChessPieceColor.White ? 'r' : 'R';

        /* public override List<Square> GetAttackedSquares(ChessBoard board, Position position)
        {
            List<Square> attackedSquares = new List<Square>();
            bool kingAttackingSequenceSet = false;
            int[][] directions = new int[][]
          {
                new int[] {0,1},
                new int [] {1,0},
                new int [] {0,-1},
                new int [] {-1,0}, 
          };


            foreach (var dir in directions)
            {
                int dx = dir[0];
                int dy = dir[1];

                int x = position.X + dx;
                int y = position.Y + dy;

                List<Square> kingAttackingSequence = new List<Square>();

                while (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    Position squarePosition = new Position(x, y);
                    //Console.WriteLine("Square Position X: " + x + "  Y: " + y);
                    ChessPiece piece = board.GetPieceAt(squarePosition);

                    if (piece != null && piece is King && piece.Color != Color)
                    {
                        SlidingPieceAttack.SetKingAttackingSequence(new List<Square>(kingAttackingSequence));
                        kingAttackingSequenceSet = true;
                    }

                    attackedSquares.Add(board.GetSquare(squarePosition));
                    kingAttackingSequence.Add(board.GetSquare(squarePosition));

                    if (piece != null && (!(piece is King) || (piece is King && piece.Color == Color))) break;

                    x += dx;
                    y += dy;
                }
            }
            if (!kingAttackingSequenceSet)
            {
                SlidingPieceAttack.SetKingAttackingSequence(new List<Square>());
            }


            return attackedSquares;
        }*/

        public override List<Square> GetAttackedSquares(ChessBoard board, Position position)
        {
            List<Square> attackedSquares = new List<Square>();
            bool kingAttackBlocked = false;
            bool kingAttackDirect = false;
            int[][] directions = new int[][]
          {
                new int[] {0,1},
                new int [] {1,0},
                new int [] {0,-1},
                new int [] {-1,0}, 
          };


            foreach (var dir in directions)
            {
                int dx = dir[0];
                int dy = dir[1];

                int x = position.X + dx;
                int y = position.Y + dy;

                List<Square> potentialSquares = new List<Square>();
                bool blocked = false;
                while (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    Position squarePosition = new Position(x, y);
                    // Console.WriteLine("Square Position X: " + x + "  Y: " + y);
                    ChessPiece piece = board.GetPieceAt(squarePosition);


                    if (blocked) { potentialSquares.Add(board.GetSquare(squarePosition)); }
                    else { attackedSquares.Add(board.GetSquare(squarePosition)); }

                    if (piece != null && piece is King && piece.Color != Color)
                    {
                        //encounters enemy king
                        if (blocked)
                        {
                            //attack is blocked
                            attackedSquares.AddRange(potentialSquares);
                            AimsAtKing = true;
                            KingDirection[0] = dx;
                            KingDirection[1] = dy;
                            kingAttackBlocked = true;
                            break;

                        }
                        else
                        {
                            AttacksKing = true;
                            kingAttackDirect = true;
                            KingDirection[0] = dx;
                            KingDirection[1] = dy;

                            x += dx;
                            y += dy;
                            if (x >= 0 && x < 8 && y >= 0 && y < 8)
                            {
                                attackedSquares.Add(board.GetSquare(new Position(x, y)));
                                break;
                            }
                        }
                    }

                    else if (piece != null && (!(piece is King) || (piece is King && piece.Color == Color)))
                    {
                        //encounters piece that can block attack
                        blocked = true;
                    }

                    x += dx;
                    y += dy;
                }
            }

            return attackedSquares;
        }

        public void SetSlidingAttack(PieceAttackSliding attack)
        {
            throw new NotImplementedException();
        }

        public bool HasMoved()
        {
            return Moved;
        }

        public void MarkAsMoved()
        {
            Moved = true;
        }
    }
}
