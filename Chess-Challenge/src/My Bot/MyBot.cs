using System;
using ChessChallenge.API;

public class MyBot : IChessBot
{

   Move bestMoveThisTurn = Move.NullMove;


    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        Console.WriteLine($"Found {moves.Length} moves:\n");
        foreach (var move in moves)
        {
            Console.WriteLine(move + "\tTargetSquareIndex: " + move.TargetSquare
            + "\t CaptureValue = " + MoveCaptureValue(board, move));
        }

        // Search for the best move, go X levels deep
        Move? bestMove = Search(board, moves, 3);

        // return the move that has the highest capture value, but if there are no capture moves, return the move that has the highest board position value

        return bestMove ?? moves[0];
    }

    private Move? Search(Board board, Move[] moves, int depth)
    {
        if (moves.Length == 0) { return null; }

        if (depth == 0) { return moves[0]; }

        int[] moveValuesAfterDepth = new int[moves.Length];

        Move[] placeboMoves = new Move[1000];
        int i = 0;

        while (depth > 0)
        {   for (;i < moves.Length; i++)
            {
                // search and sort the moves by capture value, and then by board position value if the capture value is 0        
                Array.Sort(moves, (moveA, moveB) => MoveCaptureValue(board, moveB) - MoveCaptureValue(board, moveA));

                // if the first move is a checkmate, return it
                if (moves[i].CapturePieceType == PieceType.King) { return moves[i]; }
                // if the first move is not a capture move, sort the moves by board position value
                if (moves[i].CapturePieceType == PieceType.None)
                {
                    Array.Sort(moves, (move1, move2) => BoardPositionValueOfMove(board, move1) - BoardPositionValueOfMove(board, move2));
                }
                placeboMoves[i] = moves[0];
            }

            placeboMoves[i] = moves[0];
            
            depth--;

        }

        // make current best move
        Move bestMove = moves[0];
        // iterate through all moves and find the best move by recursively calling the search function

        // redect moves in placeboMoves
        foreach (var move in placeboMoves)
        {            
            if(move!=null)
            {
                board.MakeMove(move);
                Move? bestMoveAfterDepth = Search(board, moves, depth);
                
            }
            board.UndoMove(move);
        }


        return bestMove;
    }


    // Capture value is equal to the value of the captured piece minus the value of the capturing piece times -1 
    public int MoveCaptureValue(Board board, Move move)
    {
        PieceType capturedPiece = board.GetPiece(move.TargetSquare).PieceType;

        // captureValue is equal to the value of the captured piece minus the value of the capturing piece times -1 (to keep the good positive), except for the king which is 1024
        int captureValue = capturedPiece == PieceType.King ? 1024 : ((int)capturedPiece - (int)board.GetPiece(move.StartSquare).PieceType) * -1;

        // return 0 if the target square is empty and the capture value if it is not
        return board.GetPiece(move.TargetSquare).PieceType ==
         PieceType.None ? 0 : captureValue;
    }

    public int BoardPositionValueOfMove(Board board, Move move)
    {
        // return the distance from the center of the board of the target square
        return Math.Abs(move.TargetSquare.File - 3) + Math.Abs(move.TargetSquare.Rank - 3);
    }



}