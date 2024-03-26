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
    public PlayerInfo playerInfo;
    public PlayerInfo enemyInfo;

    [Networked]
    public int AddForce { get; private set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    CharacterMovementHandler characterMovementHandler;
    PlayerStateHandler playerStateHandler;

    public TMP_Text nickNameText;

    [Header("Respawn")]
    public bool isRespawnRequsted = false;

    float lastHitTime = 0f;
    float damageDelay = 0.4f;

    public void CheckFallRespawn()
    {
        if (transform.position.y < -12)
        {
            if (Object.HasStateAuthority)
            {
                KillSelf();
            }
        }
    }
    public void RequestRespawn() => isRespawnRequsted = true;

   
    IEnumerator ServerReviveCO()
    {
        yield return new WaitForSeconds(2.0f);
        RequestRespawn();
    }

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
    }
    
    #region Event
    public void SubscribeToPlayerActionEvents(PlayerActionEvents _playerActionEvents)
    {
        //Update
        _playerActionEvents.OnPlayerUpdate += OnPlayerUpdate;

        //Respawn
        _playerActionEvents.OnPlyaerRespawn += OnPlyaerRespawn;

        //Respawn
        _playerActionEvents.OnPlyaerDeath += OnPlyaerDeath;
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
        OnRespawned();
        isRespawnRequsted = false;
    }
    #endregion
    #region Respawn
    public void OnRespawned()
    {
        if (HasStateAuthority)
        {
            playerInfo.OnRespawned();
        }
        isDead = false;
        AddForce = 1;
    }
    public void KillSelf()
    {
        Debug.Log($"{transform.name} isDead");
        StartCoroutine(ServerReviveCO());
        //if (HasStateAuthority)
        //{
        //    playerInfo.enemyinfo.kill += 1;
        //}
        isDead = true;
    }
    void OnDeath()
    {
        PlayerActionEvents eventHandler = GetComponent<PlayerActionEvents>();
        eventHandler.TriggerDeath();
    }
    #endregion
    public void OnTakeDamage(string enemyname, int weaponNum, Vector3 _attackDir, EAttackType eAttackType = EAttackType.Knockback, int _addForce = 2, int _attackDamage = 1)
    {
        if (!HasStateAuthority && HasInputAuthority)
        {
            return;
        }

        if (lastHitTime + damageDelay > Time.time && playerInfo.GetEnemyName() == enemyname)
        {
            Debug.Log("버그로 타격판정");
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
        //playerInfo.enemyinfo =_enemyInfo;
        AddForce += _addForce;
        //playerInfo.SetEnemyName(_enemyInfo.GetName());
        playerInfo.enemyName = enemyname;
        playerStateHandler.isHit = true;

        if (eAttackType == EAttackType.Knockback)
            characterMovementHandler.HitAddForce(_attackDir, AddForce);

    }
    static void OnStateChanged(Changed<HPHandler> changed)
    {
        bool isDeathCurrent = changed.Behaviour.isDead;
        changed.LoadOld();
        bool isDeadOld = changed.Behaviour.isDead;
        if (isDeathCurrent)
        {
            changed.Behaviour.OnDeath();
        }
    }

    #region Non
    //public void KillLogUpdate()
    //{
    //    var Q = Instantiate(_killLogPrefab);
    //    Q.transform.parent = _killLogPanel.transform;

    //    if (playerInfo.GetEnemyName() == "")
    //    {
    //        Q.GetComponent<KillLog>().SetLog(playerInfo.GetName(), " ", _weaponSprite[((int)EWeaponType.Gravity)]);
    //    }
    //    else
    //        Q.GetComponent<KillLog>().SetLog(playerInfo.GetEnemyName(), playerInfo.GetName(), _weaponSprite[_weaponSpriteNum]);
    //}
    //static void KillPlayer(Changed<HPHandler> changed)
    //{
    //    changed.Behaviour.KDAUpdate();

    //}
    //static void DeathPlayer(Changed<HPHandler> changed)
    //{
    //    changed.Behaviour.KDAUpdate();
    //}

    //public void KDAUpdate()
    //{
    //    if (HasInputAuthority)
    //    {
    //        UIManager.Instance.PlayerKDAScoreUI(playerInfo.kill, playerInfo.death);
    //    }
    //}

    //static void ShowBoard(Changed<HPHandler> changed)
    //{
    //    changed.Behaviour.QQQQ();
    //}
    //public void QQQQ()
    //{
    //    if (HasInputAuthority)
    //    {
    //        _scoreBoard.SetActive(_showBoard);
    //    }
    //}
    #endregion
}




