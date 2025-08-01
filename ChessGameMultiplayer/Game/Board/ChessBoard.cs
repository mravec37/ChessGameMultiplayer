using ChessGameMultiplayer.Game.ChessPieces;

namespace ChessGameMultiplayer.Game.Board;

public class ChessBoard
{
    private Square[,] squares = new Square[8, 8];

    public ChessBoard()
    {
    }

    private void InitializeSquares()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                squares[x, y] = new Square();
            }
        }
    }

    public void InitializeStartingPositions()
    {
        InitializeSquares();
        // Setup pawns
        for (int x = 0; x < 8; x++)
        {
            SetPiece(new Position(x, 6), new Pawn(ChessPieceColor.White));
            SetPiece(new Position(x, 1), new Pawn(ChessPieceColor.Black));
        }

        // Setup back rows
        string[] order = { "Rook", "Knight", "Bishop", "Queen", "King", "Bishop", "Knight", "Rook" };

        for (int x = 0; x < 8; x++)
        {
            // White pieces
            SetPiece(new Position(x, 7), CreatePiece(order[x], ChessPieceColor.White));
            // Black pieces
            SetPiece(new Position(x, 0), CreatePiece(order[x], ChessPieceColor.Black));
        }
    }

    private ChessPiece CreatePiece(string type, ChessPieceColor color)
    {
        return type switch
        {
            "Pawn" => new Pawn(color),
            "Rook" => new Rook(color),
            "Knight" => new Knight(color),
            "Bishop" => new Bishop(color),
            "Queen" => new Queen(color),
            "King" => new King(color),
            _ => throw new ArgumentException($"Unknown piece type: {type}")
        };
    }


    public Square GetSquare(int x, int y)
    {
        return squares[x, y];
    }

    public Square GetSquare(Position pos)
    {
        return squares[pos.X, pos.Y];
    }

    public ChessPiece? GetPieceAt(Position pos)
    {
        return GetSquare(pos).Piece;
    }

    public void SetPiece(Position pos, ChessPiece? piece)
    {
        GetSquare(pos).Piece = piece;
    }

    public void MovePiece(Position from, Position to)
    {
        var piece = GetPieceAt(from);
        if (piece != null)
        {
            Console.WriteLine($"Moving {piece.GetType().Name} ({piece.Color}) from ({from.X}, {from.Y}) to ({to.X}, {to.Y})");
        }
        else
        {
            Console.WriteLine($"Attempted to move from empty square at ({from.X}, {from.Y})");
        }

        SetPiece(to, piece);
        SetPiece(from, null);
    }

}
