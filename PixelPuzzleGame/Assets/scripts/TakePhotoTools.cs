using System;
using UnityEngine;

public class TakePhotoTools : MonoBehaviour
{
    public static TakePhotoTools Insence
    {
        get
        {
            if (_instance == null)
            {
                GameObject Main = GameObject.Find("Main");
                if (Main == null)
                {
                    Main = new GameObject("Main");
                }
                _instance = Main.AddComponent<TakePhotoTools>();
            }
            return _instance;
        }
    }

    private static TakePhotoTools _instance;
    private static Action<string> tempAction;
    public void TakePhoto(Action<string> action)
    {
        tempAction = action;
#if UNITY_EDITOR
        Debug.Log("TakePhoto");
        OnPictureTaken("Resource/793c802c-d5ef-4e5d-be80-f9a83fd8dfa6.png");
#elif UNITY_ANDROID
    TakePhotoAndroid();
#elif UNITY_IOS
    TakePhotoIos();
#else

#endif
    }

    private void TakePhotoAndroid()
    {
        using (AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass pluginClass = new AndroidJavaClass("com.kai.unityplugin.NativeBridge");
            pluginClass.CallStatic("takePicture");
        }
    }

    private void TakePhotoIos()
    {
        Debug.Log("ios take photo ios");
        IOSCamera.TakePicture();
    }

    /// <summary>
    /// call back
    /// </summary>
    /// <param name="path"></param>
    public void OnPictureTaken(string path)
    {
        Debug.Log("OnPictureTaken : " + path);

        if (tempAction != null)
        {
            try
            {
                tempAction(path);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            tempAction = null;
        }
    }
}
