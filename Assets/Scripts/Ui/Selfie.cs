using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class Selfie : MonoBehaviour
{
    WebCamTexture webCamTexture;
    public static int size = 256;

    void Start()
    {
        webCamTexture = new WebCamTexture();
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
        photo.SetPixels(webCamTexture.GetPixels(webCamTexture.width / 2 - smallest / 2, webCamTexture.height / 2 - smallest / 2, smallest, smallest )); // TODO setting mip level to reduce image size ...
        photo.Apply();
        TextureScale.Bilinear(photo, size, size);

        
        //Encode to a PNG
        byte[] bytes = photo.EncodeToJPG();
        webCamTexture.Stop();
        //byte[] bytes = new byte[] { 23, 24, 25, 26, 27 };
        cb(bytes);
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        File.WriteAllBytes(Application.persistentDataPath + "/selfie.png", bytes);
    }

    public void Snap(Player.PictureReady cb)
    {
        StartCoroutine(TakePhoto(cb));
    }
}