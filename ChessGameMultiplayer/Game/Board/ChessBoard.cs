using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.ChessPieces;
using ChessGameMultiplayer.Game.Moves;
using System.Text.RegularExpressions;

namespace ChessGameMultiplayer.Game.Board;

public class ChessBoard
{
    private Square[,] squares = new Square[8, 8];

    public Dictionary<Square, Position> squarePositions;

    private Dictionary<ChessPiece, Position> pieces;

    public Position WhiteKingPos { get; private set; }
    public Position BlackKingPos { get; private set; }

    bool initialized = false;


    public ChessBoard()
    {
        pieces = new Dictionary<ChessPiece, Position>();
        squarePositions = new Dictionary<Square, Position>();
        WhiteKingPos = new Position(4, 7);
        BlackKingPos = new Position(4, 0);
    }

    private void InitializeSquares()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Square square = new Square();
                squares[x, y] = square;
                squarePositions[square] = new Position(x, y);
            }
        }
    }

    public List<ChessPiece> InitializeStartingPositions()
    {
        InitializeSquares();
        var createdPieces = new List<ChessPiece>();
        // Setup pawns
        for (int x = 0; x < 8; x++)
        {
            var whitePos = new Position(x, 6);
            var blackPos = new Position(x, 1);

            var whitePawn = new Pawn(ChessPieceColor.White);
            var blackPawn = new Pawn(ChessPieceColor.Black);

            MovePieceToSquare(whitePos, whitePawn);
            MovePieceToSquare(blackPos, blackPawn);

            pieces[whitePawn] = whitePos;
            pieces[blackPawn] = blackPos;

            createdPieces.Add(whitePawn);
            createdPieces.Add(blackPawn);
        }

        // Setup back rows
        string[] order = { "Rook", "Knight", "Bishop", "Queen", "King", "Bishop", "Knight", "Rook" };

        for (int x = 0; x < 8; x++)
        {
            var whitePos = new Position(x, 7);
            var blackPos = new Position(x, 0);

            var whitePiece = CreatePiece(order[x], ChessPieceColor.White);
            var blackPiece = CreatePiece(order[x], ChessPieceColor.Black);

            MovePieceToSquare(whitePos, whitePiece);
            MovePieceToSquare(blackPos, blackPiece);

            pieces[whitePiece] = whitePos;
            pieces[blackPiece] = blackPos;

            createdPieces.Add(whitePiece);
            createdPieces.Add(blackPiece);
        }
        return createdPieces;
        /*if (!initialized)
        {
            initialized = true;

            InitializeSquares();
            var createdPieces = new List<ChessPiece>();

            // Black pieces
            var blackKing = new King(ChessPieceColor.Black);
            MovePieceToSquare(new Position(2, 0), blackKing);
            pieces[blackKing] = new Position(2, 0);
            createdPieces.Add(blackKing);
            BlackKingPos = new Position(2, 0);

            var blackBishop1 = new Bishop(ChessPieceColor.Black);
            MovePieceToSquare(new Position(7, 1), blackBishop1);
            pieces[blackBishop1] = new Position(7, 1);
            createdPieces.Add(blackBishop1);

            var blackBishop2 = new Bishop(ChessPieceColor.Black);
            MovePieceToSquare(new Position(6, 3), blackBishop2);
            pieces[blackBishop2] = new Position(6, 3);
            createdPieces.Add(blackBishop2);

            // White pieces
            var whiteKing = new King(ChessPieceColor.White);
            MovePieceToSquare(new Position(2, 7), whiteKing);
            pieces[whiteKing] = new Position(2, 7);
            createdPieces.Add(whiteKing);
            WhiteKingPos = new Position(2, 7);

            var whiteRook = new Rook(ChessPieceColor.White);
            MovePieceToSquare(new Position(1, 7), whiteRook);
            pieces[whiteRook] = new Position(1, 7);
            createdPieces.Add(whiteRook);

            var whiteBishop = new Bishop(ChessPieceColor.White);
            MovePieceToSquare(new Position(3, 7), whiteBishop);
            pieces[whiteBishop] = new Position(3, 7);
            createdPieces.Add(whiteBishop);

            var whitePawn1 = new Pawn(ChessPieceColor.White);
            MovePieceToSquare(new Position(1, 6), whitePawn1);
            pieces[whitePawn1] = new Position(1, 6);
            createdPieces.Add(whitePawn1);

            var whitePawn2 = new Pawn(ChessPieceColor.White);
            MovePieceToSquare(new Position(7, 4), whitePawn2);
            pieces[whitePawn2] = new Position(7, 4);
            createdPieces.Add(whitePawn2);

            Console.WriteLine("Number of created pieces: " + createdPieces.Count);
            return createdPieces;
        }
        return null;*/
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

    public ChessPiece MovePieceToSquare(Position pos, ChessPiece? piece)
    {
        return GetSquare(pos).SetPiece(piece);
    }

    public void RemovePieceFromSquare(Position pos)
    {
        GetSquare(pos).RemovePiece();
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

        ChessPiece captured = MovePieceToSquare(to, piece);
        RemovePieceFromSquare(from);

        //if (captured != null) pieces.Remove(captured);

        //We dont need to update the attacking squares for the piece itself, because it gets updated automatically
        //because piece can only move to the square it attacks so when the piece is moved to a piece that it attacks
        //its attacking squares sequences gets notified to update automatically, except for pawn!!!!!!!!!
        /* GetSquare(from).UpdateAllAttackedSquaresSequences();
         GetSquare(to).UpdateAllAttackedSquaresSequences();*/

        pieces[piece] = new Position(to.X, to.Y);
        UpdateKingPosition(to);
    }

    public void RemoveCapturedPiece(ChessPiece captured)
    {
        if (captured != null) pieces.Remove(captured);
    }

    public void UpdateAllPieceAttacksOnSquare(Position position)
    {
        GetSquare(position).UpdateAllAttackedSquaresSequences();
    }

    private void UpdateKingPosition(Position pos)
    {
        if (GetPieceAt(pos) is King king)
        {
            if (king.Color == ChessPieceColor.White)
            {
                WhiteKingPos = pos;
            }
            else
            {
                BlackKingPos = pos;
            }
        }
    }

    internal Position GetPiecePosition(ChessPiece piece)
    {
        return pieces[piece];
    }

}
