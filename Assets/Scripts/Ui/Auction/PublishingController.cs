using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PublishingController : MonoBehaviour {

    public static PublishingController auctionController;
    public static PublishingController instance
    {
        get
        {
            if (!auctionController)
            {
                auctionController = FindObjectOfType(typeof(PublishingController)) as PublishingController;

                if (!auctionController)
                {
                    Debug.LogError("There needs to be one active AuctionController script on a GameObject in your scene.");
                }
            }

            return auctionController;
        }
    }

    private static string PUBLISH_SOMETHING = "Vous allez publier ce secret";
    private static string PUBLISH_NOTHING = "Vous ne publiez rien";

    public GameObject secretPopupPublishing;
    public GameObject inventorySecretPrefab;
    public GameObject publishingContent;
    public Button publishNothing;
    public GameObject choiceMadePanel;
    public GameObject scrollSecretsToPublish;

    private bool choiceMade;

    public Button nothingPublishedButton;

    public void Start()
    {
        publishNothing.onClick.AddListener(delegate {
            choiceMade = !choiceMade;
            if (choiceMade)
            {
                UiMainController.instance.localPlayer.checkToPublish = new Check(null, null, null, false, 0);
                UiMainController.instance.localPlayer.CmdIsReadyForNextPhase(true);
                ChoiceMade(new Check(), false);
            }
        });

        nothingPublishedButton.onClick.AddListener(delegate
        {
            AnimationController.instance.StopAnimationSecret();
            AnimationController.instance.PlayWaitingAnimation();
            //UiMainController.instance.localPlayer.CmdSetNextPhase(GamePhase.DiscussionBeforeDecision);
            UiMainController.instance.localPlayer.CmdIsReadyForNextPhase(true);
        });
    }

    public void ResetUiPublishing()
    {
        Color playerColor = PlayerDatabase.instance.GetPlayerDeckColor(UiMainController.instance.localPlayer);
        publishNothing.transform.FindChild("BtPublishBorder").GetComponent<Image>().color = playerColor;
        secretPopupPublishing.transform.FindChild("PublishSecretButton").FindChild("BtPublishBorder").GetComponent<Image>().color = playerColor;
        secretPopupPublishing.transform.FindChild("PopUpBorder").GetComponent<Image>().color = playerColor;
        scrollSecretsToPublish.SetActive(true);
        secretPopupPublishing.SetActive(false);
        publishNothing.gameObject.SetActive(true);
        choiceMadePanel.SetActive(false);
        choiceMade = false;
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in publishingContent.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }

    public void UpdatePublishingPanel(Player p)
    {
        List<Check> checks = p.checks;
        List<Check> tmpChecks = checks.FindAll(c => !c.published);

        foreach (Check check in tmpChecks)
        {
            Secret s = check.secret;

            GameObject inventorySecret = Instantiate(inventorySecretPrefab, publishingContent.transform);
            inventorySecret.transform.localScale = new Vector3(1, 1, 1);
            inventorySecret.transform.FindChild("SecretBg").GetComponent<Image>().color = new Color(24f / 255f, 26f / 255f, 32f / 255f, 1f);
            inventorySecret.transform.FindChild("SecretBorder").GetComponent<Image>().color = PlayerDatabase.instance.GetPlayerDeckColor(UiMainController.instance.localPlayer);
            Sprite spriteIcon = s.spriteIcon;
            Texture2D texture = null;
            if (spriteIcon == null)
            {
                texture = Resources.Load("Secrets/Icons/icon-" + check.secret.secretID) as Texture2D;
                spriteIcon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                check.secret.spriteIcon = spriteIcon;
            }
            Transform secretIcon = inventorySecret.transform.FindChild("Image");
            GameObject color = secretIcon.FindChild("Color").gameObject;
            GameObject icon = secretIcon.FindChild("Icon").gameObject;
            GameObject chest = secretIcon.FindChild("Chest").gameObject;
            GameObject holder = secretIcon.FindChild("Holder").gameObject;
            GameObject sharedImage = inventorySecret.transform.FindChild("Shared").gameObject;
            sharedImage.SetActive(false);
            icon.GetComponent<Image>().sprite = spriteIcon;
            texture = Resources.Load("Secrets/Colors/color-" + PlayerDatabase.instance.GetPlayerDeckID(check.playerTargeted)) as Texture2D;
            color.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            texture = Resources.Load("Secrets/Chests/chest-" + PlayerDatabase.instance.GetPlayerDeckID(check.playerTargeted)) as Texture2D;
            chest.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            holder.GetComponent<Image>().sprite = PlayerDatabase.instance.GetSprite(check.playerTargeted.playerName);
            inventorySecret.transform.FindChild("TextSecret").GetComponent<Text>().text = check.playerTargeted.playerName + " " + s.secretTextCommon;
            inventorySecret.transform.FindChild("SecretNumber").GetComponent<Image>().color = PlayerDatabase.instance.GetPlayerDeckColor(UiMainController.instance.localPlayer);
            inventorySecret.transform.FindChild("SecretNumber").FindChild("Text").GetComponent<Text>().text = check.cardID + "";
            inventorySecret.GetComponent<Button>().onClick.AddListener(delegate { if (!choiceMade) OpenSecretPopupPublishing(check); });
        }

        if (tmpChecks.Count == 0)
        {
            GameObject tmpSecretEmpty = Instantiate(InventoryController.instance.secretsEmpty, publishingContent.transform);
            tmpSecretEmpty.transform.FindChild("Text").GetComponent<Text>().text = InventoryController.FOUND_EMPTY;
        }
    }

    public void OpenSecretPopupPublishing(Check check)
    {
        publishNothing.gameObject.SetActive(false);
        Texture2D texture = null;
        Sprite spriteIcon = check.secret.spriteIcon;
        if (spriteIcon == null)
        {
            texture = Resources.Load("Secrets/Icons/icon-" + check.secret.secretID) as Texture2D;
            spriteIcon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            check.secret.spriteIcon = spriteIcon;
        }
        Transform secretIcon = secretPopupPublishing.transform.FindChild("SecretIcon");
        GameObject color = secretIcon.FindChild("Color").gameObject;
        GameObject icon = secretIcon.FindChild("Icon").gameObject;
        GameObject chest = secretIcon.FindChild("Chest").gameObject;
        GameObject holder = secretIcon.FindChild("Holder").gameObject;
        Player p = PlayerDatabase.instance.GetPlayer(check.playerTargeted.playerName);
        icon.GetComponent<Image>().sprite = spriteIcon;
        texture = Resources.Load("Secrets/Colors/color-" + PlayerDatabase.instance.GetPlayerDeckID(p)) as Texture2D;
        color.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        texture = Resources.Load("Secrets/Chests/chest-" + PlayerDatabase.instance.GetPlayerDeckID(p)) as Texture2D;
        chest.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        holder.GetComponent<Image>().sprite = PlayerDatabase.instance.GetSprite(p.playerName);
        secretPopupPublishing.transform.FindChild("SecretText").GetComponent<Text>().text = check.playerTargeted.playerName + " " + check.secret.secretTextCommon;
        secretPopupPublishing.transform.FindChild("CloseButton").GetComponent<Button>().onClick.RemoveAllListeners();
        secretPopupPublishing.transform.FindChild("CloseButton").GetComponent<Button>().onClick.AddListener(delegate {
            if (!choiceMade)
            {
                secretPopupPublishing.SetActive(false);
                publishNothing.gameObject.SetActive(true);
            }
        });
        secretPopupPublishing.transform.FindChild("PublishSecretButton").GetComponent<Button>().onClick.RemoveAllListeners();
        secretPopupPublishing.transform.FindChild("PublishSecretButton").GetComponent<Button>().onClick.AddListener(delegate {
            choiceMade = !choiceMade;
            if (choiceMade)
            {
                // Add the secret to the auctions !
                Player localPlayer = UiMainController.instance.localPlayer;
                localPlayer.checkToPublish = check;
                CheckID checkIDToPublish = localPlayer.checksID.ToList().Find(c => check.secret.netId.Equals(c.secret));
                localPlayer.CmdAddCheckToPublish(checkIDToPublish);
                UiMainController.instance.localPlayer.CmdIsReadyForNextPhase(true);
                ChoiceMade(check, true);
            }
        });
        secretPopupPublishing.SetActive(true);
    }

    public void ChoiceMade(Check check, bool publishingSomething)
    {
        choiceMadePanel.SetActive(true);
        secretPopupPublishing.SetActive(false);
        scrollSecretsToPublish.SetActive(false);
        publishNothing.gameObject.SetActive(false);
        choiceMadePanel.transform.FindChild("ChoiceText").GetComponent<Text>().text = publishingSomething ? PUBLISH_SOMETHING : PUBLISH_NOTHING;
        GameObject secretChosen = choiceMadePanel.transform.FindChild("SecretChosen").gameObject;
        if (publishingSomething)
        {
            Sprite spriteIcon = check.secret.spriteIcon;
            Texture2D texture = null;
            if (spriteIcon == null)
            {
                texture = Resources.Load("Secrets/Icons/icon-" + check.secret.secretID) as Texture2D;
                spriteIcon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                check.secret.spriteIcon = spriteIcon;
            }
            Transform secretIcon = secretChosen.transform.FindChild("SecretIcon");
            GameObject color = secretIcon.FindChild("Color").gameObject;
            GameObject icon = secretIcon.FindChild("Icon").gameObject;
            GameObject chest = secretIcon.FindChild("Chest").gameObject;
            GameObject holder = secretIcon.FindChild("Holder").gameObject;
            Player p = PlayerDatabase.instance.GetPlayer(check.playerTargeted.playerName);
            icon.GetComponent<Image>().sprite = spriteIcon;
            texture = Resources.Load("Secrets/Colors/color-" + PlayerDatabase.instance.GetPlayerDeckID(p)) as Texture2D;
            color.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            texture = Resources.Load("Secrets/Chests/chest-" + PlayerDatabase.instance.GetPlayerDeckID(p)) as Texture2D;
            chest.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            holder.GetComponent<Image>().sprite = PlayerDatabase.instance.GetSprite(p.playerName);
            secretChosen.transform.FindChild("SecretText").GetComponent<Text>().text = check.playerTargeted.playerName + " " + check.secret.secretTextCommon;
            secretChosen.transform.FindChild("PopUpBg").GetComponent<Image>().color = new Color(24f / 255f, 26f / 255f, 32f / 255f, 1f);
            secretChosen.transform.FindChild("PopUpBorder").GetComponent<Image>().color = PlayerDatabase.instance.GetPlayerDeckColor(UiMainController.instance.localPlayer);
            secretChosen.SetActive(true);
        }
        else
        {
            secretChosen.SetActive(false);
        }
    }

}
