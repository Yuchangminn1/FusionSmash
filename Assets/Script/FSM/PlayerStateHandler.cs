using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateHandler : NetworkBehaviour
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
    float jumpTime = 0f;
    float jumpClearTime = 0.1f;

    public int maxJumpCount { get; private set; } = 2;
    //Attack
    [Networked(OnChanged = nameof(ChangeAttackCount))] //
    public int attackCount { get; set; } = 0;
    public int maxAttackCount { get; private set; } = 2;
    public float lastAttackTime { get; set; } = 0f;
    float attackComboTime = 1.5f;
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



    Vector3 inputVec3;

    #endregion
    //Weapon
    [Header("Weapon ")]
    WeaponHandler weaponHandler;
    public override void Spawned()
    {
        base.Spawned();
        AnimationTrigger = false;
        anima = GetComponent<Animator>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        weaponHandler = GetComponent<WeaponHandler>();

        //Weapon
        weaponHandler.SetEq();
        weapon = (int)EWeaponType.Sword;
        PlayerChangeWeapon((int)EWeaponType.Sword);

        State_Initialize();
        if (HasStateAuthority)
        {
            nextState = moveState;
            ChangeState();
        }
        ActionVind();
    }
    void ActionVind()
    {
        #region Event
        //Move
        CharacterHandler.Move += Move;
        //Update
        CharacterHandler.CharacterUpdate += CharacterUpdate;
        //Attack
        //Respawn
        CharacterHandler.Respawn += Respawn;
        #endregion
    }
    void CharacterUpdate(CharacterHandler _characterHandler)
    {
        StateChageUpdate();
        ResetCondition();
    }



    void Respawn(CharacterHandler _characterHandler)
    {
        //SetCharacterControllerEnabled(true);
        //networkRigidbody.TeleportToPosition(Utils.GetRandomSpawnPoint());
        GetWeaponHandler()._equipWeapon.OnRespawn();
    }
    void Update()
    {
        if (HasStateAuthority)
        {
            stateMachine.Update();
        }
    }
    void FixedUpdate()
    {
        if (HasStateAuthority)
        {
            stateMachine.FixedUpdate();
        }
    }
    private void LateUpdate()
    {
        SetFloat("InputX", inputVec3.x);
        SetFloat("InputZ", inputVec3.y);
    }
    #region State
    public void StateChageUpdate()
    {
        if (stateMachine == null)
        {
            if (HasStateAuthority)
            {
                Debug.Log("stateMachine is Null");
            }
            return;
        }
        if (HasStateAuthority)
        {
            stateMachine.Update();
            stateMachine.LateUpdate();
        }
    }
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
        if (HasStateAuthority)
        {
            state = num;
            SetInt("State", num);
        }
    }
    public void SetState2(int num)
    {
        if (HasStateAuthority)
        {
            state2 = num;
            SetInt("State2", num);
        }
    }
    public bool Isvisi()
    {
        return AnimationTrigger;
    }
    public void ChangeState()
    {
        if (HasStateAuthority)
        {
            stateMachine.ChangeState(nextState);
        }
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
            changed.Behaviour.SetInt("State", newS);
        }
    }
    static void ChangeState2(Changed<PlayerStateHandler> changed)
    {
        int newS = changed.Behaviour.state2;
        changed.LoadOld();
        int oldS = changed.Behaviour.state2;
        if (newS != oldS)
        {
            changed.Behaviour.SetInt("State2", newS);
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
            changed.Behaviour.SetInt("State2", newS);
        }
    }
    static void ChangeAttackCount(Changed<PlayerStateHandler> changed)
    {
        int newS = changed.Behaviour.attackCount;
        changed.LoadOld();
        int oldS = changed.Behaviour.attackCount;
        if (newS != oldS)
        {
            changed.Behaviour.SetInt("State2", newS);
        }
    }
    static void ChangeCanMove(Changed<PlayerStateHandler> changed)
    {
        NetworkBool newS = changed.Behaviour.canMove;
        changed.LoadOld();
        NetworkBool oldS = changed.Behaviour.canMove;
        Debug.Log(newS + "입력");
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
    
    public void PlayerChangeWeapon(int _weaponNum)
    {
        weaponHandler.ChangeWeapon(_weaponNum);
    }
    #endregion
    #region Move
    void Move(Vector2 _dirVector2)
    {
        SetInputVec(_dirVector2);
    }
    public Vector3 SetInputVec(Vector3 vector3)
    {
        inputVec3 = vector3;
        return inputVec3;
    }
    #endregion
    #region Jump
    public bool JumpAble()
    {
        if (stateMachine.GetState() == null) return false;
        if (!stateMachine.GetState().isAbleJump)
        {
            return false;
        }
        if (jumpCount < maxJumpCount)
        {
            isJumpButtonPressed = true;
            jumpCount++;
            jumpTime = Time.time;
            return true;
        }
        isJumpButtonPressed = false;
        return false;
    }
    public void ResetCondition()
    {
        if (IsGround() && !Isvisi())
        {
            //Reset Counter
            ResetJumpCount();
            //_dodgeCount = 0;
        }
    }
    public void ResetJumpCount()
    {
        if (HasStateAuthority)
        {
            if(jumpTime+jumpClearTime<Time.time)
                jumpCount = 0;
        }
    }
    #endregion
    #region Attack

    
    public void Fire()
    {
        characterMovementHandler.Fire();
    }
    public void AttackEnter()
    {
        //AddAttackCount();
        SetStopMove(true);
        lastAttackTime = Time.time;
        isFireButtonPressed = false;
        Fire();
    }
    public void AttackExit()
    {
        SetStopMove(false);
        lastAttackTime = Time.time;
        GetEquipWeapon().SetCollistion(false);
    }
    public void AddAttackCount()
    {
        if (attackCount < maxAttackCount)
        {
            attackCount++;
        }
    }
    public void SetAttackCount(int _attackCount)
    {
        if (_attackCount < 4 && _attackCount >= 0)
        {
            attackCount = _attackCount;
        }
    }
    public void AttackCountReset()
    {
        if (state == 4)
        {
            return;
        }
        if (attackCount >= maxAttackCount)
        {
            attackCount = 0;
            return;
        }
        if (lastAttackTime + attackComboTime < Time.time)
        {
            attackCount = 0;
        }
    }
    public bool AbleFire()
    {
        if (GetCurrentState() == null)
        {
            return false;
        }
        if (weaponHandler == null || !weaponHandler.AbleFire())
        {
            return false;
        }
        EntityState tmpQ = GetCurrentState();
        if (tmpQ.isAbleAttack)
        {
            if (!Isvisi() || tmpQ.isCancel)
            {
                if (weapon == 0)
                {
                    isFireButtonPressed = true;
                    AddAttackCount();
                    return true;
                }
                if (weapon == 1)
                {
                    if (attackCount < maxAttackCount)
                    {
                        isFireButtonPressed = true;
                        AddAttackCount();
                        return true;
                    }
                }
            }
        }
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

    //Weapon
    public PlayerWeapon GetEquipWeapon()
    {
        if (weaponHandler == null)
        {
            Debug.Log("EquipWeapon is Null");
        }
        return weaponHandler.GetEquipWeapon();
    }
    public void ChangeWeapon(int _weaponNum)
    {
        weaponHandler.ChangeWeapon(_weaponNum);
    }

    public WeaponHandler GetWeaponHandler() { return weaponHandler; }
    //임시 델리게이트로 묶어야할듯
}
