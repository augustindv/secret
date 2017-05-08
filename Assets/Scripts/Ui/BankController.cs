using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ChangeBankEvent : UnityEvent<int>
{
}

public class BankController : MonoBehaviour
{

    // Instance
    public static BankController bankController;
    public static BankController instance
    {
        get
        {
            if (!bankController)
            {
                bankController = FindObjectOfType(typeof(BankController)) as BankController;

                if (!bankController)
                {
                    Debug.LogError("There needs to be one active BankController script on a GameObject in your scene.");
                }
            }

            return bankController;
        }
    }

    public ChangeBankEvent changeBankEmitter;

    public delegate void ChangeBank(int newBank);
    public event ChangeBank BankChangedHandler;

    public void BankHasChanged(int newBank)
    {
        if (BankChangedHandler != null)
            BankChangedHandler(newBank);

        changeBankEmitter.Invoke(newBank);
    }
}
