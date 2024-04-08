using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Video.VideoPlayer;
//using static UnityEditorInternal.VersionControl.ListControl;
enum StateType
{
    Move,
    Jump,
    Fall,
    Land,
    Attack,
    Hit,
    Dodge,
    Death,
    Heal,
}


public class PlayerStateHandler : NetworkBehaviour, IPlayerActionListener
{
    [Networked(OnChanged = nameof(ChangeState))]
    public int state { get; set; }
    [Networked(OnChanged = nameof(ChangeState2))] //
    public int state2 { get; set; }
    [Networked(OnChanged = nameof(ChangeWeapon))] //
    public int weapon { get; set; }
    [Networked(OnChanged = nameof(ChangeAnimationTrigger))]
    public NetworkBool AnimationTrigger { get; set; }
    [Networked(OnChanged = nameof(ChangeCanMove))]
    public NetworkBool canMove { get; set; } = true;
    [Networked(OnChanged = nameof(ChangeStopMove))]
    public NetworkBool stopMove { get; set; } = false;

    public bool isdead = false;
    public bool isStop = false;
    public bool isHeal = false;
    public bool isHit = false;

    public int fHpHeal = 40;
    public int healNum;
    public int healNumMax = 3;

    [Header("GroundCheck")]
    [SerializeField] protected float GroundCheckDis = 0.65f;
    public LayerMask groundLayer;
    float groundCheckRad = 0.2f;

    [SerializeField] Animator anima;
    //Jump
    [Networked(OnChanged = nameof(ChangeJumpCount))] //
    public int jumpCount { get; set; } = 0;

    public float jumpTime = 0f;
    //public float jumpCoolTime = 0.05f;

    public int maxJumpCount { get; private set; } = 2;
    //Attack
    [Networked(OnChanged = nameof(ChangeAttackCount))] //
    public int attackCount { get; set; } = 0;
    public int maxAttackCount { get; private set; } = 2;
    public float lastAttackTime { get; set; } = 0f;
    float attackComboTime = 0.3f;
    #region State
    protected StateMachine stateMachine;
    public PlayerState nextState;
    public MoveState moveState { get; private set; }
    public JumpState jumpState { get; private set; }
    public FallState fallState { get; private set; }
    public LandState landState { get; private set; }
    public AttackState attackState { get; private set; }
    public HitState hitState { get; private set; }
    public DodgeState dodgeState { get; private set; }
    public DeathState deathState { get; private set; }
    public HealState healState { get; private set; }

    //public NetworkCharacterControllerPrototypeCustom networkCC;
    CharacterMovementHandler characterMovementHandler;

    public bool isJumpButtonPressed = false;

    public bool isFireButtonPressed = false;

    public bool isDodgeButtonPressed = false;
    [Networked]
    public int moveDir { get; set; } = 0;

    public bool conAttack = false;
    [Networked]
    public int attackstack { get; set; } = 0;

    #endregion
    //Weapon
    [Header("Weapon ")]
    WeaponHandler weaponHandler;
    private Dictionary<string, int> animationHashes = new Dictionary<string, int>();

    void Start()
    {
        //animationHashes.Add("State", Animator.StringToHash("State"));
        //animationHashes.Add("State2", Animator.StringToHash("State2"));
        //animationHashes.Add("AttackCount", Animator.StringToHash("AttackCount"));
        //animationHashes.Add("AnimationTrigger", Animator.StringToHash("AnimationTrigger"));

    }
    public override void Spawned()
    {


        base.Spawned();
        
        animationHashes.Add("State", Animator.StringToHash("State"));
        animationHashes.Add("State2", Animator.StringToHash("State2"));
        animationHashes.Add("AttackCount", Animator.StringToHash("AttackCount"));
        animationHashes.Add("AnimationTrigger", Animator.StringToHash("AnimationTrigger"));
        AnimationTrigger = false;
        anima = GetComponent<Animator>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        weaponHandler = GetComponent<WeaponHandler>();
        State_Initialize();
        if (stateMachine == null)
        {
            Debug.LogError("stateMachine is null after initialization.");
        }

        nextState = moveState;
        ChangeState();
        weapon = weaponHandler.equipWeapon._weaponNum;
        //어택카운트 변경 추가 
    }

