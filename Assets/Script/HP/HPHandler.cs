using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;

public class HPHandler : NetworkBehaviour
{
    public  event Action<HPHandler> Death;

    int MaxHp { get; set; }
    [Networked(OnChanged = nameof(OnHPChanged))]
    public int HP { get; private set; }
    [Networked]
    public int AddForce { get;private set; }
    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    bool isInitialized = false;
    public Color uiOnHitColor;
    public Image uiONHitImage;
    public GameObject playerModel;
    public GameObject deathGameObjectPrefab;
    CharacterMovementHandler characterMovementHandler;
    PlayerStateHandler playerStateHandler;
    LocalUICanvas localUICanvas;
    [Header("KillLog UI")]
    public GameObject _killLogPanel; // ų�α� �г�
    public GameObject _killLogPrefab;// ų�α� ������
    [Networked(OnChanged = nameof(ShowBoard))]
    public NetworkBool _showBoard { get; set; } = true;

    [Header("KDA Info UI")]
    public GameObject _scoreBoard;
    // KillLog
    public Sprite[] _weaponSprite;
    [Networked]
    public int _weaponSpriteNum { get; set; }
    [Networked(OnChanged = nameof(KillPlayer))]
    public int _kill { get; set; }
    [Networked(OnChanged = nameof(DeathPlayer))]
    public int _death { get; set; }
    [Networked]
    public NetworkString<_16> _enemyNickName { get; set; }
    public HPHandler enemyHPHandler;
    public string _nickName;

    [Header("Respawn")]
    public bool isRespawnRequsted = false;

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

    //UI 관련은 매니저 만들어서 델리게이트 실험하기 좋을듯 
    IEnumerator OnHitCo()
    {

        if (Object.HasInputAuthority)
        {
            uiONHitImage.color = uiOnHitColor;
            localUICanvas.ChangeHPBar(HP, MaxHp, transform);

        }
        yield return new WaitForSeconds(0.2f);

        if (Object.HasInputAuthority && !isDead)
        {
            uiONHitImage.color = new Color(0, 0, 0, 0);
        }
    }

    IEnumerator ServerReviveCO()
    {
        yield return new WaitForSeconds(2.0f);
        RequestRespawn();
    }

    public override void Spawned()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        localUICanvas = GetComponentInChildren<LocalUICanvas>(); ;
        playerStateHandler = GetComponent<PlayerStateHandler>();
        MaxHp = 500;

        _killLogPanel = GameObject.Find("KillLogPanelnel");
        HpReset();
        isDead = false;
        isInitialized = true;

    }
    #region Vind
    public void ActionVind(CharacterHandler characterHandler)
    {
        //Update
        characterHandler.CharacterUpdate += CharacterUpdate;

        //Respawn
        characterHandler.Respawn += Respawn;
    }
    void CharacterUpdate(CharacterHandler _characterHandler)
    {
        CheckFallRespawn();
    }
    void Respawn(CharacterHandler _characterHandler)
    {
        OnRespawned();
        isRespawnRequsted = false;
    }

    #endregion

    public void OnTakeDamage(string _hitPlayer, int weaponNum, int _addForce = 10, int _attackDamage = 1)
    {
        Debug.Log("OnTakeDamage");
        if (HasStateAuthority)
        {
            _weaponSpriteNum = weaponNum;
        }
        if (isDead && !Object.HasStateAuthority)
        {
            return;
        }
        _enemyNickName = _hitPlayer;
        AddForce += _addForce;
        HP -= _attackDamage;
        if (HP <= 0)
        {
            Debug.Log($"{transform.name} isDead");
            StartCoroutine(ServerReviveCO());
            isDead = true;
            return;
        }
        playerStateHandler.isHit = true;

        

    }

    public void KillSelf()
    {
        HP = 0;
        Debug.Log($"{transform.name} isDead");
        StartCoroutine(ServerReviveCO());
        isDead = true;
    }



    public void HPUIUpdate()
    {
        localUICanvas.ChangeHPBar(HP, MaxHp, transform);
    }

    void OnHPReduced()
    {
        if (!isInitialized)
        {
            return;
        }
        StartCoroutine(OnHitCo());
    }
    static void OnHPChanged(Changed<HPHandler> changed)
    {

        int newHP = changed.Behaviour.HP;

        changed.LoadOld();

        int oldHP = changed.Behaviour.HP;
        changed.LoadNew();

        if (newHP != oldHP)
        {
            changed.Behaviour.HPUIUpdate();
            changed.Behaviour.OnHPReduced();
        }
    }
    static void OnStateChanged(Changed<HPHandler> changed)
    {
        //Debug.Log($"OnHPReduced()");

        bool isDeathCurrent = changed.Behaviour.isDead;

        changed.LoadOld();

        bool isDeadOld = changed.Behaviour.isDead;

        //Handle on death for the player. Also check if the player was dead but is now alive in that case reive the player.
        if (isDeathCurrent)
        {
            changed.Behaviour.OnDeath();
        }
        else if (!isDeathCurrent && isDeadOld)
        {
            changed.Behaviour.OnReive();
            //�̷��ϱ� �ѹ��̳� ?
            //changed.Behaviour.HpReset();
        }
    }
    void OnDeath()
    {
        CharacterHandler characterHandler = GetComponent<CharacterHandler>();
        
        if (enemyHPHandler != null)
        {
            ++enemyHPHandler._kill;
        }

        KillLogUpdate();

        if (playerModel == null)
        {
            Debug.Log("playerModel is Null");
            return;
        }
        Death(this);
        playerModel.gameObject.SetActive(false);

        Instantiate(deathGameObjectPrefab, transform.position, Quaternion.identity);

    }

    void OnReive()
    {
        if (playerModel == null)
        {
            Debug.Log("playerModel is Null");
            return;
        }
        if (Object.HasInputAuthority)
            uiONHitImage.color = new Color(0, 0, 0, 0);

        playerModel.gameObject.SetActive(true);
        


    }

    public void OnRespawned()
    {
        if (HasStateAuthority)
        {
            ++_death;
            _enemyNickName = "";
        }
        isDead = false;
        HpReset();
        AddForce =1;

    }

    void HpReset()
    {
        //Debug.Log("ResetHP");
        HP = 0;
        HP = MaxHp;
        HPUIUpdate();

    }
    public int ReturnMaxHP()
    {
        return MaxHp;
    }

    public void KillLogUpdate()
    {
        var Q = Instantiate(_killLogPrefab);
        Q.transform.parent = _killLogPanel.transform;

        //Ǯ�Ƿ� ������
        if (_enemyNickName.ToString() == "")
        {
            Q.GetComponent<KillLog>().SetLog(_nickName, " ", _weaponSprite[((int)EWeaponType.Gravity)]);
        }
        else
            Q.GetComponent<KillLog>().SetLog(_enemyNickName.ToString(), _nickName, _weaponSprite[_weaponSpriteNum]);


    }
    static void KillPlayer(Changed<HPHandler> changed)
    {
        changed.Behaviour.KDAUpdate();

    }
    static void DeathPlayer(Changed<HPHandler> changed)
    {
        changed.Behaviour.KDAUpdate();
    }

    public void KDAUpdate()
    {
        if (HasInputAuthority)
        {
            UIManager.Instance.PlayerKDAScoreUI(_kill, _death);
        }
    }

    static void ShowBoard(Changed<HPHandler> changed)
    {
        changed.Behaviour.QQQQ();
    }
    public void QQQQ()
    {
        if (HasInputAuthority)
        {
            _scoreBoard.SetActive(_showBoard);
        }
    }
}




