using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class HPHandler : NetworkBehaviour
{

    int MaxHp { get; set; }

    [Networked(OnChanged = nameof(OnHPChanged))]
    int HP { get; set; }

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
            uiONHitImage.color = new Color(0, 0, 0,0);
        }
        

    }

    IEnumerator ServerReviveCO()
    {
        yield return new WaitForSeconds(2.0f);
        characterMovementHandler.RequestRespawn();
    }


    void Start()
    {
        Debug.Log("내 이름은 " + transform.name);
        //hpBar = transform.GetComponentInChildren<Slider>();
        HpReset();

        isDead = false;


        isInitialized = true;
    }
    void Update()
    {
        
    }
    public void OnTakeDamage(int _attackDamage = 1)
    {
        if (isDead)
        {
            return;
        }
        Debug.Log($"OnTakeDamage{HP}전");

        HP -= 1;
        localUICanvas.ChangeHPBar(HP, MaxHp, transform);
        Debug.Log($"OnTakeDamage{HP}후");
        Debug.Log($"{Time.time} {transform.name}took damage get {HP} left");
        if (HP <= 0)
        {
            Debug.Log($"{transform.name} isDead");

            StartCoroutine(ServerReviveCO());

            isDead = true;

        }

    }
    //변하면 호출인데 아직 잘 모름 
    static void OnHPChanged(Changed<HPHandler> changed)
    {
        Debug.Log($" OnHPChanged()");


        int newHP = changed.Behaviour.HP;
        
        //load the old value
        changed.LoadOld();

        int oldHP = changed.Behaviour.HP;
        changed.LoadNew();

        //check if the HP has been decreased
        if (newHP != oldHP)
        {
            //changed.Behaviour.HPBarValue(); 이런 호스트 체력을 다른 플레이어들 UI로 공유해주네 
            //changed에서 변경된 지역함수? 는 이 함수로 호출 되는 모든 함수에서 그 값으로 작용 ???
            //여기서 쓰는 함수는 static 이기 떄문에 ? changed.Behaviour를 써야하네 공부 해봐야할듯
            changed.Behaviour.OnHPReduced();

        }
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

    static void OnStateChanged(Changed<HPHandler> changed)
    {
        //Debug.Log($"OnHPReduced()");

        bool isDeathCurrent = changed.Behaviour.isDead;

        changed.LoadOld();

        bool isDeadOld = changed.Behaviour.isDead;

        //Handle on death for the player. Also check if the player was dead but is now alive in that case reive the player.
        if (isDeathCurrent)
            changed.Behaviour.OnDeath();
        else if (!isDeathCurrent && isDeadOld)
        {
            changed.Behaviour.OnReive();
            //이러니까 한방이네 ?
            //changed.Behaviour.HpReset();
        }


    }
    void OnDeath()
    {

        playerModel.gameObject.SetActive(false);
        hitboxRoot.HitboxRootActive = false;
        characterMovementHandler.SetCharacterControllerEnabled(false);
        

        Instantiate(deathGameObjectPrefab,transform.position,Quaternion.identity);

    }
    void OnReive()
    {

        if (Object.HasInputAuthority)
            uiONHitImage.color = new Color(0,0,0,0);

        playerModel.gameObject.SetActive(true);
        hitboxRoot.HitboxRootActive = true;
        characterMovementHandler.SetCharacterControllerEnabled(true);
        

    }

    public void OnRespawned()
    {
        isDead = false;
        HpReset();
    }

    void HpReset()
    {
        HP = 0;
        HP = MaxHp;
        localUICanvas.ChangeHPBar(HP, MaxHp, transform);

    }
    public int ReturnMaxHP()
    {
        return MaxHp;
    }
}



    
