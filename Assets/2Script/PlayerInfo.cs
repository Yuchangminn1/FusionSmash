using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    PlayerActionEvents playerActionEvents;

    public GameObject playerInfoPrefab;

    public PlayerInfoUI playerInfoUI;

    [Networked(OnChanged = nameof(NickNameChanged))]
    public NetworkString<_16> playerName { get; set; }
    [Networked(OnChanged = nameof(EnemyNameChanged))]
    public NetworkString<_16> enemyName { get; set; }
    //public string Name { get; set; }
    [Networked(OnChanged = nameof(ChangeKill))]
    public int kill { get; set; } = 0;
    [Networked(OnChanged = nameof(ChangeDeath))]
    public int death { get; set; } = 0;
    [Networked(OnChanged = nameof(ChangeForce))]
    public int force { get; set; } = 1;
    [Networked]
    public int playerNumber { get; set; }
    //PlayerInfoUIManager playerInfoUIManager;

    [Networked]
    public int playingState { get; set; }
    //public PlayerInfo enemyinfo;
    public void SubscribeToPlayerActionEvents(PlayerActionEvents _playerActionEvents)
    {
        playerActionEvents = _playerActionEvents;

        if (_playerActionEvents == null)
        {
            Debug.LogError("PlayerActionEvents component is missing!");
            return;
        }
        else
        {
            playerActionEvents = _playerActionEvents;
        }
        //OnTakeDamage
        _playerActionEvents.OnTakeDamage += OnTakeDamage;

        playerActionEvents.OnGameStart += OnGameStart;
        playerActionEvents.OnPlyaerRespawn += OnPlyaerRespawn;


    }
    public void OnGameStart()
    {
        GameManager.Instance.StartGame();
    }
    public void OnPlyaerRespawn()
    {
        force = 1;
    }
    public void TriggerGameStart()
    {
        if(playerActionEvents != null)
        {
            playerActionEvents.TriggerGameStart();

        }
    }

    public void TriggerGameEnd()
    {
        if (playerActionEvents != null)
        {
            playerActionEvents.TriggerGameEnd();
        }
        
    }

    void OnTakeDamage(int _force,bool _isSmash)
    {
        force += _force;
        Debug.Log("OnTakeDamage");
    }
    public override void Spawned()
    {
        PlayerInfo[] players = FindObjectsOfType<PlayerInfo>();
        playerNumber = players.Length;

        //playerInfoUIManager = GameObject.Find("PlayersInfos").GetComponent<PlayerInfoUIManager>();
        //if (playerInfoUIManager != null)
        //{
        //    playerInfoUIManager.AddPlayerInfo(playerNumber, Name);
        //}
    }

    static void NickNameChanged(Changed<PlayerInfo> changed)
    {
        changed.Behaviour.SetName(changed.Behaviour.playerName.ToString());
    }
    static void EnemyNameChanged(Changed<PlayerInfo> changed)
    {
        changed.Behaviour.SetEnemyName(changed.Behaviour.enemyName.ToString());
    }

    static void ChangeKill(Changed<PlayerInfo> changed)
    {
        int newS = changed.Behaviour.kill;
        changed.LoadOld();
        int oldS = changed.Behaviour.kill;
        if (newS != oldS)
        {
            changed.Behaviour.SetKill(newS);
        }
    }
    static void ChangeDeath(Changed<PlayerInfo> changed)
    {
        int newS = changed.Behaviour.death;
        changed.LoadOld();
        int oldS = changed.Behaviour.death;
        if (newS != oldS)
        {
            changed.Behaviour.SetDeath(newS);
        }
    }

    static void ChangeForce(Changed<PlayerInfo> changed)
    {
        int newS = changed.Behaviour.force;
        changed.LoadOld();
        int oldS = changed.Behaviour.force;
        if (newS != oldS)
        {
            changed.Behaviour.SetForce(newS);
        }
    }
    public void Start()
    {
        CreatePlayerInfoUI();
    }
    public void SetName(string _nickName)
    {

        if(playerName != _nickName)
        {
            playerName = _nickName;
            HPHandler hp = GetComponent<HPHandler>();
            hp.nickNameText.text = _nickName;
            
            GameManager.Instance.SetPlayer(this);
        }
        if(playerInfoUI != null)
            playerInfoUI.SetPlayerName(_nickName);
        if(playerActionEvents != null)
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
    public void SetDeath(int _death)
    {
        death = _death;
    }
    public void SetForce(int _force)
    {
        force = _force;
        if(playerInfoUI != null)
        {
            playerInfoUI.SetPlayerForce(force * 10);
        }
    }
    public void OnRespawned()
    {
        SetEnemyName("");

        if (playingState == (int)PlayingState.Stop || playingState == (int)PlayingState.Waiting)
            return;
        ++death;
    }

    public void CreatePlayerInfoUI()
    {
        if (playerInfoUI == null)
        {
            playerInfoUI = Instantiate(playerInfoPrefab).GetComponent<PlayerInfoUI>();
        }
        playerInfoUI.InitialPlayerInfo(playerName.ToString(), force);
    }


}
