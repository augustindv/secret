using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfileController : MonoBehaviour {

    public GameObject secretsContent;
    public GameObject myTeam;

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
        int i = 0;
        foreach (Transform child in secretsContent.transform)
        {
            GameObject textSecret = child.transform.FindChild("TextSecret").gameObject;
            GameObject image = child.transform.FindChild("Image").gameObject;

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
                textSecret.GetComponent<Text>().text = secret.secretText;
                Texture2D texture = Resources.Load("Secrets/Icons/icon-" + secret.imageID) as Texture2D;
                image.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            i++;
        }
    }

    public void UpdateProfileTeam(Player p)
    {
        myTeam.transform.FindChild("PlayerName").GetComponent<Text>().text = p.playerName;
        myTeam.transform.FindChild("Team").GetComponent<Text>().text = p.team.GetComponent<Team>().teamName;
        myTeam.transform.FindChild("Image").GetComponent<Image>().sprite = PlayerDatabase.instance.GetSprite(p.playerName);
    }
}
