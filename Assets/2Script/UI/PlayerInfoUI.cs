using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    public TMP_Text playerName;
    public TMP_Text playerCurrentForce;
    Transform layoutGroup;

    // Start is called before the first frame update
    void Start()
    {
        layoutGroup = GameObject.Find("PlayerInfoGroup").transform;
        if (layoutGroup == null)
        {
            Debug.Log("GameObject.Find Error");
        }
        else
        {
            transform.SetParent(layoutGroup, false);
        }
    }

    // Update is called once per frame
    
    public void SetPlayerName(string _name)
    {
        playerName.text = _name;
    }

    public void SetPlayerForce(int _force)
    {
        playerCurrentForce.text = _force*10+ "%";
    }

    public void InitialPlayerInfo(string _name, int _force)
    {
        playerName.text = _name;
        playerCurrentForce.text = _force * 10 + "%";

    }
}
