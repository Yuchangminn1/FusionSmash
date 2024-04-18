using Fusion;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class PlayerInfo : NetworkBehaviour, IPlayerActionListener
{
    public bool isSpawned { get; private set; } = false;

    HPHandler hPHandler;

    PlayerActionEvents playerActionEvents;

    public GameObject playerInfoPrefab;

    public PlayerInfoUI playerInfoUI;
    //public bool InitRe { get; set; } = false;
    [Networked(OnChanged = nameof(NickNameChanged))]
    public NetworkString<_16> playerName { get; set; }
    [Networked(OnChanged = nameof(EnemyNameChanged))]
    public NetworkString<_16> enemyName { get; set; }
    //public string Name { get; set; }
    [Networked(OnChanged = nameof(KillChanged))]
    public int kill { get; set; } = 0;
    [Networked(OnChanged = nameof(PlayingStateChanged))]
    [SerializeField]
    private int playingState { get; set; }

    public int PlayingState { get { return playingState; } set { playingState = value; } }
    [Networked(OnChanged = nameof(ForceChanged))]
    public int force { get; private set; } = 1;
    [Networked]
    public int life { get; private set; } = 0;
    [Networked]
    [SerializeField]
    private int playerNumber { get;  set; }
    public int PlayerNumber { get { return playerNumber; } set {  playerNumber = value; } }
    int defaultForce = 1;
    int defaultLife = 3;
    void ResetLife()
    {
        life = defaultLife;
    }
    void ResetForce()
    {
        force = defaultForce;
        Debug.Log("ResetForce()");

    }
    public void AddForce(int _force)
    {
        force += _force;
    }
    void ReduceLife()
    {
        if (HasStateAuthority)
        {
            if (playingState == (int)EPlayingState.Playing)
            {
                life -= 1;
                if (life <= 0)
                {
                    playingState = (int)EPlayingState.Death;
                }

            }
        }


    }

    public void TriggerGameOver()
    {
        if (playerActionEvents != null)
        {
            playerActionEvents.TriggerGameOver();
        }
        //Debug.Log("TriggerGameOver");
    }


    public void TriggerInit()
    {
        if (playerActionEvents != null)
        {
            playerActionEvents.TriggerInit();
        }

    }

    public void TriggerGameStart()
    {
        if (playerActionEvents != null)
        {

            playerActionEvents.TriggerGameStart();
            TriggerInit();
        }
    }

    public void TriggerGameEnd()
    {
        Debug.Log("PlayerInfo TriggerEnd");

        if (playerActionEvents != null)
        {
            playerActionEvents.TriggerGameEnd();
            TriggerInit();
            playerActionEvents.TriggerRespawn();
        }
        else
        {
            Debug.Log("playerActionEvents Is Null");
        }
    }


    public override void Spawned()
    {
        PlayerInfo[] players = FindObjectsOfType<PlayerInfo>();
        playerNumber = players.Length;
        hPHandler = GetComponent<HPHandler>();
        //GameManager.Instance.SetPlayer(this);
        isSpawned = true;

        if (GameManager.Instance.roomState == (int)ERoomState.Playing)
        {
            playingState = (int)EPlayingState.Death;
        }
        else
        {
            playingState = (int)EPlayingState.Waiting;
        }
        
        GameManager.Instance.SetPlayer(this);
    }

    static void NickNameChanged(Changed<PlayerInfo> changed)
    {
        changed.Behaviour.SetName(changed.Behaviour.playerName.ToString());
    }
    static void EnemyNameChanged(Changed<PlayerInfo> changed)
    {
        changed.Behaviour.SetEnemyName(changed.Behaviour.enemyName.ToString());
    }
    static void PlayingStateChanged(Changed<PlayerInfo> changed)
    {
        int newS = changed.Behaviour.playingState;
        changed.LoadOld();
        int oldS = changed.Behaviour.playingState;
        if (newS != oldS)
        {
            changed.Behaviour.StateChanged(newS);
        }
    }
    static void ForceChanged(Changed<PlayerInfo> changed)
    {
        int newS = changed.Behaviour.force;
        changed.LoadOld();
        int oldS = changed.Behaviour.force;
        if (newS != oldS)
        {
            changed.Behaviour.ForceChange(newS);
        }
    }
    void ForceChange(int _force)
    {
        if(playingState == (int)EPlayingState.Playing)
            UIManager.Instance.GetUIObject(playerNumber).SetPlayerForce(_force);
    }
    void StateChanged(int _playingState)
    {
        if (_playingState == (int)EPlayingState.Death)
        {
            if (HasInputAuthority)
            {
                GameManager.Instance.FadeIn_Out(2f);
                Debug.Log("FadeInOut");
            }

            TriggerGameOver();
        }

        else if (_playingState == (int)EPlayingState.Waiting)
        {
            hPHandler.SetTraceCamera(true);

            Debug.Log("SetTraceCamera");
        }

        else if(_playingState == (int)EPlayingState.Playing)
        {
            ResetForce();
            UIManager.Instance.SetUIObject(playerNumber, true);
            UIManager.Instance.GetUIObject(playerNumber).InitialPlayerInfo(playerName.ToString(), force);
        }
    }
    static void KillChanged(Changed<PlayerInfo> changed)
    {
        int newS = changed.Behaviour.kill;
        changed.LoadOld();
        int oldS = changed.Behaviour.kill;
        if (newS != oldS)
        {
            changed.Behaviour.SetKill(newS);
        }
    }
    public void SetName(string _nickName)
    {

        if (playerName != _nickName)
        {
            playerName = _nickName;
            HPHandler hp = GetComponent<HPHandler>();
            hp.nickNameText.text = _nickName;


        }
        if (playerInfoUI != null)
            playerInfoUI.SetPlayerName(_nickName);
        if (playerActionEvents != null)
            playerActionEvents.TriggerPlayerNameChange(playerName.ToString());
    }

    public void SetEnemyName(string _enemyName)
    {
        enemyName = _enemyName;
    }
    public string GetName()
    {
        return playerName.ToString();
    }
    public string GetEnemyName()
    {
        return enemyName.ToString();
    }
    public void SetKill(int _kill)
    {
        kill = _kill;
    }


    public void OnRespawned()
    {
        SetEnemyName("");

        if (playingState == (int)EPlayingState.Stop || playingState == (int)EPlayingState.Waiting)
            return;
    }

    public void CreatePlayerInfoUI()
    {
        if (playerInfoUI == null)
        {
            playerInfoUI = Instantiate(playerInfoPrefab).GetComponent<PlayerInfoUI>();
            UIManager.Instance.GetUIObject(playerNumber).InitialPlayerInfo(playerName.ToString(), force);

        }
        playerInfoUI.InitialPlayerInfo(playerName.ToString(), force);
    }
    void Start()
    {
        //CreatePlayerInfoUI();
    }

    public void SubscribeToPlayerActionEvents(ref PlayerActionEvents _playerActionEvents)
    {

        if (_playerActionEvents == null)
        {
            Debug.LogError("PlayerActionEvents component is missing!");
            return;
        }

        //OnTakeDamage
        _playerActionEvents.OnTakeDamage += OnTakeDamage;
        _playerActionEvents.OnPlyaerInit += OnPlyaerInit;
        _playerActionEvents.OnPlyaerDeath += OnPlyaerDeath;
        //_playerActionEvents.OnPlayerUpdate += OnPlayerUpdate;
        _playerActionEvents.OnPlyaerFixedUpdate += OnPlyaerFixedUpdate;
        _playerActionEvents.OnGameStart += OnGameStart;
        _playerActionEvents.OnPlyaerRespawn += OnPlyaerRespawn;
        _playerActionEvents.OnGameOver += OnGameOver;
        playerActionEvents = _playerActionEvents;
        

    }
    void OnTakeDamage(int _force, bool _isSmash)
    {
        AddForce(_force);
        Debug.Log("OnTakeDamage");
    }
    public void OnPlyaerInit()
    {
        ResetForce();
        ResetLife();
    }
    public void OnPlyaerDeath()
    {
        ReduceLife();
    }
    public void OnPlyaerFixedUpdate()
    {

    }

    public void OnGameStart()
    {
        //GameManager.Instance.StartGame();
        //playerInfoUI.SetUIObject(playerNumber, true);
    }
    public void OnPlyaerRespawn()
    {
        ResetForce();

    }
    void OnGameOver()
    {

    }
}
