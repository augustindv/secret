using UnityEngine;
using UnityEngine.Networking;

public class Bank : NetworkBehaviour {

    [SyncVar (hook = "onChangeBank")]
    public int bankMoney = 0;
    [SyncVar]
    public float totalMoneyInGame = 70f;

    public static Bank bank;
    public static Bank instance
    {
        get
        {
            if (!bank)
            {
                bank = FindObjectOfType(typeof(Bank)) as Bank;

                if (!bank)
                {
                    Debug.LogError("There needs to be one active Bank script on a GameObject in your scene.");
                }
            }

            return bank;
        }
    }

    void onChangeBank(int value)
    {
        BankController.instance.BankHasChanged(value);    
    }

    // Use this for initialization
    void Start () {
        totalMoneyInGame = bankMoney + (PlayerDatabase.instance.GetNumberOfPlayers() * 10);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
