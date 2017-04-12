using UnityEngine;

public class TextureMask
{
    private static Color[] texColors;
    private static Color[] newColors;
    private static Color[] maskColors;

    public static void Mask(Texture2D tex, Texture2D mask)
    {
        texColors = tex.GetPixels();
        maskColors = mask.GetPixels();
        newColors = new Color[mask.width * mask.height];

        for (var y = 0; y < mask.height; y++)
        {
            var yw = y * mask.width;
            for (var x = 0; x < mask.height; x++)
            {
                newColors[yw + x] = texColors[yw + x] * (1 - maskColors[yw + x].a);
            }
        }

        tex.SetPixels(newColors);
        tex.Apply();

        texColors = null;
        newColors = null;
    }
}