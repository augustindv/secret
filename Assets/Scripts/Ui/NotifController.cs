using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class ChangeNotifEvent : UnityEvent<string>
{
}

public class NotifController : MonoBehaviour {

    public static NotifController notifController;
    public static NotifController instance
    {
        get
        {
            if (!notifController)
            {
                notifController = FindObjectOfType(typeof(NotifController)) as NotifController;

                if (!notifController)
                {
                    Debug.LogError("There needs to be one active NotifController script on a GameObject in your scene.");
                }
            }

            return notifController;
        }
    }

    public List<GameObject> secrets = new List<GameObject>();

    private float time = 0;
    private static int UPDATE_RATE = 15;

    public ChangeNotifEvent onChangeNotif;

    public delegate void ChangeNotif(string newText);
    public event ChangeNotif NotifChanged;

    public void NotifHasChanged(string newText)
    {
        if (NotifChanged != null)
            NotifHasChanged(newText);

        onChangeNotif.Invoke(newText);
    }

    public void UpdateNotif()
    {
        time = 0;
        if (secrets.Count == 0)
        {
            secrets = GameObject.FindGameObjectsWithTag("Secret").ToList();
        }

        if (secrets.Count != 0)
        {
            Secret secret = secrets[Random.Range(0, secrets.Count - 1)].GetComponent<Secret>();
            NotifHasChanged(secret.secretTextClue);
        }
    }

    public void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        if (time >= UPDATE_RATE)
        {
            UpdateNotif();
        }
    }
}
