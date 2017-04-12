using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageAddDst : MonoBehaviour {
    public void Add(string name)
    {
        GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(name));
    }
}
