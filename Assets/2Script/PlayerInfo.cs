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
    private int playerNumber { get; set; }
    public int PlayerNumber { get { return playerNumber; } set { playerNumber = value; } }
    int defaultForce = 0;
    int defaultLife = 3;

    [Networked(OnChanged = nameof(FadeInChanged))]
    public NetworkBool FadeIN { get; set; } = false;
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
            playerActionEvents.TriggerRespawn();
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
        if (HasInputAuthority)
        {
            GameManager.Instance.FadeOut(1f, 0.5f);
        }

        PlayerInfo[] players = FindObjectsOfType<PlayerInfo>();
        playerNumber = players.Length;
        hPHandler = GetComponent<HPHandler>();
        //GameManager.Instance.SetPlayer(this);
        isSpawned = true;

        if (GameManager.Instance.roomState == (int)ERoomState.Playing)
        {
            GameManager.Instance.playingPlayerNum += 1;
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
    static void FadeInChanged(Changed<PlayerInfo> changed)
    {
        bool newS = changed.Behaviour.FadeIN;
        changed.LoadOld();
        bool oldS = changed.Behaviour.FadeIN;
        if (newS)
        {
            changed.Behaviour.FadeIn();
        }
    }
    void FadeIn()
    {
        FadeIN = false;
        if (HasInputAuthority)
        {
            GameManager.Instance.FadeIn(0.2f);
            Debug.Log("Fade IN");
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
        if (playingState == (int)EPlayingState.Playing)
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
            SoundManager.Instance.StopSound((int)EAudio.audioSourceBGM);
            hPHandler.SetTraceCamera(true);
            Debug.Log("SetTraceCamera");
            UIManager.Instance.OnGameWait();

        }

        else if (_playingState == (int)EPlayingState.Playing)
        {
            if (HasInputAuthority)
            {
                GameManager.Instance.FadeOut(1f,0.5f);
                Debug.Log("FadeInOut");
                SoundManager.Instance.StopSound((int)EAudio.audioSourceBGM);
                SoundManager.Instance.PlaySound((int)EAudio.audioSourceBGM, (int)ESound.BGM2);
                SoundManager.Instance.PlaySound((int)EAudio.audioSourceGameSet, (int)ESound.GameStart);
                Debug.Log("Sound Start");
                
            }
            TriggerGameStart();
            UIManager.Instance.SetUIObject(playerNumber, true);
            UIManager.Instance.GetUIObject(playerNumber).InitialPlayerInfo(playerName.ToString(), force);
            UIManager.Instance.OnGameStart();

            //ResetForce();
        }
        else if (_playingState == (int)EPlayingState.Stop)
        {
            UIManager.Instance.winnerText.text = "승자  : " + GameManager.Instance.winnerName.ToString();
            SoundManager.Instance.PlaySound((int)EAudio.audioSourceCharacter, (int)ESound.Victoty1, 2f);

            if (HasInputAuthority)
            {
                SoundManager.Instance.StopSound((int)EAudio.audioSourceBGM);
                SoundManager.Instance.PlaySound((int)EAudio.audioSourceGameSet, (int)ESound.GameEnd);
                Debug.Log("Sound End");
                UIManager.Instance.OnGameEnd();

            }
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

    public void SubscribeToPlayerActionEvents(ref PlayerActionEvents _playerActionEvents)
    {
        playerActionEvents = _playerActionEvents;

        if (_playerActionEvents == null)
        {
            Debug.LogError("PlayerActionEvents component is missing!");
            return;
        }
        //OnTakeDamage
        _playerActionEvents.OnTakeDamage += OnTakeDamage;
        _playerActionEvents.OnPlyaerInit += OnPlyaerInit;
        _playerActionEvents.OnPlyaerDeath += OnPlyaerDeath;
        _playerActionEvents.OnPlyaerRespawn += OnPlyaerRespawn;
        _playerActionEvents.OnGameOver += OnGameOver;


    }
    void OnTakeDamage(int _force, bool _isSmash)
    {
        AddForce(_force);
        Debug.Log("OnTakeDamage");
    }
    public void OnPlyaerInit()
    {
        if(HasInputAuthority)
            Debug.Log("OnInit");
        if (HasStateAuthority)
        {
            ResetForce();
            ResetLife();
        }
    }
    public void OnPlyaerDeath()
    {
        ReduceLife();
    }
    
    
    public void OnPlyaerRespawn()
    {
        ResetForce();

    }
    void OnGameOver()
    {
        GameManager.Instance.playingPlayerNum -= 1;
        Debug.Log($"playingPlayerNum = {GameManager.Instance.playingPlayerNum}");
    }
    public void GameEndToWaiting(float duration)
    {
        Debug.Log("GameEndToWaiting");
        if(playingState == (int)EPlayingState.Playing )
        {
            GameManager.Instance.winnerName = playerName.ToString();
            playerActionEvents.TriggerVictory();
        }
        
        playingState = (int)EPlayingState.Stop;
        StartCoroutine(CGameEndToWaiting(duration));

    }
    private IEnumerator CGameEndToWaiting(float duration)
    {
        yield return new WaitForSeconds(duration);
        playingState = (int)EPlayingState.Waiting;
        TriggerGameEnd();

    }
}
