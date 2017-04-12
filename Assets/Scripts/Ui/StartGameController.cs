using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameController : MonoBehaviour {

    public Canvas uiStartGame;
    public InputField nameField;
    public Dropdown deckChoice;

    public static StartGameController startGameController;

    public static StartGameController instance
    {
        get
        {
            if (!startGameController)
            {
                startGameController = FindObjectOfType(typeof(StartGameController)) as StartGameController;

                if (!startGameController)
                {
                    Debug.LogError("There needs to be one active StartGameController script on a GameObject in your scene.");
                }
            }

            return startGameController;
        }
    }

    public void SetActiveUIStartGame(bool active)
    {
        uiStartGame.gameObject.SetActive(active);
    }
}

