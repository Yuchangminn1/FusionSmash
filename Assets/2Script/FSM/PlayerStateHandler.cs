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
enum EStateType
{
    Move,
    Jump,
    Fall,
    Land,
    Attack,
    Hit,
    KnockBack,
    Dodge,
    Death,
    Heal,
    Victory,
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
    [Networked(OnChanged = nameof(ChangeKnockBack))]
    public NetworkBool isKnockBack { get; set; } = false;

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

    public int maxJumpCount { get; private set; } = 2;
    //Attack
    [Networked(OnChanged = nameof(ChangeAttackCount))] //
    public int attackCount { get; set; } = 0;
    public int maxAttackCount { get; private set; } = 3;
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
    public KnockBackState knockBackState { get; private set; }

    public DeathState deathState { get; private set; }
    public HealState healState { get; private set; }

    CharacterMovementHandler characterMovementHandler;

    public bool isJumpButtonPressed = false;

    public bool isFireButtonPressed = false;

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
    PlayerEffectHandler playerEffectHandler;

    public override void Spawned()
    {
        playerEffectHandler = GetComponent<PlayerEffectHandler>();

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
            if (jumpTime + 0.1f < Time.time)
                ResetJumpCount();
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
        knockBackState = new KnockBackState(this, 6);
        //dodgeState = new DodgeState(this, 6);
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

        anima.SetInteger(animationHashes["AttackCount"], num);
        Debug.Log($"Attack Count = {num}");
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
    void JumpCountSound(int _jumpCount)
    {
        if (_jumpCount == 1)
        {
            SoundManager.Instance.PlaySound((int)EAudio.audioSourceCharacter, (int)ESound.Jump1);
        }
        else if (_jumpCount == 2)
        {
            SoundManager.Instance.PlaySound((int)EAudio.audioSourceCharacter, (int)ESound.Jump2);
        }
    }

    void AttackCountSound(int _attackCount)
    {
        if (_attackCount > 0)
        {
            if (_attackCount == maxAttackCount)
            {
                SoundManager.Instance.PlaySound((int)EAudio.audioSourceCharacter, (int)ESound.SmashAttack);
            }
            else
            {
                SoundManager.Instance.PlaySound((int)EAudio.audioSourceCharacter, (int)ESound.NomalAttack);
            }
        }
    }

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
            changed.Behaviour.JumpCountSound(newS);

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
            changed.Behaviour.AttackCountSound(newS);

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

    static void ChangeKnockBack(Changed<PlayerStateHandler> changed)
    {
        NetworkBool newS = changed.Behaviour.isKnockBack;
        changed.LoadOld();
        NetworkBool oldS = changed.Behaviour.isKnockBack;
        if (newS != oldS)
        {
            changed.Behaviour.KnockBackParticle(newS);
        }
    }
    public void KnockBackParticle(bool _tf)
    {
        if (_tf)
        {
            PlayKnockBackParticle();
        }
        else
        {
            StopKnockBackParticle();
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
        if (stateMachine.GetState() == null) return false;
        if (!stateMachine.GetState().isAbleJump)
        {
            return false;
        }
        if (jumpCount < maxJumpCount)
        {
            isJumpButtonPressed = true;

            ++jumpCount;

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
        AddAttackCount();
        if (attackCount == maxAttackCount)
        {
            attackstack = 0;
        }
        if (weapon == 1)
        {

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
            //weaponHandler.GetEquipWeapon().SetCollistion(false);
        }
        if (nextState.currentStateNum != (int)EStateType.Attack)
        {
            SetStopMove(false);
        }
        lastAttackTime = Time.time;

        //GetEquipWeapon().SetCollistion(false);
        //이거 고민중
    }
    public void AddAttackCount()
    {
        Debug.Log("Add AttackCount");
        if (attackCount < maxAttackCount)
        {
            attackCount++;
        }
        --attackstack;
    }

    public void AttackCountReset()
    {
        if (attackCount != 0)
        {
            if (IsGround())
            {
                if (lastAttackTime + attackComboTime < Time.time || attackCount > maxAttackCount)
                {
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
            return false;
        }
        if (weaponHandler == null) //|| !weaponHandler.AbleFire()
        {
            return false;
        }
        if (attackCount >= maxAttackCount)
        {
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
                    attackCount = maxAttackCount;
                    isFireButtonPressed = true;
                    return true;
                }
                if (weapon == 1)
                {
                    attackCount = maxAttackCount;

                    isFireButtonPressed = true;
                    return true;
                }
            }
            else
            {
                if (weapon == 0)
                {
                    isFireButtonPressed = true;
                    ++attackstack;
                    return true;
                }
                if (weapon == 1)
                {
                    isFireButtonPressed = true;
                    ++attackstack;
                    return true;
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



    public void PlayKnockBackParticle()
    {
        playerEffectHandler.PlayParticle();
    }
    public void StopKnockBackParticle()
    {
        playerEffectHandler.StopParticle();
    }
    public void SubscribeToPlayerActionEvents(ref PlayerActionEvents _playerActionEvents)
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
        //FixedUpdate
        _playerActionEvents.OnPlyaerFixedUpdate += OnPlayerFixedUpdate;
        //TakeDamage
        _playerActionEvents.OnTakeDamage += OnTakeDamage;
        //PlyaerInit
        _playerActionEvents.OnPlyaerInit += OnPlyaerInit;
        //OnVictory
        _playerActionEvents.OnVictory += OnVictory;

    }
    void OnPlyaerInit()
    {
        canMove = true;
        AnimationTrigger = false;
        stateMachine.ChangeState(moveState);
    }
    public void OnTakeDamage(int _force, bool _tf)
    {
        SetCanMove(false);
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

    void OnVictory()
    {
        state = (int)EStateType.Victory;
        state2 = UnityEngine.Random.Range(0, 4);
    }

    


}
