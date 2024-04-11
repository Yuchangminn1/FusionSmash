using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
public class PlayerInfoUIPrefab : NetworkBehaviour
{
    public TMP_Text playerName;
    public TMP_Text playerForce;
    public Image playerNumberIcon;
    public Sprite[] iconSprite;
    public void SetPlayerInfo(int _playerNum, string name, Transform _tr,int force =1)
    {
        playerName.text = name;
        playerForce.text = force.ToString();
        playerNumberIcon.sprite = iconSprite[_playerNum - 1];
        transform.parent = _tr;

    }
    public void ForceUpdate(int force = 1)
    {
        playerForce.text = force.ToString();
    }
}
