using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tuto : MonoBehaviour {

	public Button TutoButton;
	public GameObject uiStartGame;

	public GameObject UiTutoScreen01;
	public Button ButtonScreen01;

	public GameObject uiMain;
    public GameObject secretsContent;
    public GameObject secretPopup;

    public GameObject ProfilTuto;
	public GameObject uiProfil01;
	public Button ProfilButton01;
	public GameObject uiProfil02;
	public Button ProfilButton02;
	public GameObject uiProfil03;
	public Button ProfilButton03;
	public GameObject uiProfil04;
	public Button ProfilButton04;
	public GameObject uiProfil05;

	public GameObject uiInventory;
	public Button InventoryTutoButton;
	public GameObject uiInventory01;
	public Button InventoryTutoButton01;
	public GameObject uiInventory02;
	public Button InventoryTutoButton02;
	public GameObject uiInventory03;

	public GameObject uiMessages;
	public GameObject uiMessages01;
	public Button MessagesTutoButton01;
	public GameObject uiMessages02;
	public Button MessagesTutoButton02;

	public GameObject InventoryButtonGO;

	public GameObject uiDecision;
	public GameObject TutoGoDecision;
	public GameObject UiInventoryArrowGoDecision;

	public Button GoDecisionButton;
	public GameObject GoDecisionObject;
	public GameObject Highlight;
	public GameObject DecisionTuto01;
	public Button DecisionButton01;
	public GameObject DecisionCible01;
	public GameObject DecisionCible02;

    public GameObject messagesContent;
    public GameObject messageRxTutoPrefab;
    public GameObject messageTxTutoPrefab;

    bool TutoProgress01;
	bool TutoProgress02;
	bool TutoProgress03;
    bool IsDone;

    // Instance
    public static Tuto Tutos;
	public static Tuto instance;

    public static string[] SECRET_TEXTS_PROFILE = { "Vous nous aidez à conquérir le monde du jeu de société", "Vous avez infiltré la secte des Gobelins", "Vous avez participé à la diffusion de Secret Societies", "Vous avez intimidé le jury pour qu'il nous mette de bonnes notes", "Vous avez acheté tous les jeux de Thierry Perreau" };
    public static string[] SECRET_TEXTS = { "Secret1", "Secret2", "Secret3", "Secret4", "Secret5" };
    public static int[] SECRET_IDS = { 1, 2, 3, 4, 5 };
    public static string GENERIC_NAME = "Toto";

    // Use this for initialization
    void Start () {
		TutoButton.onClick.AddListener(GoTuto);
    }

    public void SetProfile()
    {
        List<Secret> secretsList = new List<Secret>();
        for (int j = 0; j < 5; j++)
        {
            Secret secret = new Secret(j == 0 ? true : false, SECRET_TEXTS_PROFILE[j], SECRET_TEXTS[j], SECRET_IDS[j]);
            secretsList.Add(secret);
        }

        int i = 0;
        foreach (Transform mySecret in secretsContent.transform)
        {
            Secret secret = secretsList[i];

            GameObject textSecret = mySecret.transform.FindChild("TextSecret").gameObject;
            GameObject image = mySecret.transform.FindChild("Image").gameObject;
            GameObject secretBorder = mySecret.transform.FindChild("SecretBorder").gameObject;
            GameObject sharedImage = mySecret.transform.FindChild("Shared").gameObject;


            
            mySecret.gameObject.GetComponent<Button>().onClick.AddListener(delegate { OpenSecretPopup(secret.secretTextProfile, secret.secretID, secret.shared); });
            textSecret.GetComponent<Text>().text = secret.secretTextProfile;

            GameObject color = image.transform.FindChild("Color").gameObject;
            GameObject icon = image.transform.FindChild("Icon").gameObject;
            GameObject chest = image.transform.FindChild("Chest").gameObject;
            GameObject holder = image.transform.FindChild("Holder").gameObject;

            Sprite spriteIcon = secret.spriteIcon;
            Texture2D texture;
            if (spriteIcon == null)
            {
                texture = Resources.Load("Secrets/Icons/icon-tuto-" + secret.secretID) as Texture2D;
                spriteIcon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                secret.spriteIcon = spriteIcon;
            }
            icon.GetComponent<Image>().sprite = spriteIcon;

            if (secret.shared)
            {
                sharedImage.SetActive(true);
            }
            else
            {
                sharedImage.SetActive(false);
            }
            i++;
        }

        Instantiate(messageRxTutoPrefab, messagesContent.transform);
        Instantiate(messageTxTutoPrefab, messagesContent.transform);
    }

    public void OpenSecretPopup(string secretText, int imageID, bool shared)
    {
        Texture2D texture = Resources.Load("Secrets/Icons/icon-tuto-" + imageID) as Texture2D;
        Transform secretIcon = secretPopup.transform.FindChild("SecretIcon");
        GameObject sharedText = secretPopup.transform.FindChild("Shared").gameObject;
        if (!shared)
        {
            sharedText.SetActive(false);
        }
        else
        {
            sharedText.SetActive(true);
        }

        GameObject color = secretIcon.FindChild("Color").gameObject;
        GameObject icon = secretIcon.FindChild("Icon").gameObject;
        GameObject chest = secretIcon.FindChild("Chest").gameObject;
        GameObject holder = secretIcon.FindChild("Holder").gameObject;

        icon.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        secretPopup.transform.FindChild("SecretText").GetComponent<Text>().text = secretText;
        secretPopup.transform.FindChild("CloseButton").GetComponent<Button>().onClick.RemoveAllListeners();
        secretPopup.transform.FindChild("CloseButton").GetComponent<Button>().onClick.AddListener(delegate { secretPopup.SetActive(false); });
        secretPopup.SetActive(true);
    }

    public void GoTuto() {
        UiMainController.instance.inTuto = true;
        TutoGoDecision.SetActive(true);
        ChangeScreen.SetButtons();
        SetProfile();
        GoDecisionButton.onClick.AddListener(CantGo);
        UiTutoScreen01.SetActive(true);
		uiStartGame.SetActive (false);
		ButtonScreen01.onClick.AddListener(GoMainScreenTuto01);
        VuforiaController.instance.OKScanTuto.gameObject.SetActive(false);
    }

    public void GoMainScreenTuto01(){
		UiTutoScreen01.SetActive(false);
		uiMain.SetActive(true);
		uiInventory.SetActive(true);
		uiInventory01.SetActive(true);
		InventoryTutoButton01.onClick.AddListener(GoMainScreenTuto02);
	}

	public void GoMainScreenTuto02(){
		uiInventory01.SetActive(false);
		uiInventory02.SetActive(true);
		InventoryTutoButton02.onClick.AddListener(HideInventoryTuto);
	}

	public void HideInventoryTuto(){
		TutoProgress01 = true;
		
		uiInventory.SetActive(false);
		uiInventory03.SetActive(true);
		ProfilTuto.SetActive (true);
		uiProfil01.SetActive (true);
		ProfilButton01.onClick.AddListener (GoProfilTuto01);

		uiMessages.SetActive (true);
		uiMessages01.SetActive (true);
		MessagesTutoButton01.onClick.AddListener (GoMessage02);
	}

	public void GoProfilTuto01(){
		uiProfil01.SetActive (false);
		uiProfil02.SetActive (true);
		ProfilButton02.onClick.AddListener (GoProfilTuto02);
	}

	public void GoProfilTuto02(){
		uiProfil02.SetActive (false);
		uiProfil03.SetActive (true);
		ProfilButton03.onClick.AddListener (GoProfilTuto03);
	}

	public void GoProfilTuto03(){
		uiProfil03.SetActive (false);
		uiProfil04.SetActive (true);
		ProfilButton04.onClick.AddListener (GoProfilTuto04);
	}

	public void GoProfilTuto04(){
		uiProfil04.SetActive (false);
		//uiProfil05.SetActive (true);
		TutoProgress02 = true;
	}
		
	public void GoMessage02(){
		uiMessages01.SetActive (false);
		uiMessages02.SetActive (true);
		MessagesTutoButton02.onClick.AddListener (HideMessagesTuto);
	}

	public void HideMessagesTuto(){
		uiMessages02.SetActive (false);
		uiMessages.SetActive (false);
		TutoProgress03 = true;
        InventoryButtonGO.SetActive(false);
        uiInventory03.SetActive(false);
        UiInventoryArrowGoDecision.SetActive(true);
        GoDecisionButton.onClick.RemoveAllListeners();
        GoDecisionButton.onClick.AddListener(GoDecisionPhase);
        InventoryButtonGO.SetActive(false);
    }

    public void GoDecisionPhase() {
		uiMain.SetActive (false);
		UiInventoryArrowGoDecision.SetActive (false);
		uiDecision.SetActive (true);
		GoDecisionObject.SetActive (false);
		Highlight.SetActive (false);
		DecisionTuto01.SetActive (true);
		DecisionButton01.onClick.AddListener (Decision01);
	}

	public void Decision01() {
		DecisionTuto01.SetActive (false);
		DecisionCible01.SetActive (true);
	}

	public void Decision02() {
		DecisionCible01.SetActive (false);
	}

	void CantGo() {
		InventoryButtonGO.SetActive (true);
	}
}
