using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ChangeMoneyEvent : UnityEvent<int>
{
}

public class MoneyController : MonoBehaviour {

    // Instance
    public static MoneyController moneyController;
    public static MoneyController instance
    {
        get
        {
            if (!moneyController)
            {
                moneyController = FindObjectOfType(typeof(MoneyController)) as MoneyController;

                if (!moneyController)
                {
                    Debug.LogError("There needs to be one active MoneyController script on a GameObject in your scene.");
                }
            }

            return moneyController;
        }
    }

    public ChangeMoneyEvent onChangeMoney;

    public delegate void ChangeMoney(int newMoney);
    public event ChangeMoney MoneyChanged;

    public void MoneyHasChanged(int newMoney)
    {
        if (MoneyChanged != null)
            MoneyHasChanged(newMoney);

        onChangeMoney.Invoke(newMoney);
    }
}
