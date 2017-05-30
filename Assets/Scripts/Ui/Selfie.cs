using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class Selfie : MonoBehaviour
{
    WebCamTexture webCamTexture;
    public static int size = 256;
    public Quaternion baseRotation;
    private bool foundWebCamSize = false;
    private Vector2 webCamSize = new Vector2(100, 100);


    void Start()
    {
        WebCamDevice frontWebcam = WebCamTexture.devices[0];
        foreach(WebCamDevice device in WebCamTexture.devices)
        {
            if (device.isFrontFacing)
                frontWebcam = device;
        }

        webCamTexture = new WebCamTexture(frontWebcam.name, 256, 256);
        baseRotation = transform.rotation;
        gameObject.GetComponent<Image>().material.mainTexture = webCamTexture;
        webCamTexture.Play();
    }

    IEnumerator TakePhoto(Player.PictureReady cb)
    {

        // NOTE - you almost certainly have to do this here:

        yield return new WaitForEndOfFrame();
        //yield return new WaitForSeconds(0.3f);

        // it's a rare case where the Unity doco is pretty clear,
        // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
        // be sure to scroll down to the SECOND long example on that doco page 

        int smallest = Mathf.Min(webCamTexture.width, webCamTexture.height);
        Texture2D photo = new Texture2D(smallest, smallest);
        photo.SetPixels(webCamTexture.GetPixels(
            webCamTexture.width / 2 - smallest / 2, 
            webCamTexture.height / 2 - smallest / 2, 
            smallest, smallest )); // TODO setting mip level to reduce image size ...
        photo.Apply();
        TextureScale.Bilinear(photo, size, size);

        Debug.Log(webCamTexture.videoRotationAngle);
        // TODO something more generic ...

        Color32[] pixels = photo.GetPixels32();
        if (webCamTexture.videoRotationAngle > 0) pixels = Rotate90(pixels, photo.width, 0);

#if UNITY_IOS // IOS rotate phto 180... who knows shy
        pixels = Rotate180(pixels, photo.width, 0);
#endif
        photo.SetPixels32(pixels);
        

        //Encode to a PNG
        byte[] bytes = photo.EncodeToJPG(50);
        webCamTexture.Stop();
        gameObject.GetComponent<Image>().enabled = false;
        //byte[] bytes = new byte[] { 23, 24, 25, 26, 27 };
        cb(bytes);
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        Debug.Log("writing to " + Application.persistentDataPath);
        File.WriteAllBytes(Application.persistentDataPath + "/selfietest.png", bytes);
  /*      File.WriteAllBytes("Assets/selfieQ.jpg", photo.EncodeToJPG());
        File.WriteAllBytes("Assets/selfieQ30.jpg", photo.EncodeToJPG(30));
        File.WriteAllBytes("Assets/selfieQ75.jpg", photo.EncodeToJPG(75));
        File.WriteAllBytes("Assets/selfieQ50.jpg", photo.EncodeToJPG(50));*/

    }

    public void Snap(Player.PictureReady cb)
    {
        StartCoroutine(TakePhoto(cb));
    }

    static Color32[] Rotate90(Color32[] matrix, int n, int angle)
    {
        Color32[] ret = new Color32[n * n];

        for (int x = 0; x < n; ++x)
        {
            for (int y = 0; y < n; ++y)
            {
                ret[x * n + y] = matrix[(n - y - 1) * n + x];
            }
        }

        return ret;
    }

    static Color32[] Rotate180(Color32[] matrix, int n, int angle)
    {
        Color32[] ret = new Color32[n * n];

        for (int x = 0; x < n; ++x)
        {
            for (int y = 0; y < n; ++y)
            {
                ret[x * n + y] = matrix[n * (n - 1 - x) + n - 1 - y];
            }
        }

        return ret;
    }

    void Update()
    {
        if(!foundWebCamSize)
        {
            if (webCamTexture.width < 100)
            {
                Debug.Log("Still waiting another frame for correct info...");
                return;
            }
            else
            {
                webCamSize = new Vector2(webCamTexture.width, webCamTexture.height);
                Debug.Log("Webcam size" + webCamSize);
                foundWebCamSize = true;
                float scaleY = webCamTexture.videoVerticallyMirrored ? 1.0f : -1.0f;
                if (webCamSize.x > webCamSize.y)
                    transform.localScale = new Vector3(webCamSize.x / webCamSize.y, scaleY, 0.0f);
                else
                    transform.localScale = new Vector3(1, scaleY * (webCamSize.y / webCamSize.x), 0.0f);

            }
        }
        transform.rotation = baseRotation * Quaternion.AngleAxis(webCamTexture.videoRotationAngle, Vector3.back);
    }
}