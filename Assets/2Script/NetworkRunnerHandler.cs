using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;
//using UnityEditor.Build.Content;


public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;

    NetworkRunner networkRunner;
    private void Awake()
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

            if(SceneManager.GetActiveScene().name != "MainMenu")
            {
                var clienTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient,"TestSession", NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
            }
            Debug.Log("sever networkRunner started.");

        }
    }
    //public void StarthostMigration(HostMigrationToken hostMigrationToken)
    //{
    //    networkRunner = Instantiate(networkRunnerPrefab);
    //    networkRunner.name = "Network runner - Migrated";

    //    var clientTask = InitializeNetworkRunnerHostMigration(networkRunner, hostMigrationToken);

    //    Debug.Log($"Host migration started");
    //}

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

    //protected virtual Task InitializeNetworkRunnerHostMigration(NetworkRunner runner,HostMigrationToken hostMigrationToken)
    //{
    //    var sceneManager = GetSceneManager(runner);

    //    runner.ProvideInput = true;

    //    return runner.StartGame(new StartGameArgs
    //    {
    //        SceneManager = sceneManager,
    //        HostMigrationToken = hostMigrationToken,
    //        HostMigrationResume = HostMigrationResume,
    //        ConnectionToken = GameManager.instance.GetConnectionToken(),

    //    }) ;

    //}

    //void HostMigrationResume(NetworkRunner runner)
    //{
    //    Debug.Log($"HostMigrationResume started");

    //    foreach(var resumeNetworkObject in runner.GetResumeSnapshotNetworkObjects())
    //    {
    //        if(resumeNetworkObject.TryGetBehaviour<CharacterMovementHandler>(out var networkrigidbody))
    //        {
    //            runner.Spawn(resumeNetworkObject,position: networkrigidbody.transform.position,rotation:networkrigidbody.transform.rotation,onBeforeSpawned: (runner, newNetworkobject) =>
    //            {
    //                newNetworkobject.CopyStateFrom(resumeNetworkObject);

    //                if(resumeNetworkObject.TryGetBehaviour<HPHandler>(out HPHandler oldHPHandler))
    //                {
    //                    HPHandler newHPHandler = newNetworkobject.GetComponent<HPHandler>();
    //                    newHPHandler.CopyStateFrom(oldHPHandler);
                        
    //                }
    //            } 
    //        }

    //    }
    //}

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
