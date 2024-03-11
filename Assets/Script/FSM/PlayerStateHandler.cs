using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private bool animationTrigger { get; set; }

    public bool isdead = false;
    public bool isStop = false;
    public bool isHeal = false;
    public bool isHit = false;

    public int fHpHeal = 40;
    public int healNum;
    public int healNumMax = 3;

    [SerializeField] protected float GroundCheckDis = 0.65f;
    public LayerMask groundLayer;
    float groundCheckRad = 0.2f;

    [SerializeField] Animator anima;
    //Jump
    [Networked(OnChanged = nameof(ChangeJumpCount))] //
    public int jumpCount { get; set; } = 0;
    public int maxJumpCount { get; private set; } = 2;

    [Networked(OnChanged = nameof(ChangeAttackCount))] //
    public int attackCount { get; set; } = 0;
    public int maxAttackCount { get; private set; } = 3;
    

    #region FSM
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
    public CharacterMovementHandler characterMovementHandler;

    public bool isJumpButtonPressed = false;

    public bool isFireButtonPressed = false;

    public bool isDodgeButtonPressed = false;



    Vector3 inputVec3;

    #endregion

    public float lastAttackTime { get; set; } = 0f;

    float attackComboTime = 1.5f;

    //public float attackCoolDown { get; set; }

    //public bool attackCoolDownOn { get; set; }


    void Awake()
    {
        anima = GetComponent<Animator>();
        SetAnimationTrigger(false);
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {

        #region FSM_Initialize
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
        #endregion
        if (HasStateAuthority)
        {
            nextState = moveState;
            this.ChangeState();
        }
    }

    void Update()
    {
        if (HasStateAuthority)
        {
            stateMachine.Update();
        }

    }
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
    void FixedUpdate()
    {
        if (HasStateAuthority)
        {
            stateMachine.FixedUpdate();
        }
    }
    public override void FixedUpdateNetwork()
    {
        AnimationTrigger = animationTrigger;
    }

    private void LateUpdate()
    {
        SetFloat("InputX", inputVec3.x);
        SetFloat("InputZ", inputVec3.y);
    }

    public void StateChageUpdate()
    {
        if (stateMachine == null)
        {
            Debug.Log("stateMachine is Null");
            return;
        }

        if (HasStateAuthority)
        {
            stateMachine.Update();
            stateMachine.LateUpdate();
        }
    }
    public Vector3 SetInputVec(Vector3 vector3)
    {
        inputVec3 = vector3;
        return inputVec3;
    }

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
        Debug.Log(newS);
        if (newS != oldS)
        {
            changed.Behaviour.SetAnimationTrigger(newS);
            
        }
    }


    //Jump
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

            return true;
        }
        isJumpButtonPressed = false;
        return false;
    }
    public void Fire() 
    {
        characterMovementHandler.Fire();
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

    
    public void ResetJumpCount()
    {
        if (HasStateAuthority)
        {
            jumpCount = 0;
        }
    }


    //Ʈ���Ÿ� ���� ������� 

    public bool IsGround()
    {
        return characterMovementHandler.IsGround();
    }
    #region Animator
    public void SetBool(string _parameters, bool _tf) => anima.SetBool(_parameters, _tf);
    public void SetInt(string _parameters, int _num) => anima.SetInteger(_parameters, _num);
    public void SetFloat(string _parameters, float value) => anima.SetFloat(_parameters, value);
    public void ZeroHorizontal() => anima.SetFloat("Horizontal", 0f);
    public void AnimaPlay(string _name) => anima.Play(_name);
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
        //Debug.Log($"State2 = {state2}");
        if (HasStateAuthority)
        {
            state2 = num;
            SetInt("State2", num);
        }
    }

    public bool Isvisi()
    {
        return animationTrigger;
    }

    public void ChangeState()
    {
        if (HasStateAuthority)
        {
            stateMachine.ChangeState(nextState);
        }
    }
    #endregion

    public void SetCanMove(bool _tf)
    {
        characterMovementHandler.SetCanMove(_tf);
    }
    public void SetStopMove(bool _tf)
    {
        characterMovementHandler.SetStopMove(_tf);
    }
    public void SetAnimationTrigger(bool _tf)
    {
        animationTrigger = _tf;
        SetBool("AnimationTrigger", animationTrigger);
    }
    
    public void AttackEnter() 
    {
        if (attackCount < 3)
        {
            attackCount++;
        }
        SetStopMove(true);
        lastAttackTime = Time.time;
        isFireButtonPressed = false;
        
        Fire();
    }
    public EntityState GetCurrentState() 
    {
        if (stateMachine.GetState() == null) 
            return null;

        return stateMachine.GetState();
    }
    public bool AbleFire()
    {
        AttackCountReset();
        if (GetCurrentState() == null)
        {
            return false;
        }
        if (GetCurrentState().isAbleAttack && !Isvisi()) 
        {
            if(weapon == 0)
            {
                return true;
            }
            if (weapon == 1)
            {
                if (attackCount < 3)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    public void AttackCountReset()
    {
        if(lastAttackTime+attackComboTime < Time.time)
        {
            attackCount = 0;
        }

        //if(Time.time)
    }

}
