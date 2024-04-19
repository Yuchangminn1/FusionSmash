using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;
using Unity.VisualScripting;
//using UnityEditor.Build.Content;


public class NetworkRunnerHandler : MonoBehaviour
{
    public static NetworkRunnerHandler Instance { get; private set; }
    public NetworkRunner networkRunnerPrefab;
    bool isNew = true;
    NetworkRunner networkRunner;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 여기에 네트워크 초기화 및 설정 코드 추가
        }
        else if (Instance != this)
        {
            Instance.isNew = false;
            Instance.CMSpawnNetworkRunner();
            Instance.Start();
            Destroy(gameObject);
            return;
        }
        CMSpawnNetworkRunner();
    }
    public bool GetIsNew() => isNew;
    void CMSpawnNetworkRunner()
    {
        NetworkRunner networkRunnerInScene = FindObjectOfType<NetworkRunner>();

        if (networkRunnerInScene != null)
            networkRunner = networkRunnerInScene;
    }

    void Start()
    {
        if(networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network Runner";
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                var clienTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient,"TestSession", NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
            }
            Debug.Log("sever networkRunner started.");
        }
    }
    

    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if(sceneManager == null)
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }
        return sceneManager;

    }

    public void OnJoinLobby()
    {
        var clientTask = JoinLobby();
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode,string sessionName, NetAddress address ,SceneRef scene,Action<NetworkRunner> initialized)
    {
        //���� �Լ���� �����ϰ� �Ѿ 
        var sceneManager = GetSceneManager(runner);
        runner.ProvideInput = true;
        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = sessionName,
            CustomLobbyName = "OurLobbyID",
            Initialized = initialized,
            SceneManager = sceneManager,
        }) ;
    }


    public void ReturnToLobby()
    {
        StartCoroutine(ReturnToLobbyCoroutine());
    }

    // 비동기 작업을 사용하여 네트워크 연결 종료 및 씬 전환 수행
    private IEnumerator ReturnToLobbyCoroutine()
    {
        // 네트워크 연결이 활성화되어 있는 경우, 연결 종료
        if (networkRunner != null && networkRunner.IsRunning)
        {
            networkRunner.Shutdown();
            while (networkRunner.IsRunning)
            {
                // 네트워크 종료가 완료될 때까지 대기
                yield return null;
            }
        }

        // 로비 씬으로 전환
        SceneManager.LoadScene("MainMenu"); // "LobbySceneName"을 실제 로비 씬 이름으로 바꿔주세요.
    }


    private async Task JoinLobby()
    {
        Debug.Log("JoinLobby Started");

        string lobbyID = "OurLobbyID";

        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);

        if (!result.Ok)
        {
            Debug.Log($"Unable to join lobby {lobbyID}");
        }
        else
        {
            Debug.Log("joinLobby ok");
        }
    }

    public void CreateGame(string sessionName, string sceneName)
    {
        Debug.Log($"Create session {sessionName} scene {sceneName} build Index {SceneUtility.GetBuildIndexByScenePath($"0Scenes/{sceneName}")}");

        var clinetTask = InitializeNetworkRunner(networkRunner, GameMode.Host, sessionName, NetAddress.Any(), SceneUtility.GetBuildIndexByScenePath($"0Scenes/{sceneName}"),null);
    }

    public void JoinGame(SessionInfo sessionInfo)
    {
        Debug.Log($"Join session {sessionInfo.Name}");

        var clinetTask = InitializeNetworkRunner(networkRunner, GameMode.Client, sessionInfo.Name, NetAddress.Any(), SceneUtility.GetBuildIndexByScenePath($"0Scenes/{sessionInfo.Name}"),null);
    }
}
