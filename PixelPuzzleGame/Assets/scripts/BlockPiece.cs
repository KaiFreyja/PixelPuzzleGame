using UnityEngine;
using UnityEngine.UI;

public class BlockPiece : MonoBehaviour
{
    //åªç›à íu
    public int row; 
    public int col;
    // ê≥ämà íu
    public int originRow, originCol;
    private BlockPuzzleMain puzzle;

    public void Init(int r, int c, BlockPuzzleMain p)
    {
        row = r;
        col = c;
        originRow = r;
        originCol = c;
        puzzle = p;
    }

    public bool isCheck()
    {
        return row == originRow && col == originCol;
    }

    public void TryMove()
    {
        puzzle.TryMovePiece(this);
        puzzle.CheckWin();
    }
}
