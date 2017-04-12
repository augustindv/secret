using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ChangeScreenEvent : UnityEvent<int>
{
}

public class UiMainController : MonoBehaviour {

    public ChangeScreenEvent onChangeScreen;

    public delegate void ChangeScreen(int newScreen);
    public event ChangeScreen ScreenChanged;

    private int _actualScreen;
    public int actualScreen // 0 = Middle screen, -1 = left; 1 = right;
    {
        get
        {
            return _actualScreen;
        }
        set
        {
            if (value != _actualScreen)
            {
                _actualScreen = value; 
                if (ScreenChanged != null)
                    ScreenChanged(_actualScreen); 
                onChangeScreen.Invoke(_actualScreen);
            }
        }
    }

    public GameObject movingPanel; // Panel moving with swipe and click;
    public GameObject uiMain;
    public GameObject uiPublishing;
    public GameObject uiAuction;
    public GameObject uiDecision;
    public GameObject uiRevelation;

    private float startTime;
    private Vector2 startPos;
    private bool couldBeSwipe;
    private float comfortZone = 1000;
    private float minSwipeDistX = 200;
    private float minSwipeDistY = 100;
    private float maxSwipeTime = 1;

    public Button[] buttons;

    [HideInInspector]
    public Player localPlayer;

    // Instance
    public static UiMainController uiMainController;
    public static UiMainController instance
    {
        get
        {
            if (!uiMainController)
            {
                uiMainController = FindObjectOfType(typeof(UiMainController)) as UiMainController;

                if (!uiMainController)
                {
                    Debug.LogError("There needs to be one active StartGameController script on a GameObject in your scene.");
                }
            }

            return uiMainController;
        }
    }

    void Awake()
    {
        buttons[0].onClick.AddListener(delegate { Move(-1); });
        buttons[1].onClick.AddListener(delegate { Move(0); });
        buttons[2].onClick.AddListener(delegate { Move(1); });

        // TODO : Place the screen at the right place at the start...
       /* float width = movingPanel.transform.FindChild("Inventory").GetComponent<RectTransform>().rect.width;
        movingPanel.transform.FindChild("Profile").GetComponent<RectTransform>().offsetMax = new Vector2(-Screen.width, 0);
        movingPanel.transform.FindChild("Profile").GetComponent<RectTransform>().offsetMin = new Vector2(-Screen.width, 0);
        movingPanel.transform.FindChild("Message").GetComponent<RectTransform>().offsetMax = new Vector2(Screen.width, 0);
        movingPanel.transform.FindChild("Message").GetComponent<RectTransform>().offsetMin = new Vector2(Screen.width, 0);*/
    }

    public void ResetUiMain()
    {
        Move(0);
        InventoryController.instance.ResetUiInventory();
    }

    void Move(int screenToGo)
    {
        int swipeDirection = 0;
        if (actualScreen == -1 && screenToGo == 0) {
            swipeDirection = -1;
        } else if (actualScreen == -1 && screenToGo == 1)
        {
            swipeDirection = -2;
        } else if (actualScreen == 0 && screenToGo == -1)
        {
            swipeDirection = 1;
        } else if (actualScreen == 0 && screenToGo == 1)
        {
            swipeDirection = -1;
        } else if (actualScreen == 1 && screenToGo == 0)
        {
            swipeDirection = 1;
        } else if (actualScreen == 1 && screenToGo == -1)
        {
            swipeDirection = 2;
        }

        if (swipeDirection != 0)
        {
            StartCoroutine(MoveCanvas(swipeDirection * Screen.width, 0.3f));
            actualScreen = screenToGo;
        }
    }

    bool CanSwipe(int swipeDirection)
    {
        return (swipeDirection > 0 && actualScreen == 0) || (swipeDirection > 0 && actualScreen == 1) || (swipeDirection < 0 && actualScreen == 0) || (swipeDirection < 0 && actualScreen == -1);
    }

    void SetActualScreen(int swipeDirection)
    {
        if (swipeDirection > 0 && actualScreen == 0)
        {
            actualScreen = -1;
        } else if (swipeDirection > 0 && actualScreen == 1)
        {
            actualScreen = 0;
        } else if (swipeDirection < 0 && actualScreen == 0)
        {
            actualScreen = 1;
        } else if (swipeDirection < 0 && actualScreen == -1)
        {
            actualScreen = 0;
        }
    }

    IEnumerator MoveCanvas(float width, float time)
    {
        float elapsedTime = 0;
        Vector3 startingPos = movingPanel.transform.position;
        Vector3 newPosition = movingPanel.transform.position + new Vector3(width, 0, 0);
        while (elapsedTime < time)
        {
            movingPanel.transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    void Update()
    {
        if (Input.touches.Length > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    couldBeSwipe = true;
                    startPos = touch.position;
                    startTime = Time.time;

                    break;

                case TouchPhase.Moved:
                    if (Mathf.Abs(touch.position.x - startPos.x) > comfortZone)
                    {
                        couldBeSwipe = false;
                    }
                    break;

                case TouchPhase.Ended:
                    var swipeTime = Time.time - startTime;
                    var swipeDistX = Mathf.Abs(touch.position.x - startPos.x);
                    var swipeDistY = Mathf.Abs(touch.position.y - startPos.y);
                    if (couldBeSwipe && (swipeTime < maxSwipeTime) && (swipeDistX > minSwipeDistX) && (swipeDistY < minSwipeDistY))
                    {
                        // It's a swiiiiiiiiiiiipe!
                        int swipeDirection = (int) Mathf.Sign(touch.position.x - startPos.x);
                        couldBeSwipe = false;
                        // Do something here in reaction to the swipe.
                        if (CanSwipe(swipeDirection))
                        {
                            StartCoroutine(MoveCanvas(swipeDirection * Screen.width, 0.3f));
                            SetActualScreen(swipeDirection);
                        }
                    }
                    break;
            }
        }
    }
}
