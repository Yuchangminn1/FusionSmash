using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;

public class HPHandler : NetworkBehaviour, IPlayerActionListener
{
    PlayerActionEvents playerActionEvents;

    public PlayerInfo playerInfo;
    public PlayerInfo enemyInfo;

    [Networked]
    public int addForce { get; private set; }
    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    CharacterMovementHandler characterMovementHandler;
    PlayerStateHandler playerStateHandler;

    public TMP_Text nickNameText;
    //[Networked]
    //public NetworkBool isRespawnRequsted { get; set; } = false;

    float lastHitTime = 0f;
    float damageDelay = 0.1f;

    const int defaultforce = 1;

    Camera playerTraceCamera;
    Camera observingCamera;

    float MYTIME = 0f;

    public override void Spawned()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        playerStateHandler = GetComponent<PlayerStateHandler>();
        playerInfo = GetComponent<PlayerInfo>();
        nickNameText = GetComponentInChildren<TMP_Text>();
        isDead = false;
        if (HasInputAuthority)
        {
            nickNameText.color = new Color(0, 0, 0, 0);
        }
        //Camera
        playerTraceCamera = GameObject.Find("TraceCamera").GetComponent<Camera>();
        observingCamera = GameObject.Find("ObservingCamera").GetComponent<Camera>();
    }
    public void SetTraceCamera(bool _tf)
    {
        playerTraceCamera.enabled = _tf;
        observingCamera.enabled = !_tf;
        Debug.Log("Trace Camera Set");
    }

    
    public void OnTakeDamage(string enemyname, int weaponNum, Vector3 _attackDir, EAttackType eAttackType = EAttackType.Knockback, int _addForce = 2, int _attackDamage = 1)
    {
        if (!HasStateAuthority)
        {
            return;
        }
        if (lastHitTime + damageDelay > Time.time && playerInfo.GetEnemyName() == enemyname)
        {
            lastHitTime = Time.time;
            return;
        }
        else
        {
            lastHitTime = Time.time;
        }
        if (isDead && !Object.HasStateAuthority)
        {
            return;
        }
        addForce += _addForce;
        playerInfo.enemyName = enemyname;
        if (eAttackType == EAttackType.Knockback)
        {
            playerActionEvents.TriggerPlayerOnTakeDamage(_addForce, true);
            characterMovementHandler.HitAddForce(_attackDir, addForce);
            playerStateHandler.isKnockBack = true;
            Debug.Log("KnockBack");
            playerStateHandler.isHit = false;
        }
        else
        {
            playerActionEvents.TriggerPlayerOnTakeDamage(_addForce, false);
            if (!playerStateHandler.isKnockBack)
                playerStateHandler.isHit = true;
        }
    }
    public void KillSelf()
    {
        if (!isDead)
        {
            Debug.Log($"{transform.name} isDead");
            StartCoroutine(ServerReviveCO());
            isDead = true;
            MYTIME = Time.time;
        }
    }
    IEnumerator ServerReviveCO()
    {
        yield return new WaitForSeconds(2.0f);
        if (playerInfo.PlayingState != (int)EPlayingState.Death && playerInfo.PlayingState != (int)EPlayingState.Stop)
        {
            playerActionEvents.TriggerRespawn();
        }
    }
    public void CheckFallRespawn()
    {
        if (transform.position.y < -12 && !isDead)
        {
            if (HasStateAuthority)
            {
                KillSelf();
            }
        }
    }
    static void OnStateChanged(Changed<HPHandler> changed)
    {
        bool isDeathCurrent = changed.Behaviour.isDead;
        changed.LoadOld();
        bool isDeadOld = changed.Behaviour.isDead;
        changed.Behaviour.ChangedIsDead(isDeathCurrent);

    }
    void ChangedIsDead(bool _tf)
    {
        if (_tf)
        {
            if (HasInputAuthority)
            {
                Debug.Log("HPHandler Fade In");

                GameManager.Instance.FadeIn(0.2f);
            }
            playerActionEvents.TriggerDeath();
            //이거 추후에 playerinfo로 
        }
        else
        {
            if (HasInputAuthority)
            {
                Debug.Log("HPHandler Fade Out");
                GameManager.Instance.FadeOut(1f, 0.5f);
            }
        }
    }

    #region Event
    public void SubscribeToPlayerActionEvents(ref PlayerActionEvents _playerActionEvents)
    {
        playerActionEvents = _playerActionEvents;

        //Update
        _playerActionEvents.OnPlayerUpdate += OnPlayerUpdate;

        //Respawn
        _playerActionEvents.OnPlyaerRespawn += OnPlyaerRespawn;

        //Respawn
        _playerActionEvents.OnPlyaerDeath += OnPlyaerDeath;

        _playerActionEvents.OnPlyaerInit += OnPlyaerInit;

        _playerActionEvents.OnGameOver += OnGameOver;


    }
    void OnPlyaerInit()
    {
        addForce = defaultforce;
        isDead = false;
    }
    void OnPlyaerDeath()
    {
        if (HasStateAuthority)
        {
            if (enemyInfo != null)
            {
                enemyInfo.kill += 1;
            }
        }
    }
    
    void OnPlayerUpdate()
    {
        CheckFallRespawn();
    }
    void OnPlyaerRespawn()
    {
        if (HasStateAuthority)
        {
            playerInfo.OnRespawned();
        }
        isDead = false;
        addForce = defaultforce;
        Debug.Log("HPHnadler OnPlyaerRespawn");
    }
    void OnGameOver()
    {
        if(HasInputAuthority)
            SetTraceCamera(false);
    }
    #endregion





}




