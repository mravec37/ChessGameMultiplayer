using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.ChessPieces;
using ChessGameMultiplayer.Game.Logic;
using ChessGameMultiplayer.Game.Moves;
using System.Text.RegularExpressions;

namespace ChessGameMultiplayer.Game.Board;

public class ChessBoard
{
    private Square[,] squares = new Square[8, 8];

    public Dictionary<Square, Position> squarePositions;

    public Dictionary<Pawn, Position> enPassantPos;

    public Dictionary<ChessPiece, Position>  PiecesPos { get; set; }

    public Position WhiteKingPos { get; private set; }
    public Position BlackKingPos { get; private set; }

    bool initialized = false;


    public ChessBoard()
    {
        PiecesPos = new Dictionary<ChessPiece, Position>();
        enPassantPos = new Dictionary<Pawn, Position>();
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
        //piece positions for testing
        /* InitializeSquares();
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

         return createdPieces;*/



        /*InitializeSquares();
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

            PiecesPos[whitePawn] = whitePos;
            PiecesPos[blackPawn] = blackPos;

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

            PiecesPos[whitePiece] = whitePos;
            PiecesPos[blackPiece] = blackPos;

            createdPieces.Add(whitePiece);
            createdPieces.Add(blackPiece);
        }
        return createdPieces;*/

        InitializeSquares();
        var createdPieces = new List<ChessPiece>();

        // Rank 8 (y = 0): b . . . . r k .
        var p00 = new Bishop(ChessPieceColor.Black);
        MovePieceToSquare(new Position(0, 0), p00); PiecesPos[p00] = new Position(0, 0); createdPieces.Add(p00);

        var p50 = new Rook(ChessPieceColor.Black);
        MovePieceToSquare(new Position(5, 0), p50); PiecesPos[p50] = new Position(5, 0); createdPieces.Add(p50);

        var p60 = new King(ChessPieceColor.Black);
        MovePieceToSquare(new Position(6, 0), p60); PiecesPos[p60] = new Position(6, 0); createdPieces.Add(p60);


        // Rank 7 (y = 1): . . . . b p p p
        var p41 = new Bishop(ChessPieceColor.Black);
        MovePieceToSquare(new Position(4, 1), p41); PiecesPos[p41] = new Position(4, 1); createdPieces.Add(p41);

        var p51 = new Pawn(ChessPieceColor.Black);
        MovePieceToSquare(new Position(5, 1), p51); PiecesPos[p51] = new Position(5, 1); createdPieces.Add(p51);

        var p61 = new Pawn(ChessPieceColor.Black);
        MovePieceToSquare(new Position(6, 1), p61); PiecesPos[p61] = new Position(6, 1); createdPieces.Add(p61);

        var p71 = new Pawn(ChessPieceColor.Black);
        MovePieceToSquare(new Position(7, 1), p71); PiecesPos[p71] = new Position(7, 1); createdPieces.Add(p71);


        // Rank 6 (y = 2): p . . . q n . .
        var p02 = new Pawn(ChessPieceColor.Black);
        MovePieceToSquare(new Position(0, 2), p02); PiecesPos[p02] = new Position(0, 2); createdPieces.Add(p02);

        var p42 = new Queen(ChessPieceColor.Black);
        MovePieceToSquare(new Position(4, 2), p42); PiecesPos[p42] = new Position(4, 2); createdPieces.Add(p42);

        var p52 = new Knight(ChessPieceColor.Black);
        MovePieceToSquare(new Position(5, 2), p52); PiecesPos[p52] = new Position(5, 2); createdPieces.Add(p52);


        // Rank 5 (y = 3): empty


        // Rank 4 (y = 4): . . N . . . . .
        var p24 = new Knight(ChessPieceColor.White);
        MovePieceToSquare(new Position(2, 4), p24); PiecesPos[p24] = new Position(2, 4); createdPieces.Add(p24);


        // Rank 3 (y = 5): . . P r B . Q .
        var p25 = new Pawn(ChessPieceColor.White);
        MovePieceToSquare(new Position(2, 5), p25); PiecesPos[p25] = new Position(2, 5); createdPieces.Add(p25);

        var p35 = new Rook(ChessPieceColor.Black);
        MovePieceToSquare(new Position(3, 5), p35); PiecesPos[p35] = new Position(3, 5); createdPieces.Add(p35);

        var p45 = new Bishop(ChessPieceColor.White);
        MovePieceToSquare(new Position(4, 5), p45); PiecesPos[p45] = new Position(4, 5); createdPieces.Add(p45);

        var p65 = new Queen(ChessPieceColor.White);
        MovePieceToSquare(new Position(6, 5), p65); PiecesPos[p65] = new Position(6, 5); createdPieces.Add(p65);


        // Rank 2 (y = 6): P P . . . P . P
        var p06 = new Pawn(ChessPieceColor.White);
        MovePieceToSquare(new Position(0, 6), p06); PiecesPos[p06] = new Position(0, 6); createdPieces.Add(p06);

        var p16 = new Pawn(ChessPieceColor.White);
        MovePieceToSquare(new Position(1, 6), p16); PiecesPos[p16] = new Position(1, 6); createdPieces.Add(p16);

        var p56 = new Pawn(ChessPieceColor.White);
        MovePieceToSquare(new Position(5, 6), p56); PiecesPos[p56] = new Position(5, 6); createdPieces.Add(p56);

        var p76 = new Pawn(ChessPieceColor.White);
        MovePieceToSquare(new Position(7, 6), p76); PiecesPos[p76] = new Position(7, 6); createdPieces.Add(p76);


        // Rank 1 (y = 7): . . K . R . R .
        var p27 = new King(ChessPieceColor.White);
        MovePieceToSquare(new Position(2, 7), p27); PiecesPos[p27] = new Position(2, 7); createdPieces.Add(p27);

        var p47 = new Rook(ChessPieceColor.White);
        MovePieceToSquare(new Position(4, 7), p47); PiecesPos[p47] = new Position(4, 7); createdPieces.Add(p47);

        var p67 = new Rook(ChessPieceColor.White);
        MovePieceToSquare(new Position(6, 7), p67); PiecesPos[p67] = new Position(6, 7); createdPieces.Add(p67);

        WhiteKingPos = new Position(2, 7);
        BlackKingPos = new Position(6, 0);

        return createdPieces;

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
        if(piece is IMoveState moveState)
        {
            moveState.MarkAsMoved();
        }

        ChessPiece captured = MovePieceToSquare(to, piece);
        RemovePieceFromSquare(from);

        //if (captured != null) pieces.Remove(captured);

        //We dont need to update the attacking squares for the piece itself, because it gets updated automatically
        //because piece can only move to the square it attacks so when the piece is moved to a piece that it attacks
        //its attacking squares sequences gets notified to update automatically, except for pawn!!!!!!!!!
        /* GetSquare(from).UpdateAllAttackedSquaresSequences();
         GetSquare(to).UpdateAllAttackedSquaresSequences();*/

        PiecesPos[piece] = new Position(to.X, to.Y);
        UpdateKingPosition(to);
    }

    public void RemovePiece(ChessPiece captured)
    {
        if (captured != null) PiecesPos.Remove(captured);
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
        return PiecesPos[piece];
    }

}
