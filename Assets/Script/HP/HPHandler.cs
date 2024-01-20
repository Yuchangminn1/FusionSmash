using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;

public class HPHandler : NetworkBehaviour
{

    int MaxHp { get; set; }
    [Networked(OnChanged = nameof(OnHPChanged))]
    public int HP { get; private set; }
    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    bool isInitialized = false;
    public Color uiOnHitColor;
    public Image uiONHitImage;
    public GameObject playerModel;
    public GameObject deathGameObjectPrefab;
    //Other components
    HitboxRoot hitboxRoot;
    CharacterMovementHandler characterMovementHandler;
    LocalUICanvas localUICanvas;
    [Header("KillLog UI")]
    public GameObject _killLogPanel; // 킬로그 패널
    public GameObject _killLogPrefab;// 킬로그 프리팹
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
    //public Slider hpBar;

    //[SerializeField] Image hpBarImage;
    //[SerializeField] Image hpHealFillImage;
    //[SerializeField] ParticleSystem playerParticle;

    void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        hitboxRoot = GetComponentInChildren<HitboxRoot>();
        localUICanvas = GetComponentInChildren<LocalUICanvas>(); ;
        
        MaxHp = 5;
    }
    IEnumerator OnHitCo()
    {
        //bodyMeshRender.material.color = Color.white;

        if (Object.HasInputAuthority)
        {
            uiONHitImage.color = uiOnHitColor;
            Debug.Log($"OnHitCo 의 HP = {HP}");
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
        characterMovementHandler.RequestRespawn();
    }


    void Start()
    {
        //hpBar = transform.GetComponentInChildren<Slider>();
        HpReset();

        isDead = false;


        isInitialized = true;
    }

    

    void Update()
    {

    }

    public override void Spawned()
    {
        _killLogPanel = GameObject.Find("KillLogPanelnel");
    }

    

    public void OnTakeDamage(string _hitPlayer, int weaponNum, int _attackDamage = 1)
    {
        if (HasStateAuthority)
        {
            _weaponSpriteNum = weaponNum;
        }
        if (isDead && !Object.HasStateAuthority)
        {
            return;
        }
        _enemyNickName = _hitPlayer;
        HP -= _attackDamage;
        if (HP <= 0)
        {
            Debug.Log($"{transform.name} isDead");
            StartCoroutine(ServerReviveCO());
            isDead = true;
        }

    }

    public void KillSelf()
    {
        HP = 0;
        Debug.Log($"{transform.name} isDead");
        StartCoroutine(ServerReviveCO());
        isDead = true;
    }

    //변하면 호출인데 아직 잘 모름 


    public void HPUIUpdate()
    {
        localUICanvas.ChangeHPBar(HP, MaxHp, transform);
    }

    void OnHPReduced()
    {
        Debug.Log($"OnHPReduced 의 HP = {HP}");
        if (!isInitialized)
        {
            return;
        }
        StartCoroutine(OnHitCo());
    }
    static void OnHPChanged(Changed<HPHandler> changed)
    {
        Debug.Log($" OnHPChanged()");


        int newHP = changed.Behaviour.HP;

        changed.LoadOld();

        int oldHP = changed.Behaviour.HP;
        changed.LoadNew();

        //check if the HP has been decreased
        if (newHP != oldHP)
        {
            changed.Behaviour.HPUIUpdate();
            //changed.Behaviour.HPBarValue(); 이런 호스트 체력을 다른 플레이어들 UI로 공유해주네 
            //changed에서 변경된 지역함수? 는 이 함수로 호출 되는 모든 함수에서 그 값으로 작용 ???
            //여기서 쓰는 함수는 static 이기 떄문에 ? changed.Behaviour를 써야하네 공부 해봐야할듯
            changed.Behaviour.OnHPReduced();
            //changed.Behaviour.HP = newHP;
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
            //이러니까 한방이네 ?
            //changed.Behaviour.HpReset();
        }
    }
    void OnDeath()
    {
        if(enemyHPHandler != null)
        {
            ++enemyHPHandler._kill;
        }

        KillLogUpdate();

        if (playerModel == null)
        {
            Debug.Log("playerModel is Null");
            return;
        }
        playerModel.gameObject.SetActive(false);
        hitboxRoot.HitboxRootActive = false;
        characterMovementHandler.SetCharacterControllerEnabled(false);


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
        hitboxRoot.HitboxRootActive = true;
        characterMovementHandler.SetCharacterControllerEnabled(true);


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
    }

    void HpReset()
    {
        Debug.Log("ResetHP");
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

        //풀피로 떨어짐
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