    public void SubscribeToPlayerActionEvents(PlayerActionEvents _playerActionEvents)
    {
        //Move
        _playerActionEvents.OnPlayerMove += OnPlayerMove;
        //Update
        _playerActionEvents.OnPlayerUpdate += OnPlayerUpdate;
        //Respawn
        _playerActionEvents.OnPlyaerRespawn += OnPlyaerRespawn;
        //Jump
        _playerActionEvents.OnPlayerJump += OnPlayerJump;
        //Jump
        _playerActionEvents.OnPlayerAttack += OnPlayerAttck;
        //Death
        _playerActionEvents.OnPlyaerDeath += OnPlyaerDeath;

        _playerActionEvents.OnPlyaerFixedUpdate += OnPlayerFixedUpdate;


    }

    public void OnPlyaerDeath()
    {
        canMove = false;
        AnimationTrigger = false;
    }
    public void OnPlyaerRespawn()
    {
        canMove = true;
        AnimationTrigger = false;
        stateMachine.ChangeState(nextState);
    }
    public void OnPlayerMove(float _dirVector2)
    {
        SetInputVec(_dirVector2);
    }
    public void OnPlayerJump()
    {
    }
    public void OnPlayerAttck()
    {
        //Debug.Log("attackstack = " + attackstack);
    }
    public void SetInputVec(float vector2)
    {
        moveDir = vector2 == 0 ? 0 : 1;
    }
    void OnPlayerUpdate()
    {
        StatechangeUpdate();
        ResetCondition();
    }
    void OnPlayerFixedUpdate()
    {
        stateMachine.FixedUpdate();
    }
    void OnPlayerDeath()
    {

    }
    public void StatechangeUpdate()
    {
        if (stateMachine == null)
        {
            Debug.Log("stateMachine is Null");
            return;
        }
        stateMachine.Update();
        stateMachine.LateUpdate();
    }
    public void ResetCondition()
    {
        if (IsGround() && !Isvisi())
        {
            //Reset Counter
            if (jumpTime + 0.1f < Time.time)
                ResetJumpCount();
            //_dodgeCount = 0;
        }
    }

    private void LateUpdate()
    {
        if (canMove)
            SetFloat("InputX", (float)moveDir);
        else
        {
            SetFloat("InputX", 0f);
        }
    }
    #region State

    public EntityState GetCurrentState()
    {
        if (stateMachine.GetState() == null)
            return null;

        return stateMachine.GetState();
    }


    private void State_Initialize()
    {
        stateMachine = new StateMachine();

        moveState = new MoveState(this, 0);
        jumpState = new JumpState(this, 1);
        fallState = new FallState(this, 2);
        landState = new LandState(this, 3);
        attackState = new AttackState(this, 4);
        hitState = new HitState(this, 5);
        dodgeState = new DodgeState(this, 6);
        deathState = new DeathState(this, 7);
        healState = new HealState(this, 8);
    }
    public void SetState(int num)
    {
        state = num;
        //animationID
        anima.SetInteger(animationHashes["State"], num);
        //SetInt(animationHashes["State"], num);
    }
    public void SetState2(int num)
    {
        state2 = num;
        anima.SetInteger(animationHashes["State2"], num);
        //SetInt("State2", num);
    }
    public void SetAttackCount(int num)
    {
        if (num <= maxAttackCount && num >= 0)
        {
            attackCount = num;
        }
        anima.SetInteger(animationHashes["AttackCount"], num);
    }
    public bool Isvisi()
    {
        return AnimationTrigger;
    }
    public void ChangeState()
    {
        if (HasStateAuthority)
            stateMachine.ChangeState(nextState);
    }
    #endregion
    #region NetworkProperty
    static void ChangeState(Changed<PlayerStateHandler> changed)
    {
        int newS = changed.Behaviour.state;
        changed.LoadOld();
        int oldS = changed.Behaviour.state;
        if (newS != oldS)
        {
            changed.Behaviour.SetState(newS);
        }
    }
    static void ChangeState2(Changed<PlayerStateHandler> changed)
    {
        int newS = changed.Behaviour.state2;
        changed.LoadOld();
        int oldS = changed.Behaviour.state2;
        if (newS != oldS)
        {
            changed.Behaviour.SetState2(newS);
        }
    }
    static void ChangeWeapon(Changed<PlayerStateHandler> changed)
    {
        int newS = changed.Behaviour.weapon;
        changed.LoadOld();
        int oldS = changed.Behaviour.weapon;
        if (newS != oldS)
        {
            changed.Behaviour.SetInt("Weapon", newS);
        }
    }

