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

    //이게 사실상 개조할수 있는 심장이라고 생각해야할듯

    public NetworkPlayer playerPrefab;

    CharacterInputhandler characterInputhandler;


    // Start is called before the first frame update
    void Start()
    {
        

    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer) //연결되어있으면
        {
            runner.Spawn(playerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, player);
            //생성해
        }
        
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //입력은 이 함수에서만 하는걸로 보임 

        //Local != null  >> 게임 오브젝트와 상호작용할 수 있는 플레이어   
        if (characterInputhandler == null && NetworkPlayer.Local != null )
            characterInputhandler = NetworkPlayer.Local.GetComponent<CharacterInputhandler>();
        // >> NetworkPlayer 는 생성된 게임 오브젝트 캐릭터에 붙어있음   
        // runner 가 생성된 플레이어

        if (characterInputhandler != null)
        {
            // NetworkInput.Set(NetworkInputData) 꼴로 보임 
            input.Set(characterInputhandler.GetNetworkInput());
            //네트워크 인풋 전송하는 방식으로 보임 
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
        //연결요청
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

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
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
