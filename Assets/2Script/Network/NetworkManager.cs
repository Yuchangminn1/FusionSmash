using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

using static System.Collections.Specialized.BitVector32;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner runnerInstance;
    public string lobbyName = "default";
    public Transform sessionListContentParent;
    public GameObject sessionListEntryPrefab;
    public Dictionary<string, GameObject> sessionListUIDictionary = new Dictionary<string, GameObject>();
    public GameObject PlayerPrefab;

    string mySceneName = "MyS2";
    string myLobbySceneName = "Lobby";

    CharacterInputhandler characterInputhandler;




    //public List<SessionListEntry> sessionListEntryList = new List<SessionListEntry>();
    private void Awake()
    {
        runnerInstance = gameObject.GetComponent<NetworkRunner>();
        if (runnerInstance == null)
        {
            runnerInstance = gameObject.AddComponent<NetworkRunner>();
        }
    }
    private void Start()
    {
        runnerInstance.JoinSessionLobby(SessionLobby.Shared, lobbyName);
    }

    public void CreateRandomSession()
    {
        int randomInt = UnityEngine.Random.Range(1000, 9999);
        string randomSessionName = "Room-" + randomInt.ToString();
        runnerInstance.StartGame(new StartGameArgs()
        {
            Scene = (SceneManager.GetSceneByName(mySceneName).buildIndex),
            //Scene = (SceneManager.GetSceneByName("MyS2").buildIndex),
            SessionName = randomSessionName,
            GameMode = GameMode.AutoHostOrClient,
        });
    }

    public static void ReturnTOLobby()
    {
        //NetworkManager.runnerInstance.Despawn(runnerInstance.GetPlayerObject(runnerInstance.LocalPlayer));
        NetworkManager.runnerInstance.Shutdown(true, ShutdownReason.Ok);
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SceneManager.LoadScene(myLobbySceneName);
    }
    //public int GetSceneIndex(string sceneName)
    //{
    //    for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
    //    {
    //        string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

    //        string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
    //        if (name == sceneName)
    //        {
    //            return i;
    //        }

    //    }
    //    return -1;
    //}
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Debug.Log("player Joined");
            StartCoroutine(LoadSceneAndSpawnPlayer(mySceneName, runner,PlayerPrefab,player));

            //Destroy(gameObject);
        }
        //StartCoroutine(LoadSceneAndSpawnPlayer(mySceneName, runner, PlayerPrefab, player));
        //else
        //{
        //    Debug.Log("player Joined");
        //    SceneManager.LoadScene("MyS2");
        //    NetworkObject playerNetworkObject = runner.Spawn(PlayerPrefab, Vector3.zero);
        //    runner.SetPlayerObject(player, playerNetworkObject);
        //}

    }
    public IEnumerator LoadSceneAndSpawnPlayer(string mySceneName, NetworkRunner runner, GameObject PlayerPrefab, PlayerRef player)
    {
        // 비동기로 씬 로딩
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mySceneName);

        // 로딩이 완료될 때까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 씬 로딩 완료 후 플레이어 스폰
        Debug.Log("Scene loaded, spawning player...");
        NetworkObject playerNetworkObject = runner.Spawn(PlayerPrefab, Vector3.zero, Quaternion.identity, inputAuthority: runner.LocalPlayer);

        if (playerNetworkObject != null)
        {
            Debug.Log("Player spawned successfully.");
            characterInputhandler = playerNetworkObject.GetComponent<CharacterInputhandler>();
            //�̸� �ٲ�
            runner.name = "Network Runner";
            //��Ʈ��ũ ���� ���� 
            var clienTask = InitializeNetworkRunner(runnerInstance, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
            Debug.Log("sever networkRunner started.");
        }
        else
        {
            Debug.Log("Failed to spawn player.");
        }
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        DeleteOldSessionsFromUI(sessionList);

        CompareLists(sessionList);


    }
    private void CompareLists(List<SessionInfo> sessionList)
    {
        //foreach (SessionInfo session in sessionList)
        //{
        //    //if (sessionListUIDictionary.ContainsKey(session.Name))
        //    //{
        //    //    UpdateEntryUI(session);
        //    //}
        //    //else
        //    //{
        //    //    CreateEntryUI(session);
        //    //}
        //}
    }
    //private void UpdateEntryUI(SessionInfo session)
    //{

    //    sessionListUIDictionary.TryGetValue(session.Name, out GameObject newEntry);
    //    SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();

    //    entryScript.roomName.text = session.Name;
    //    entryScript.playerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
    //    entryScript.joinButton.interactable = session.IsOpen;

    //    newEntry.SetActive(session.IsVisible);
    //}
    //private void CreateEntryUI(SessionInfo session)
    //{
    //    GameObject newEntry = GameObject.Instantiate(sessionListEntryPrefab);
    //    newEntry.transform.parent = sessionListContentParent;
    //    SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();
    //    sessionListUIDictionary.Add(session.Name, newEntry);

    //    entryScript.roomName.text = session.Name;
    //    entryScript.playerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
    //    entryScript.joinButton.interactable = session.IsOpen;

    //    newEntry.SetActive(session.IsVisible);
    //}

    //public void JoinGameAA(string _SessionName)
    //{
    //    NetworkManager.runnerInstance.StartGame(new StartGameArgs()
    //    {
    //        Scene = (SceneManager.GetSceneByName(mySceneName).buildIndex),
    //        //Scene = (SceneManager.GetSceneByName("MyS2").buildIndex),
    //        SessionName = _SessionName,
    //        GameMode = GameMode.AutoHostOrClient,
    //    });
    //}

    private void DeleteOldSessionsFromUI(List<SessionInfo> sessionList)
    {
        //bool isContained = false;
        //GameObject uiToDelete = null;
        //foreach (KeyValuePair<string, GameObject> kvp in sessionListUIDictionary)
        //{
        //    string sessionKey = kvp.Key;
        //    foreach (SessionInfo sessionInfo in sessionList)
        //    {
        //        if (sessionInfo.Name == sessionKey)
        //        {
        //            isContained = true;
        //            break;
        //        }
        //    }
        //    if (!isContained)
        //    {
        //        uiToDelete = kvp.Value;
        //        sessionListUIDictionary.Remove(sessionKey);
        //        Destroy(uiToDelete);
        //    }
        //}
        // 삭제할 UI의 키를 저장할 리스트
        List<string> keysToDelete = new List<string>();

        // 모든 세션 UI를 검사
        foreach (KeyValuePair<string, GameObject> kvp in sessionListUIDictionary)
        {
            string sessionKey = kvp.Key;
            bool isContained = false; // 기본값을 false로 설정

            // 현재 세션 UI가 sessionList에 포함되어 있는지 검사
            foreach (SessionInfo sessionInfo in sessionList)
            {
                if (sessionInfo.Name == sessionKey)
                {
                    isContained = true;
                    break;
                }
            }

            // 포함되어 있지 않다면 삭제 리스트에 추가
            if (!isContained)
            {
                keysToDelete.Add(sessionKey);
            }
        }

        // 삭제 리스트를 기반으로 세션 UI 삭제
        foreach (string key in keysToDelete)
        {
            GameObject uiToDelete = sessionListUIDictionary[key];
            sessionListUIDictionary.Remove(key);
            Destroy(uiToDelete);
        }
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        ;
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        ;
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        ;
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        ;
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        ;
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        ;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //Local != null  >> ���� ������Ʈ�� ��ȣ�ۿ��� �� �ִ� �÷��̾�   
        if (characterInputhandler == null && NetworkPlayer.Local != null)
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

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        ;
    }


    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        ;
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        ;
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        ;
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        ;
    }





    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        ;
    }
    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        //���� �Լ���� �����ϰ� �Ѿ 
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
        if (sceneManager == null)
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }
        runner.ProvideInput = true;
        
        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            //Scene = scene,
            SessionName = "TestRoom",
            Initialized = initialized,
            SceneManager = sceneManager
        });
        
    }

}