    static void ChangeAnimationTrigger(Changed<PlayerStateHandler> changed)
    {
        NetworkBool newS = changed.Behaviour.AnimationTrigger;
        changed.LoadOld();
        NetworkBool oldS = changed.Behaviour.AnimationTrigger;
        if (newS != oldS)
        {
            changed.Behaviour.SetBool("AnimationTrigger", newS);
        }
    }
    static void ChangeJumpCount(Changed<PlayerStateHandler> changed)
    {
        int newS = changed.Behaviour.jumpCount;
        changed.LoadOld();
        int oldS = changed.Behaviour.jumpCount;
        if (newS != oldS)
        {
            changed.Behaviour.SetState2(newS);
        }
    }
    static void ChangeAttackCount(Changed<PlayerStateHandler> changed)
    {
        int newS = changed.Behaviour.attackCount;
        changed.LoadOld();
        int oldS = changed.Behaviour.attackCount;
        if (newS != oldS)
        {
            //Debug.Log("AttackCount = " + newS);
            changed.Behaviour.SetAttackCount(newS);
        }
    }
    static void ChangeCanMove(Changed<PlayerStateHandler> changed)
    {
        NetworkBool newS = changed.Behaviour.canMove;
        changed.LoadOld();
        NetworkBool oldS = changed.Behaviour.canMove;
        //Debug.Log(newS + "입력");
        if (newS != oldS)
        {
            changed.Behaviour.SetCanMove(newS);
        }
    }
    static void ChangeStopMove(Changed<PlayerStateHandler> changed)
    {
        NetworkBool newS = changed.Behaviour.stopMove;
        changed.LoadOld();
        NetworkBool oldS = changed.Behaviour.stopMove;
        if (newS != oldS)
        {
            changed.Behaviour.SetStopMove(newS);
        }
    }
    #endregion
    #region Animator
    public void SetBool(string _parameters, bool _tf) => anima.SetBool(_parameters, _tf);
    public void SetInt(string _parameters, int _num) => anima.SetInteger(_parameters, _num);
    public void SetFloat(string _parameters, float value) => anima.SetFloat(_parameters, value);

