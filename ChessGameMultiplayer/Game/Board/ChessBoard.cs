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

        // --- Black pieces ---
        var blackKingPos = new Position(7, 0);
        var blackBishopPos = new Position(1, 0);
        var blackPawn1Pos = new Position(0, 1);
        var blackPawn2Pos = new Position(2, 1);

        var blackKing = new King(ChessPieceColor.Black);
        var blackBishop = new Bishop(ChessPieceColor.Black);
        var blackPawn1 = new Pawn(ChessPieceColor.Black);
        var blackPawn2 = new Pawn(ChessPieceColor.Black);

        MovePieceToSquare(blackKingPos, blackKing);
        MovePieceToSquare(blackBishopPos, blackBishop);
        MovePieceToSquare(blackPawn1Pos, blackPawn1);
        MovePieceToSquare(blackPawn2Pos, blackPawn2);

        pieces[blackKing] = blackKingPos;
        pieces[blackBishop] = blackBishopPos;
        pieces[blackPawn1] = blackPawn1Pos;
        pieces[blackPawn2] = blackPawn2Pos;

        createdPieces.AddRange(new List<ChessPiece> { blackKing, blackBishop, blackPawn1, blackPawn2 });

        // --- White pieces ---
        var whiteKingPos = new Position(1, 5);
        var whiteQueen1Pos = new Position(3, 2);
        var whiteQueen2Pos = new Position(6, 2);
        var whitePawn1Pos = new Position(0, 2);
        var whitePawn2Pos = new Position(2, 2);

        var whiteKing = new King(ChessPieceColor.White);
        var whiteQueen1 = new Queen(ChessPieceColor.White);
        var whiteQueen2 = new Queen(ChessPieceColor.White);
        var whitePawn1 = new Pawn(ChessPieceColor.White);
        var whitePawn2 = new Pawn(ChessPieceColor.White);

        MovePieceToSquare(whiteKingPos, whiteKing);
        MovePieceToSquare(whiteQueen1Pos, whiteQueen1);
        MovePieceToSquare(whiteQueen2Pos, whiteQueen2);
        MovePieceToSquare(whitePawn1Pos, whitePawn1);
        MovePieceToSquare(whitePawn2Pos, whitePawn2);

        pieces[whiteKing] = whiteKingPos;
        pieces[whiteQueen1] = whiteQueen1Pos;
        pieces[whiteQueen2] = whiteQueen2Pos;
        pieces[whitePawn1] = whitePawn1Pos;
        pieces[whitePawn2] = whitePawn2Pos;

        WhiteKingPos = new Position(1, 5);
        BlackKingPos = new Position(7, 0);

        createdPieces.AddRange(new List<ChessPiece> { whiteKing, whiteQueen1, whiteQueen2, whitePawn1, whitePawn2 });

        return createdPieces;

   

        /* InitializeSquares();
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
         return createdPieces;*/
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
