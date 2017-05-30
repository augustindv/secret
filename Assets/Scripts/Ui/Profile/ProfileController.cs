using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfileController : MonoBehaviour {

    public GameObject secretsContent;
    public GameObject myTeam;
    public GameObject secretPopup;

    public static ProfileController profileController;
    public static ProfileController instance
    {
        get
        {
            if (!profileController)
            {
                profileController = FindObjectOfType(typeof(ProfileController)) as ProfileController;

                if (!profileController)
                {
                    Debug.LogError("There needs to be one active ProfileController script on a GameObject in your scene.");
                }
            }

            return profileController;
        }
    }

    public void UpdateProfileSecrets(Player p)
    {
        Structures.SyncListNetworkInstanceId secrets = p.secrets;

        int cardDeckID = PlayerDatabase.instance.GetPlayerDeckID(p);

        Texture2D texture = PlayerDatabase.instance.GetColorTexture(p.playerName);
        Sprite colorSprite  = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        texture = PlayerDatabase.instance.GetChestTexture(p.playerName);
        Sprite chestSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        int i = 0;
        foreach (Transform mySecret in secretsContent.transform)
        {
            GameObject textSecret = mySecret.transform.FindChild("TextSecret").gameObject;
            GameObject image = mySecret.transform.FindChild("Image").gameObject;
            GameObject secretBorder = mySecret.transform.FindChild("SecretBorder").gameObject;
            GameObject sharedImage = mySecret.transform.FindChild("Shared").gameObject;

            NetworkInstanceId id = secrets[i].netID;
            Secret secret = null;
            if (p.isLocalPlayer)
            {
                secret = ClientScene.FindLocalObject(id).GetComponent<Secret>();
            }
            else if (p.isServer)
            {
                secret = NetworkServer.FindLocalObject(id).GetComponent<Secret>();
            }

            if (secret != null)
            {
                mySecret.gameObject.GetComponent<Button>().onClick.AddListener(delegate { OpenSecretPopup(p.playerName, secret.secretTextProfile, secret.secretID, secret.shared); });
                textSecret.GetComponent<Text>().text = secret.secretTextProfile;

                GameObject color = image.transform.FindChild("Color").gameObject;
                GameObject icon = image.transform.FindChild("Icon").gameObject;
                GameObject chest = image.transform.FindChild("Chest").gameObject;
                GameObject holder = image.transform.FindChild("Holder").gameObject;

                Sprite spriteIcon = secret.spriteIcon;
                if (spriteIcon == null)
                {
                    texture = Resources.Load("Secrets/Icons/icon-" + secret.secretID) as Texture2D;
                    spriteIcon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    secret.spriteIcon = spriteIcon;
                }
                icon.GetComponent<Image>().sprite = spriteIcon;

                color.GetComponent<Image>().sprite = colorSprite;
                chest.GetComponent<Image>().sprite = chestSprite;

                holder.GetComponent<Image>().sprite = PlayerDatabase.instance.GetSprite(p.playerName);

                secretBorder.GetComponent<Image>().color = PlayerDatabase.instance.GetPlayerDeckColor(p);
                if (secret.shared)
                {
                    sharedImage.SetActive(true);
                    sharedImage.GetComponent<Image>().color = PlayerDatabase.instance.GetPlayerDeckColor(p);
                } else
                {
                    sharedImage.SetActive(false);
                }

                mySecret.transform.FindChild("SecretNumber").GetComponent<Image>().color = PlayerDatabase.instance.GetPlayerDeckColor(p);
                mySecret.transform.FindChild("SecretNumber").FindChild("Text").GetComponent<Text>().text = (i+1) + "";
            }
            i++;
        }
    }

    public void UpdateProfileTeam(Player p)
    {
        myTeam.transform.FindChild("PlayerName").GetComponent<Text>().text = p.playerName;
        myTeam.transform.FindChild("Team").GetComponent<Text>().text = p.team.GetComponent<Team>().teamName;
        myTeam.transform.FindChild("Head").GetComponent<Image>().sprite = PlayerDatabase.instance.GetSprite(p.playerName);
        Texture2D texture = PlayerDatabase.instance.GetChestTexture(p.playerName);
        myTeam.transform.FindChild("Chest").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void OpenSecretPopup(string playerNameTarget, string secretText, int imageID, bool shared)
    {
        Texture2D texture = Resources.Load("Secrets/Icons/icon-" + imageID) as Texture2D;
        Transform secretIcon = secretPopup.transform.FindChild("SecretIcon");
        GameObject sharedText = secretPopup.transform.FindChild("Shared").gameObject;
        if (!shared)
        {
            sharedText.SetActive(false);
        } else
        {
            sharedText.SetActive(true);
        }

        GameObject color = secretIcon.FindChild("Color").gameObject;
        GameObject icon = secretIcon.FindChild("Icon").gameObject;
        GameObject chest = secretIcon.FindChild("Chest").gameObject;
        GameObject holder = secretIcon.FindChild("Holder").gameObject;

        icon.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        texture = PlayerDatabase.instance.GetColorTexture(playerNameTarget);
        color.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        texture = PlayerDatabase.instance.GetChestTexture(playerNameTarget);
        chest.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        holder.GetComponent<Image>().sprite = PlayerDatabase.instance.GetSprite(playerNameTarget);

        secretPopup.transform.FindChild("SecretText").GetComponent<Text>().text = secretText;
        secretPopup.transform.FindChild("CloseButton").GetComponent<Button>().onClick.RemoveAllListeners();
        secretPopup.transform.FindChild("CloseButton").GetComponent<Button>().onClick.AddListener(delegate { secretPopup.SetActive(false); });
        secretPopup.transform.FindChild("PopUpBorder").GetComponent<Image>().color = PlayerDatabase.instance.GetPlayerDeckColor(UiMainController.instance.localPlayer);
        secretPopup.SetActive(true);
    }
}