    #endregion
    #region Conditions
    public bool IsGround()
    {
        Vector3 tmp = transform.position;

        if (Physics.Raycast(transform.position + Vector3.up * GroundCheckDis / 2f, Vector3.down, GroundCheckDis, groundLayer))
        {
            return true;
        }
        else
        {
            bool _l = Physics.Raycast(transform.position + Vector3.left * groundCheckRad, Vector3.down, GroundCheckDis, groundLayer);
            bool _r = Physics.Raycast(transform.position + Vector3.right * groundCheckRad, Vector3.down, GroundCheckDis, groundLayer);
            bool _f = Physics.Raycast(transform.position + Vector3.forward * groundCheckRad, Vector3.down, GroundCheckDis, groundLayer);
            bool _b = Physics.Raycast(transform.position + Vector3.back * groundCheckRad, Vector3.down, GroundCheckDis, groundLayer);
            if (_l && _r && _f && _b)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public void SetCanMove(bool _tf)
    {
        canMove = _tf;
    }
    public void SetStopMove(bool _tf)
    {
        stopMove = _tf;
    }
    #endregion

    #region Jump
    public bool JumpAble()
    {
        //Debug.Log($"jumpTime  = {jumpTime} / jumpCoolTime = {jumpCoolTime} / Time.deltaTime = {Time.time}");

        if (stateMachine.GetState() == null) return false;
        if (!stateMachine.GetState().isAbleJump)
        {
            return false;
        }
        if (jumpCount < maxJumpCount)
        {
            isJumpButtonPressed = true;
            jumpCount++;

            //Debug.Log("JumpCount = " + jumpCount);
            jumpTime = Time.time;

            return true;
        }
        isJumpButtonPressed = false;
        return false;
    }

    public void ResetJumpCount()
    {
        jumpCount = 0;
    }
    #endregion
    #region Attack

    public void AttackEnter()
    {
        SetStopMove(true);
        --attackstack;
        if (attackCount == maxAttackCount)
        {
            attackstack = 0;
        }
        if (weapon == 1)
        {
            weaponHandler.GetEquipWeapon().SetCollistion(true);
            if (attackCount == maxAttackCount)
            {
                weaponHandler.GetEquipWeapon().SetAttackType(EAttackType.Knockback);
            }
            else
            {
                weaponHandler.GetEquipWeapon().SetAttackType(EAttackType.Nomal);
            }
        }
        isFireButtonPressed = false;

        lastAttackTime = Time.time;
    }

    public void AttackExit()
    {

        if (weapon == 1)
        {
            weaponHandler.GetEquipWeapon().SetCollistion(false);
        }
        if (nextState.currentStateNum != (int)StateType.Attack)
        {
            SetStopMove(false);
        }
        lastAttackTime = Time.time;
        AddAttackCount();
        //GetEquipWeapon().SetCollistion(false);
        //이거 고민중
    }
    public void AddAttackCount()
    {
        Debug.Log("Add AttackCount");
        attackCount++;
    }

    public void AttackCountReset()
    {
        //Debug.Log("AttackCountReset");
        if(attackCount != 0)
        {
            //Debug.Log("AttackCountReset IsGround attackCount != 0");

            if (IsGround())
            {
               // Debug.Log("AttackCountReset IsGround");

                if (lastAttackTime + attackComboTime < Time.time)
                {
                  //  Debug.Log("Reset AttackCount");

                    attackCount = 0;
                }
                if (attackCount > maxAttackCount)
                {
                 //   Debug.Log("Reset AttackCount");

                    attackCount = 0;
                    return;
                }
            }
        }

        

    }
    public bool AbleFire()
    {


        if (GetCurrentState() == null)
        {
            Debug.Log("return 1");
            return false;
        }
        if (weaponHandler == null) //|| !weaponHandler.AbleFire()
        {
            Debug.Log("return 2");

            return false;
        }
        if (attackCount >= maxAttackCount)
        {
            Debug.Log("return 3");

            return false;
        }

        EntityState tmpQ = GetCurrentState();
        if (tmpQ.isAbleAttack)
        {

            //AttackCount + Max Attack ~~ 이거 웨폰 핸들러로 이동 해야할듯 
            if (tmpQ.isCancel)
            {
                if (weapon == 0)
                {
                    //AnimationTrigger = false;
                    attackCount = maxAttackCount;
                    isFireButtonPressed = true;
                    //++attackstack;
                    Debug.Log("attackstack = " + attackstack);
                    return true;
                }
                if (weapon == 1)
                {
                    attackCount = maxAttackCount;

                    //AnimationTrigger = false;
                    isFireButtonPressed = true;
                    //++attackstack;
                    Debug.Log("attackstack = " + attackstack);

                    return true;
                }
            }
            else 
            {
                

                if (weapon == 0)
                {

                    isFireButtonPressed = true;
                    ++attackstack;
                    Debug.Log("attackstack = " + attackstack);
                    Debug.Log("return 4");
                    return true;
                }
                if (weapon == 1)
                {

                    isFireButtonPressed = true;
                    ++attackstack;
                    Debug.Log("attackstack = " + attackstack);
                    Debug.Log("return 4");
                    return true;
                }
            }
            
        }
        Debug.Log("return 5");

        return false;
    }

    #endregion
    #region DrawGizmos
    private void OnDrawGizmos()
    {
        Vector3 tmp = transform.position;
        tmp.y += GroundCheckDis / 2f;
        Gizmos.DrawRay(tmp, Vector3.down * GroundCheckDis);


        Gizmos.DrawRay(tmp + Vector3.left * groundCheckRad, Vector3.down * GroundCheckDis);
        Gizmos.DrawRay(tmp + Vector3.right * groundCheckRad, Vector3.down * GroundCheckDis);
        Gizmos.DrawRay(tmp + Vector3.forward * groundCheckRad, Vector3.down * GroundCheckDis);
        Gizmos.DrawRay(tmp + Vector3.back * groundCheckRad, Vector3.down * GroundCheckDis);

    }
    #endregion

}
