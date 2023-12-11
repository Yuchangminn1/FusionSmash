using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(ChangeState))]
    public int state { get; set; }
    [Networked(OnChanged = nameof(ChangeState2))]
    public int state2 { get; set; }

    public NetworkString<_16> nickName { get; set; }


    //public float dodgeCount = 0f;

    //public int jumpCount { get; set; }

    public bool isdead = false;

    public bool isStop = false;
    public bool isJumping = false;

    //체력회복 
    public bool isHeal = false;
    public int fHpHeal = 40;
    public int healNum;
    public int healNumMax = 3;

    public int attackComboCount { get; set; }
   // public int attackComboInput { get; set; }

    [SerializeField] protected float GroundCheckDis = 0.65f;
    public LayerMask groundLayer; // 땅인지 확인하기 위한 레이어 마스크


    public bool animationTrigger { get; set; }
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

    public bool attackCoolDownOn {  get; set; }

    float groundCheckRad = 0.2f;

    void Awake()
    {
        attackTime = 0f;
        attackComboTime = 1f;
        attackCoolDown = 1f;
        anima = GetComponent<Animator>();
        animationTrigger = false;
        //cc = GetComponent<CharacterController>();
        //networkCC = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>();


    }
    public void JUMPCOUNT()
    {
        characterMovementHandler.JUMPCOUNT();
    }
    public void JUMPCOUNTRESET()
    {
        characterMovementHandler.JUMPCOUNTRESET();
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            animationTrigger = false;
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
        if (Object.HasInputAuthority)
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

        if (Object.HasInputAuthority)
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
        //발밑체크


    }
    #region Animator
    public void SetInt(string _parameters, int _num) => anima.SetInteger(_parameters, _num);
    public void SetFloat(string _parameters, float value) => anima.SetFloat(_parameters, value);
    public void ZeroHorizontal() => anima.SetFloat("Horizontal", 0f);
    public void AnimaPlay(string _name) => anima.Play(_name);
    public void SetState(int num)
    {
        state = num;
        //Debug.Log($"스테이트  = {state}");
        SetInt("State", num);
        if (Object.HasInputAuthority)
        {
            //state = num;
            RPC_SetState(state);
        }
    }
    public void SetState2(int num)
    {
        state2 = num;
        //Debug.Log($"State2 = {state2}");
        SetInt("State2", num);
        if (Object.HasInputAuthority)
        {
            RPC_SetState2(state2);

        }
    }
    //스테이트 변경 더블 점프에서 사용함
    public void StateChange(EntityState _newState)
    {
        stateMachine.ChangeState(_newState);
    }

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
        //characterMovementHandler.playerstate = _state;

        //SetInt("State", state);

    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetState2(int _state2, RpcInfo info = default)
    {
        //Debug.Log($"[RPC] State2 {state2} ");
        state2 = _state2;

        
        //state2 = _state2;
        //if (state == 1 || state == 2)
        //{
        //    Debug.Log($"_state2 = {state2}");

        //    characterMovementHandler.playerjumpcount = state2;
        //}

        //SetInt("State2", state2);

    }


}
