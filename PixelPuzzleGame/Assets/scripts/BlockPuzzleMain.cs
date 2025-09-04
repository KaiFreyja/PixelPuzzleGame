using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockPuzzleMain : MonoBehaviour
{
    public GameObject parent = null;
    public Texture2D taregtT2d = null;
    public int row = 3;
    public int col = 3;

    private BlockPiece[,] grid;
    private GameObject[,] blocks;
    private Vector2Int emptySlot;

    void Start()
    {
        if (taregtT2d == null)
            return;

        CreateBlock();
        RandomPuzzle(100);
        replaceView();
    }

    /// <summary>
    /// 建立拼圖
    /// </summary>
    public void CreateBlock()
    {
        parent.transform.localScale = Vector3.one;

        int w = taregtT2d.width;
        int h = taregtT2d.height;

        int unit_w = w / col;
        int unit_h = h / row;

        grid = new BlockPiece[row, col];
        blocks = new GameObject[row, col];
        emptySlot = new Vector2Int(row - 1, col - 1); // 最後一格是空的

        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < col; x++)
            {
                Vector2 pos = new Vector2(x * unit_w - w * 0.5f + unit_w * 0.5f, y * unit_h - h * 0.5f + unit_h * 0.5f);
                GameObject go = new GameObject(y + "_" + x);
                var img = go.AddComponent<Image>();
                img.rectTransform.SetParent(parent.transform);
                img.rectTransform.sizeDelta = new Vector2(10, 10);
                img.rectTransform.anchoredPosition = pos;
                blocks[y, x] = go;

                Texture2D piece = new Texture2D(unit_w, unit_h);

                piece.SetPixels(taregtT2d.GetPixels(x * unit_w, y * unit_h, unit_w, unit_h));
                piece.Apply();


                GameObject g = new GameObject("Piece_" + y + "_" + x);
                var rimage = g.AddComponent<RawImage>();
                rimage.texture = piece;
                rimage.rectTransform.SetParent(go.transform);
                rimage.rectTransform.sizeDelta = new Vector2(unit_w, unit_h);
                rimage.rectTransform.localScale = Vector3.one;
                rimage.rectTransform.anchoredPosition = Vector2.zero;

                if (y == row - 1 && x == col - 1)
                {
                    g.SetActive(false);
                    continue;
                }

                var pieceScript = g.AddComponent<BlockPiece>();
                pieceScript.Init(y, x, this);

                grid[y, x] = pieceScript;

                // 加入點擊事件
                Button btn = g.AddComponent<Button>();

                ColorBlock cb = btn.colors;
                cb.disabledColor = Color.white;
                btn.colors = cb;

                btn.onClick.AddListener(() => pieceScript.TryMove());
            }
        }
        replaceView();
    }

    /// <summary>
    /// 調整大小已對齊螢幕
    /// </summary>
    private void replaceView()
    {

        if (!this.taregtT2d || !this.parent)
            return;

        float texWidth = taregtT2d.width;
        float texHeight = taregtT2d.height;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 計算圖片與螢幕的寬高比
        float texRatio = texWidth / texHeight;
        float screenRatio = screenWidth / screenHeight;

        float scale = 1;

        if (screenRatio > texRatio)
        {
            // 螢幕比較寬，依高度縮放
            scale = screenHeight / texHeight;
        }
        else
        {
            // 螢幕比較窄，依寬度縮放
            scale = screenWidth / texWidth;
        }

        this.parent.transform.localScale = new Vector3(scale, scale, 1);
    }

    /// <summary>
    /// 隨機拼圖
    /// </summary>
    public void RandomPuzzle(int times)
    {
        for (int i = 0; i < times; i++)
        {
            List<BlockPiece> movable = new List<BlockPiece>();

            int x = emptySlot.x;
            int y = emptySlot.y;

            // 找出四個方向可移動的拼圖
            if (x > 0 && grid[x - 1, y] != null) movable.Add(grid[x - 1, y]);
            if (x < row - 1 && grid[x + 1, y] != null) movable.Add(grid[x + 1, y]);
            if (y > 0 && grid[x, y - 1] != null) movable.Add(grid[x, y - 1]);
            if (y < col - 1 && grid[x, y + 1] != null) movable.Add(grid[x, y + 1]);

            if (movable.Count > 0)
            {
                // 隨機挑一塊拼圖移動
                BlockPiece piece = movable[Random.Range(0, movable.Count)];
                TryMovePiece(piece);
            }
        }

    }

    public void Clear()
    {
        var childs = new List<GameObject>();
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            childs.Add(parent.transform.GetChild(i).gameObject);
        }
        foreach (var a in childs)
        {
            GameObject.Destroy(a);
        }

        parent.transform.localScale = Vector3.one;
        grid = null;
        blocks = null;
        emptySlot = Vector2Int.zero;
    }

    // 嘗試移動某個拼圖
    public void TryMovePiece(BlockPiece piece)
    {
        int dx = Mathf.Abs(piece.row - emptySlot.x);
        int dy = Mathf.Abs(piece.col - emptySlot.y);

        if ((dx + dy) == 1) // 與空格相鄰
        {
            // 取得目前位置
            BlockPiece moving = grid[piece.row, piece.col];

            // 目標位置（空格）
            GameObject target = blocks[emptySlot.x, emptySlot.y];

            // 更新 grid 資料
            grid[emptySlot.x, emptySlot.y] = moving;
            grid[piece.row, piece.col] = null;

            // 更新 BlockPiece 資料
            int oldRow = piece.row;
            int oldCol = piece.col;
            piece.row = emptySlot.x;
            piece.col = emptySlot.y;
            emptySlot = new Vector2Int(oldRow, oldCol);

            // 移動 UI
            moving.GetComponent<RectTransform>().SetParent(target.transform);
            moving.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    /// <summary>
    /// 檢查是否結束了
    /// </summary>
    public void CheckWin()
    {
        bool isWin = true;
        for (int i = 0; i < row; i++)
        {
            if (!isWin)
                break;

            for (int j = 0; j < col; j++)
            {
                var pice = grid[i, j];
                if (pice == null)
                {
                    continue;
                }
                if (!(pice != null && pice.isCheck()))
                {
                    isWin = false;
                    break;
                }
            }
        }

        if (isWin)
        {
            Debug.Log("結束");
            openAll();
        }
    }

    private void openAll()
    {
        foreach (var a in blocks)
        {
            var g = a.transform.GetChild(0).gameObject;
            g.SetActive(true);
            if (g.GetComponent<Button>() != null)
            {
                g.GetComponent<Button>().interactable = false;
            }
        }
    }
}
