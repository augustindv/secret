using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBank : MonoBehaviour
{
    public Text decisionBank;

    public void ChangeBankInt(int newBank)
    {
        decisionBank.text = newBank.ToString();
    }
}
