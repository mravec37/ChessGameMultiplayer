using ChessGameMultiplayer.Game.Attack;
using ChessGameMultiplayer.Game.Board;
using ChessGameMultiplayer.Game.Logic;

namespace ChessGameMultiplayer.Game.ChessPieces
{
    public class Rook : ChessPieceSlidingAttacker, IMoveState
    {
        private bool Moved { get; set; } = false;
        public Rook(ChessPieceColor color) : base(color) { }

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

            Console.WriteLine("Original position: " + position.X + ":" +  position.Y);

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
                    Console.WriteLine("Square Position X: " + x + "  Y: " + y);
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
                            Console.WriteLine("King directions(blocked) changed to: " + KingDirection[0] + " " + KingDirection[1]);
                            Console.WriteLine("Position of black king is: " + board.BlackKingPos.X + ":" + board.BlackKingPos.Y);
                            Console.WriteLine("Position of white king is: " + board.WhiteKingPos.X + ":" + board.WhiteKingPos.Y);
                            kingAttackBlocked = true;
                            break;

                        }
                        else
                        {
                            AttacksKing = true;
                            kingAttackDirect = true;
                            KingDirection[0] = dx;
                            KingDirection[1] = dy;
                            Console.WriteLine("King directions changed(non blocked) to: " + KingDirection[0] + " " + KingDirection[1]);
                            Console.WriteLine("Position of black king is: " + board.BlackKingPos.X + ":" + board.BlackKingPos.Y);
                            Console.WriteLine("Position of white king is: " + board.WhiteKingPos.X + ":" + board.WhiteKingPos.Y);

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
            if(!kingAttackBlocked)
            {
                AimsAtKing = false;
            }
            if (!kingAttackDirect)
            {
                AttacksKing = false;
            }

            return attackedSquares;
        }

        public override List<Square> GetPossibleMoves(ChessBoard board, Position position)
        {
            List<Square> possibleMoves = new List<Square>();

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

                while (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    Position squarePosition = new Position(x, y);
                    // Console.WriteLine("Square Position X: " + x + "  Y: " + y);
                    ChessPiece piece = board.GetPieceAt(squarePosition);

                    if (piece != null)
                    {
                        if (piece.Color != Color && piece is not King)
                        {
                            possibleMoves.Add(board.GetSquare(new Position(x, y)));
                        }
                        break;
                    }

                    possibleMoves.Add(board.GetSquare(new Position(x, y)));

                    x += dx;
                    y += dy;
                }
            }
            return possibleMoves;
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

        public override List<Position> GetMoveSquares(Position positionTo, Position positionFrom)
        {
            int dx = positionTo.X - positionFrom.X;
            int dy = positionTo.Y - positionFrom.Y;

            if (dx != 0) dx = dx / Math.Abs(dx);
            if (dy != 0) dy = dy / Math.Abs(dy);

            var positions = new List<Position>();
            positions.Add(positionFrom);
            int currentPosX = positionFrom.X;
            int currentPosY = positionFrom.Y;


            while (currentPosX != positionTo.X || currentPosY != positionTo.Y)
            {
                currentPosX += dx;
                currentPosY += dy;
                positions.Add(new Position(currentPosX, currentPosY));
            }

            Console.WriteLine("Printing move square positions");
            foreach (Position position in positions)
            {
                Console.WriteLine("X: " + position.X + " " + position.Y);
            }
            return positions;
        }
    }
}
