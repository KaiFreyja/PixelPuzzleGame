using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PickImageToImage : MonoBehaviour
{
    public RawImage targetImage = null;

    public void PickImage()
    {
        PickImageTools.Insence.PickImage(PickImage);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    private void PickImage(string path)
    {
        Texture2D tex = LoadTexture2d(path);
        if (targetImage != null)
        {
            targetImage.texture = tex;
            targetImage.SetNativeSize();
        }
    }

    private Texture2D LoadTexture2d(string path)
    {
        var file = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(file);
        replaceView(tex);
        return tex;
    }

    /// <summary>
    /// 調整大小已對齊螢幕
    /// </summary>
    private void replaceView(Texture2D taregtT2d)
    {

        if (!taregtT2d || !targetImage)
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

        this.targetImage.transform.localScale = new Vector3(scale, scale, 1);
    }
}
