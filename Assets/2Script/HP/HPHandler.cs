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

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    public TMP_Text nickNameText;
    //[Networked]
    //public NetworkBool isRespawnRequsted { get; set; } = false;

    float lastHitTime = 0f;
    float damageDelay = 0.1f;

    const int defaultforce = 1;

    

    float MYTIME = 0f;

    public override void Spawned()
    {
        playerInfo = GetComponent<PlayerInfo>();
        nickNameText = GetComponentInChildren<TMP_Text>();
        isDead = false;
        if (HasInputAuthority)
        {
            nickNameText.color = new Color(0, 0, 0, 0);
        }
        
    }
    

    
    public void OnTakeDamage(string enemyname, int weaponNum, Vector3 _attackDir, EAttackType eAttackType = EAttackType.Knockback, int _addForce = 2, int _attackDamage = 1)
    {
        if (!playerInfo.isDamageAble)
        {
            return;
        }

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
        playerInfo.enemyName = enemyname;
        playerActionEvents.TriggerPlayerOnTakeDamage(_addForce);

        if (eAttackType == EAttackType.Knockback)
        {
            playerActionEvents.TriggerPlayerKnockBack(_attackDir,playerInfo.force);
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

    }
    void OnPlyaerInit()
    {
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
        Debug.Log("HPHnadler OnPlyaerRespawn");
    }
    
    #endregion





}




