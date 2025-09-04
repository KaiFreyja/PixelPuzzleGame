using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    //ui
    public GameObject HomePanel = null;
    public GameObject BlockPuzzlePanel = null;


    public RawImage rawImage = null;
    private BlockPuzzleMain blockPuzzleMain = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blockPuzzleMain = GetComponent<BlockPuzzleMain>();
        HomePanel.SetActive(true);
        BlockPuzzlePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        if (rawImage == null)
            return;

        HomePanel.SetActive(false);
        BlockPuzzlePanel.SetActive(true);

        var tex = (Texture2D)rawImage.texture;
        blockPuzzleMain.taregtT2d = tex;
        blockPuzzleMain.CreateBlock();
        blockPuzzleMain.RandomPuzzle(100);
    }

    public void Previous()
    {
        HomePanel.SetActive(true);
        BlockPuzzlePanel.SetActive(false);
        blockPuzzleMain.Clear();
    }
}
