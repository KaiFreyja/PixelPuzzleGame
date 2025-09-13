using System.Runtime.InteropServices;

public class IOSCamera
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void takePicture();

    [DllImport("__Internal")]
    private static extern void pickFromGallery();
#endif

    public static void TakePicture()
    {
#if UNITY_IOS && !UNITY_EDITOR
        takePicture();
#endif
    }

    public static void PickFromGallery()
    {
#if UNITY_IOS && !UNITY_EDITOR
        pickFromGallery();
#endif
    }
}
