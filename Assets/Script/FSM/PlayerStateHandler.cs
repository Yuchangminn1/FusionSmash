using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(ChangeState))]
    public int state { get; set; }
    [Networked(OnChanged = nameof(ChangeState2))]
    public int state2 { get; set; }

    public NetworkString<_16> nickName { get; set; }


    public float dodgeCount = 0f;

    public int jumpCount { get; set; }

    public bool isdead = false;

    public bool isStop = false;
    public bool isJumping = false;

    //체력회복 
    public bool isHeal = false;
    public int fHpHeal = 40;
    public int healNum;
    public int healNumMax = 3;



    [SerializeField] protected float GroundCheckDis = 0.65f;
    public LayerMask groundLayer; // 땅인지 확인하기 위한 레이어 마스크


    public bool animationTrigger = false;
    [SerializeField] Animator anima;

    CharacterController cc;

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




    Vector3 inputVec3;

    #endregion
    void Awake()
    {
        stateMachine = new StateMachine();

    }

    // Start is called before the first frame update
    void Start()
    {
        anima = transform.GetComponent<Animator>();
        cc = transform.GetComponent<CharacterController>();
        //networkCC = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        characterMovementHandler = transform.GetComponent<CharacterMovementHandler>();
        #region FSM_Initialize
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
        if (Object.HasInputAuthority)
        {
            nextState = moveState;
            this.ChangeState();
            //stateMachine.ChangeState(moveState);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Object.HasInputAuthority)
        {
            stateMachine.Update();
        }
    }
    void FixedUpdate()
    {
        if (Object.HasInputAuthority)
        {
            stateMachine.FixedUpdate();
            isJumpButtonPressed = false;
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
        if (Object.HasInputAuthority)
        {
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
    public bool IsGround()
    {
        //발밑체크
        return Physics.Raycast(transform.position, Vector3.down, GroundCheckDis, groundLayer);
    }
    #region Animator
    public void SetInt(string _parameters, int _num) => anima.SetInteger(_parameters, _num);
    public void SetFloat(string _parameters, float value) => anima.SetFloat(_parameters, value);
    public void ZeroHorizontal() => anima.SetFloat("Horizontal", 0f);
    public void AnimaPlay(string _name) => anima.Play(_name);
    public void SetState(int num)
    {
        state = num;
        //SetInt("State", state);
        if(Object.HasInputAuthority)
            RPC_SetState(state);
    }
    public void SetState2(int num)
    {
        state2 = num;
        //SetInt("State2", state2);
        if (Object.HasInputAuthority)
            RPC_SetState2(state2);
    }

    //public void StateChange(EntityState _newState)
    //{
    //    stateMachine.ChangeState(_newState);
    //}

    public void ChangeState()
    {
        if (Object.HasInputAuthority)
        {
            stateMachine.ChangeState(nextState);
        }
    }
    #endregion
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetState(int _state, RpcInfo info = default)
    {
       // Debug.Log($"[RPC] State{state}");
        state = _state;
        //SetInt("State", state);

    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetState2(int _state2, RpcInfo info = default)
    {
        //Debug.Log($"[RPC] State2 {state2} ");
        state2 = _state2;
        //SetInt("State2", state2);

    }


}
