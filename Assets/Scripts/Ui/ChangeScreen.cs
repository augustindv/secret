﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeScreen : MonoBehaviour {

    public Sprite[] buttonsSprites;

    private Button[] buttons;

    void Awake()
    {
        buttons = UiMainController.instance.buttons;
        buttons[0].image.overrideSprite = buttonsSprites[0];
        buttons[2].image.overrideSprite = buttonsSprites[2];
    }

    public void ChangeTabImage(int newScreen)
    {
        switch (newScreen)
        {
            case -1:
                buttons[0].image.overrideSprite = null;
                buttons[1].image.overrideSprite = buttonsSprites[1];
                buttons[2].image.overrideSprite = buttonsSprites[2];
                break;
            case 0:
                buttons[0].image.overrideSprite = buttonsSprites[0];
                buttons[1].image.overrideSprite = null;
                buttons[2].image.overrideSprite = buttonsSprites[2];
                break;
            case 1:
                buttons[0].image.overrideSprite = buttonsSprites[0];
                buttons[1].image.overrideSprite = buttonsSprites[1];
                buttons[2].image.overrideSprite = null;
                break;
            default:
                break;
        }
    }
}