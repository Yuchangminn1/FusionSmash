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


    // Start is called before the first frame update
    void Start()
    {
        

    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer) //����Ǿ�������?
        {
            runner.Spawn(playerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, player);

        }
        
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //�Է��� �� �Լ������� �ϴ°ɷ� ���� 

        //Local != null  >> ���� ������Ʈ�� ��ȣ�ۿ��� �� �ִ� �÷��̾�   
        if (characterInputhandler == null && NetworkPlayer.Local != null )
            characterInputhandler = NetworkPlayer.Local.GetComponent<CharacterInputhandler>();
        // >> NetworkPlayer �� ������ ���� ������Ʈ ĳ���Ϳ� �پ�����   
        // runner �� ������ �÷��̾�

        if (characterInputhandler != null)
        {
            // NetworkInput.Set(NetworkInputData) �÷� ���� 
            input.Set(characterInputhandler.GetNetworkInput());
            //��Ʈ��ũ ��ǲ �����ϴ� �������?���� 
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
