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
    public GameObject uiNothingPublished;
    public GameObject uiAuction;
    public GameObject uiDecision;
    public GameObject uiRevelation;
    public GameObject uiStartGame;
    public GameObject uiVuforia;

    private float startTime;
    private Vector2 startPos;
    private bool couldBeSwipe;
    private float comfortZone = 1000;
    private float minSwipeDistX = 200;
    private float minSwipeDistY = 100;
    private float maxSwipeTime = 1;

    public Button[] buttons;
    public Sprite[] buttonsSprites;

    public Animator animator;

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

        animator = movingPanel.GetComponent<Animator>();

        uiStartGame.SetActive(true);
    }

    public void SetActiveAllCanvas(bool active)
    {
        uiMain.SetActive(active);
        uiPublishing.SetActive(active);
        uiAuction.SetActive(active);
        uiDecision.SetActive(active);
        uiRevelation.SetActive(active);
        uiNothingPublished.SetActive(active);
        uiStartGame.SetActive(active);
        uiVuforia.SetActive(active);
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
            animator.SetTrigger("-1_0");
        } else if (actualScreen == -1 && screenToGo == 1)
        {
            swipeDirection = -2;
            animator.SetTrigger("-1_1");
        }
        else if (actualScreen == 0 && screenToGo == -1)
        {
            swipeDirection = 1;
            animator.SetTrigger("0_-1");
        }
        else if (actualScreen == 0 && screenToGo == 1)
        {
            swipeDirection = -1;
            animator.SetTrigger("0_1");
        }
        else if (actualScreen == 1 && screenToGo == 0)
        {
            swipeDirection = 1;
            animator.SetTrigger("1_0");
        }
        else if (actualScreen == 1 && screenToGo == -1)
        {
            swipeDirection = 2;
            animator.SetTrigger("1_-1");
        }

        if (swipeDirection != 0)
        {
            actualScreen = screenToGo;
        }
    }

    public int GetScreenToGo(int swipeDirection)
    {
        if (swipeDirection > 0 && actualScreen == 0)
        {
            return -1;
        }
        else if (swipeDirection > 0 && actualScreen == 1)
        {
            return 0;
        }
        else if (swipeDirection < 0 && actualScreen == 0)
        {
            return 1;
        }
        else if (swipeDirection < 0 && actualScreen == -1)
        {
            return 0;
        }

        return 0;
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

    /*IEnumerator MoveCanvas(float width, float time)
    {
        float elapsedTime = 0;
        Vector3 startingPos = movingPanel.transform.position;
        Vector3 newPosition = movingPanel.transform.position + new Vector3(width, 0, 0);
        while (elapsedTime < time)
        {
            //movingPanel.transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }*/

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
                            Move(GetScreenToGo(swipeDirection));
                        }
                    }
                    break;
            }
        }
    }
}
