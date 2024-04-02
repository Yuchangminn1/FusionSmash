using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToLobby : MonoBehaviour
{
    public void ExitBuutonClick()
    {
        NetworkRunnerHandler.Instance.ReturnToLobby();
    }
}
