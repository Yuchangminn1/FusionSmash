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
    public bool animationTrigger { get; set; }
    public NetworkString<_16> nickName { get; set; }


    //public float dodgeCount = 0f;

    //public int jumpCount { get; set; }

    public bool isdead = false;

    public bool isStop = false;
    public bool isJumping = false;

    //ü��ȸ�� 
    public bool isHeal = false;
    public int fHpHeal = 40;
    public int healNum;
    public int healNumMax = 3;

    public int attackComboCount { get; set; }
    // public int attackComboInput { get; set; }

    [SerializeField] protected float GroundCheckDis = 0.65f;
    public LayerMask groundLayer; // ������ Ȯ���ϱ� ���� ���̾� ����ũ
    float groundCheckRad = 0.2f;



    [SerializeField] Animator anima;

    //public CharacterController cc;
    //public Rigidbody rb;

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

    public float attackTime { get; set; }

    public float attackComboTime { get; set; }

    public float attackCoolDown { get; set; }

    public bool attackCoolDownOn { get; set; }


    void Awake()
    {
        attackTime = 0f;
        attackComboTime = 1f;
        attackCoolDown = 1f;
        anima = GetComponent<Animator>();
        SetAnimationTrigger(false);
        //cc = GetComponent<CharacterController>();
        //networkCC = GetComponent<NetworkCharacterControllerPrototypeCustom>();
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
            
        //stateMachine.ChangeState(moveState);
    }

    // Update is called once per frame
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
            //isJumpButtonPressed = false;
            //isFireButtonPressed = false;
        }
    }


    private void LateUpdate()
    {

        SetFloat("InputX", inputVec3.x);
        SetFloat("InputZ", inputVec3.y);
        //if (Object.HasInputAuthority)
        //{
        //    stateMachine.LateUpdate();
        //}

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
            //changed.Behaviour.SetState(newS);

        }
    }
    static void ChangeState2(Changed<PlayerStateHandler> changed)
    {
        Debug.Log("ChangeState2 @@");
        int newS = changed.Behaviour.state2;
        changed.LoadOld();
        int oldS = changed.Behaviour.state2;
        if (newS != oldS)
        {
            changed.Behaviour.SetInt("State2", newS);

        }
    }




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
        if (animationTrigger)
        {
            return true;
        }
        return false;
    }

    ////������Ʈ ���� ���� �������� �����
    //public void StateChange(EntityState _newState)
    //{
    //    stateMachine.ChangeState(_newState);
    //}

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

    public void SetAnimationTrigger(bool _tf) 
    {
        animationTrigger = _tf;
        SetBool("AnimationTrigger", animationTrigger);
    }
}
