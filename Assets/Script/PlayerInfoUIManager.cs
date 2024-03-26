using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using Fusion;
public class PlayerInfoUIManager : NetworkBehaviour
{
    public NetworkObject playerInfoPrefab;
    public PlayerInfoUI[] playerInfoUI = new PlayerInfoUI[4];

    //public TMP_Text playerName;
    //public TMP_Text playerForce;
    //public Image playerNumberIcon;
    // Start is called before the first frame update
    
    public void AddPlayerInfo(int _playerNum, string name, int force = 1)
    {
        if (HasStateAuthority)
        {
            NetworkObject tmp = Runner.Spawn(playerInfoPrefab);
            tmp.GetComponent<PlayerInfoUI>().SetPlayerInfo(_playerNum, name, transform, force);
        }
        
    }
    
}
