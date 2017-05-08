using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorController : MonoBehaviour {

    public static ColorController colorController;
    public static ColorController instance
    {
        get
        {
            if (!colorController)
            {
                colorController = FindObjectOfType(typeof(ColorController)) as ColorController;

                if (!colorController)
                {
                    Debug.LogError("There needs to be one active ColorController script on a GameObject in your scene.");
                }

            }

            return colorController;
        }
    }

    public Image navProfile;
    public Image navInventory;
    public Image navMessage;

    public Image closeBt1;
    public Image closeBt2;
    public Image closeBt3;

    public SpriteRenderer background;
    public Image backgroundVuforia;

    public void SetColoredUI(int deckID)
    {
        Texture2D texture = Resources.Load("ColorUi/" + deckID + "/UiProfilActive" + deckID) as Texture2D;
        navProfile.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        texture = Resources.Load("ColorUi/" + deckID + "/UiInventoryActive" + deckID) as Texture2D;
        navInventory.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        texture = Resources.Load("ColorUi/" + deckID + "/UiMessageActive" + deckID) as Texture2D;
        navMessage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        texture = Resources.Load("ColorUi/" + deckID + "/UiProfilInactive" + deckID) as Texture2D;
        UiMainController.instance.buttonsSprites[0] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        texture = Resources.Load("ColorUi/" + deckID + "/UiInventoryInactive" + deckID) as Texture2D;
        UiMainController.instance.buttonsSprites[1] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        texture = Resources.Load("ColorUi/" + deckID + "/UiMessageInactive" + deckID) as Texture2D;
        UiMainController.instance.buttonsSprites[2] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        texture = Resources.Load("ColorUi/" + deckID + "/UiClose" + deckID) as Texture2D;
        closeBt1.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        closeBt2.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        closeBt3.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        backgroundVuforia.color = PlayerDatabase.instance.GetPlayerDeckColor(UiMainController.instance.localPlayer);
        background.color = PlayerDatabase.instance.GetPlayerDeckColor(UiMainController.instance.localPlayer);

        ChangeScreen.buttonsSprites = UiMainController.instance.buttonsSprites;

        ChangeScreen.SetButtons();
    }
}
