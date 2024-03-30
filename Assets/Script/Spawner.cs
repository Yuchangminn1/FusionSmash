using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.Diagnostics;
using TMPro;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{

    //�̰� ��ǻ�?�����Ҽ� �ִ� �����̶��?�����ؾ��ҵ�

    public NetworkPlayer playerPrefab;

    CharacterInputhandler characterInputhandler;

    SessionListUIHandler sessionListUIHandler;

    // Start is called before the first frame update

    private void Awake()
    {
        sessionListUIHandler = FindObjectOfType<SessionListUIHandler>(true);
    }
    void Start()
    {
        

    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if(sessionListUIHandler == null)
            return;
        if(sessionList.Count == 0)
        {
            Debug.Log("Joined lobby no session found");
            sessionListUIHandler.OnNoSessionFound();
        }
        else
        {
            sessionListUIHandler.ClearList();

            foreach (SessionInfo sessionInfo in sessionList)
            {
                sessionListUIHandler.AddToList(sessionInfo);

                Debug.Log($"Found session {sessionInfo.Name} playerCount {sessionInfo.PlayerCount}");
            }
        }
    }
    IEnumerator CallSpawnedCo()
    {
        yield return new WaitForSeconds(0.5f);
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer) //����Ǿ�������?
        {
            runner.Spawn(playerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, player);
        }
        else
        {
            Debug.Log("OnPlayerJoined");
        }
        
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

        if (characterInputhandler == null && NetworkPlayer.Local != null )
            characterInputhandler = NetworkPlayer.Local.GetComponent<CharacterInputhandler>();

        if (characterInputhandler != null)
        {
            input.Set(characterInputhandler.GetNetworkInput());
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
       // throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //������?
       // throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
       // throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
       // throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        //throw new NotImplementedException();
    }

    

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
       // throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
       // throw new NotImplementedException();
    }

    

    

}
