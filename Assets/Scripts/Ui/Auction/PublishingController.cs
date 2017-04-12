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
    private static string PUBLISH_NOTHING = "Vous ne publier rien";

    public GameObject secretPopupPublishing;
    public GameObject inventoryLinePrefab;
    public GameObject inventorySecretPrefab;
    public GameObject inventorySecretEmptyPrefab;
    public GameObject publishingContent;
    public Button publishNothing;
    public GameObject choiceMadePanel;
    public GameObject scrollSecretsToPublish;

    private bool choiceMade;

    public void Start()
    {
        publishNothing.onClick.AddListener(delegate {
            choiceMade = !choiceMade;
            if (choiceMade)
            {
                //UiMainController.instance.localPlayer.CmdIsReadyForNextPhase(true);
                UiMainController.instance.localPlayer.checkToPublish = new Check(null, null, null);
                ChoiceMade(new Check(), false);
            }
        });
    }

    public void UpdatePublishingPanel(Player p)
    {
        // Attention : only unpublished
        List<Check> checks = p.checks;
        List<Check> tmpChecks = checks.FindAll(c => !c.secret.published);

        bool evenNbSecrets = tmpChecks.Count % 2 == 0;
        int numberOfLines = evenNbSecrets ? tmpChecks.Count / 2 : (tmpChecks.Count / 2) + 1;
        int addedNb = 0;

        foreach (Check check in tmpChecks)
        {
            Secret s = check.secret;
            Player playerOwner = check.playerTargeted;

            GameObject line = null;
            if (addedNb % 2 == 0)
            {
                line = Instantiate(inventoryLinePrefab, publishingContent.transform);
            }
            else
            {
                line = publishingContent.transform.GetChild(publishingContent.transform.childCount - 1).gameObject;
            }

            GameObject inventorySecret = Instantiate(inventorySecretPrefab, line.transform);
            GameObject inventorySecretEmpty = null;

            if (addedNb == (tmpChecks.Count-1) && !evenNbSecrets)
            {
                inventorySecretEmpty = Instantiate(inventorySecretEmptyPrefab, line.transform);
                inventorySecretEmpty.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                inventorySecretEmpty.transform.localScale = new Vector3(1, 1, 1);
            }

            inventorySecret.transform.localScale = new Vector3(1, 1, 1);
            line.transform.localScale = new Vector3(1, 1, 1);

            Sprite sprite = PlayerDatabase.instance.GetSprite(check.playerTargeted.playerName);
            inventorySecret.transform.FindChild("Holder").GetComponent<Image>().sprite = sprite;
            Texture2D texture = Resources.Load("Secrets/Icons/icon-" + check.secret.imageID) as Texture2D;
            inventorySecret.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            inventorySecret.GetComponent<Button>().onClick.AddListener(delegate { if (!choiceMade) OpenSecretPopupPublishing(check); });

            addedNb++;
        }

    }

    public void OpenSecretPopupPublishing(Check check)
    {
        publishNothing.gameObject.SetActive(false);
        Texture2D texture = Resources.Load("Secrets/Icons/icon-" + check.secret.imageID) as Texture2D;
        secretPopupPublishing.transform.FindChild("SecretIcon").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        secretPopupPublishing.transform.FindChild("SecretText").GetComponent<Text>().text = check.playerTargeted.playerName + " " + check.secret.secretText;
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
                UiMainController.instance.localPlayer.checkToPublish = check;
                UiMainController.instance.localPlayer.CmdIsReadyForNextPhase(true);
                ChoiceMade(check, true);
            }
        });
        secretPopupPublishing.SetActive(true);
    }

    public void ChoiceMade(Check check, bool publishingSomething)
    {
        secretPopupPublishing.SetActive(false);
        scrollSecretsToPublish.SetActive(false);
        publishNothing.gameObject.SetActive(false);
        choiceMadePanel.transform.FindChild("ChoiceText").GetComponent<Text>().text = publishingSomething ? PUBLISH_SOMETHING : PUBLISH_NOTHING;
        GameObject secretChosen = choiceMadePanel.transform.FindChild("SecretChosen").gameObject;
        if (publishingSomething)
        {
            Texture2D texture = Resources.Load("Secrets/Icons/icon-" + check.secret.imageID) as Texture2D;
            secretChosen.transform.FindChild("SecretIcon").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            secretChosen.transform.FindChild("SecretText").GetComponent<Text>().text = check.playerTargeted.playerName + " " + check.secret.secretText;
        } else
        {
            secretChosen.SetActive(false);
        }
        choiceMadePanel.SetActive(true);
    }

}
