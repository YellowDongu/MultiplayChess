using UnityEngine;
using UnityEngine.UIElements;
using static PieceManager;

public class MovementChecker : MonoBehaviour
{
    // ==============================================================================
    // Methods
    // ==============================================================================

    public void Initialize(bool _hostIsBlack)
    {
        if (board == null)
            board = new (PieceType, bool)[8, 8];

        GameEnd = 0;
        turn = true;
        hostIsBlack = _hostIsBlack;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                board[i, j] = (PieceType.END, false);
            }
        }

        for (int i = 0; i < 8; i++)
        {
            board[i, 6] = (PieceType.Pawn, false);
            board[i, 1] = (PieceType.Pawn, true);
        }

        board[0, 7] = (PieceType.Rook, false);
        board[7, 7] = (PieceType.Rook, false);

        board[0, 0] = (PieceType.Rook, true);
        board[7, 0] = (PieceType.Rook, true);

        board[1, 7] = (PieceType.Knight, false);
        board[6, 7] = (PieceType.Knight, false);

        board[1, 0] = (PieceType.Knight, true);
        board[6, 0] = (PieceType.Knight, true);

        board[2, 7] = (PieceType.Bishop, false);
        board[5, 7] = (PieceType.Bishop, false);

        board[2, 0] = (PieceType.Bishop, true);
        board[5, 0] = (PieceType.Bishop, true);

        board[3, 7] = (PieceType.Queen, false);
        board[4, 0] = (PieceType.Queen, true);

        board[4, 7] = (PieceType.King, false);
        board[3, 0] = (PieceType.King, true);
    }

    public bool Move(int x, int y, int toX, int toY)
    {
        if (x < 0 || y < 0 || toX < 0 || toY < 0)
            return false;

        if (x >= 8 || y >= 8 || toX >= 8 || toY >= 8)
            return false;

        if (!AvaliableMove(x, y, toX, toY))
            return false;

        if (board[toX, toY].type == PieceType.King)
            GameEnd = (board[toX, toY].isBlack) ? 1 : -1;

        board[toX, toY] = board[x, y];
        board[x, y] = (PieceType.END, false);
        turn = !turn;
        return true;
    }

    private bool AvaliableMove(int x, int y, int toX, int toY)
    {
        if (board[x, y].type == PieceType.END)
            return false;
        if (board[toX, toY].type != PieceType.END && board[x, y].isBlack == board[toX, toY].isBlack)
            return false;

        switch (board[x, y].type)
        {
            case PieceType.Pawn:
                {
                    bool result = Pawn.Validate(board, x, y, toX, toY);
                    if (result)
                        ChangePawn(x, y, toX, toY);
                    return result;
                }
            case PieceType.Rook:
                return Rook.Validate(board, x, y, toX, toY);
            case PieceType.Knight:
                return Knight.Validate(x, y, toX, toY);
            case PieceType.Bishop:
                return Bishop.Validate(board, x, y, toX, toY);
            case PieceType.Queen:
                return Queen.Validate(board, x, y, toX, toY);
            case PieceType.King:
                return King.Validate(board, x, y, toX, toY);
            default:
                return false;
        }
    }

    //public void ChangePawn(Vector2Int position, PieceType Type = PieceType.Queen)
    public void ChangePawn(int x, int y, int toX, int toY, PieceType Type = PieceType.Queen) // 일단 퀸만, UI 준비가 되면 다른 말로도 선택 가능하게.
    {
        if (board[x, y].type != PieceType.Pawn)
            return;

        if (board[x, y].isBlack && toY != 7)
            return;
        if (!board[x, y].isBlack && toY != 0)
            return;

        board[x, y].type = Type;
        board[x, y].type = PieceType.Queen; // remove later
    }

    public void TestEndTurn() { turn = !turn; }


    // ==============================================================================
    // variable & GetSet Methods
    // ==============================================================================

    private (PieceType type, bool isBlack)[,] board = new (PieceType, bool)[8, 8];
    public int GameEnd { get; private set; }
    public bool turn { get; private set; }
    public bool hostIsBlack { get; private set; }
}
