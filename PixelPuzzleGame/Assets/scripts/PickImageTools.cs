using System;
using UnityEngine;

public class PickImageTools : MonoBehaviour
{
    public static PickImageTools Insence
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
                _instance = Main.AddComponent<PickImageTools>();
            }
            return _instance;
        }
    }

    private static PickImageTools _instance;
    private static Action<string> tempAction;
    public void PickImage(Action<string> action)
    {
        tempAction = action;
#if UNITY_EDITOR
        Debug.Log("PickImage");
        OnImagePicked("Resource/793c802c-d5ef-4e5d-be80-f9a83fd8dfa6.png");
#elif UNITY_ANDROID
    PickImageAndroid();
#elif UNITY_IOS
    PickImageIos();
#else

#endif
    }

    private void PickImageAndroid()
    {
        using (AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass pluginClass = new AndroidJavaClass("com.kai.unityplugin2.NativeBridge");
            pluginClass.CallStatic("Pickimage");
        }
    }

    private void PickImageIos()
    {
        IOSCamera.PickFromGallery();
    }
    public void OnPictureTaken(string path)
    {
        OnImagePicked(path);
    }
    public void OnImagePicked(string path)
    {
        Debug.Log("OnImagePicked : " + path);
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

    public void OnImagePickFailed(string message)
    {
        Debug.Log(message);
    }
}
