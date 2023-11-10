using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Entity : MonoBehaviour
{
    
    //Component

    protected Animator animator;

    //FSM
    protected StateMachine stateMachine;

    public Vector3 entityDir;
    public bool animationTrigger = false;
    public bool isGroggy = false;
    public bool isdead = false;
    public bool isStop = false;
    public int stateNum2 = 0;

    public LayerMask groundLayer; // 땅인지 확인하기 위한 레이어 마스크

    public int attackDagame;
    public int iGroggyMax = 100;
    public int iGroggy = 0;
    public int Hp { get { return hp; } set { hp = value; } }
    public int HpMax { get { return hpMax; } private set { } }

    [SerializeField] protected int hp;
    [SerializeField] protected int hpMax;

    [SerializeField] protected float GroundCheckDis = 0.65f;

    protected virtual void Awake()
    {
        stateMachine = new StateMachine();
    }
    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if(entityDir == null)
        {
            entityDir = transform.forward;
            Debug.Log(entityDir);
        } 
        ResetHP(hpMax);
    }

    
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * GroundCheckDis);
        //진행방향의 바닥을 체크 앞 방향 디렉션으로 받아서 플러스해야지
        //Gizmos.DrawRay(transform.position, Vector3.down * GroundCheckDis);
    }
    public virtual void Hit(int _atk)
    {
        if (hp - _atk > 0)
        {
            hp -= _atk;
        }
        else
        {
            hp = 0;
            isdead = true;
        }
    }
    


    #region RayCastCheck
    public bool IsGround(Transform transform)
    {
        //발밑체크
        return Physics.Raycast(transform.position, Vector3.down, GroundCheckDis, groundLayer);
    }
    public bool DirIsGround(Transform transform)
    {
        //진행방향의 바닥을 체크 앞 방향 디렉션으로 받아서 플러스해야지
        //return Physics.Raycast(transform.position + player.dir , Vector3.down, GroundCheckDis, groundLayer);
        return Physics.Raycast(transform.position , Vector3.down, GroundCheckDis, groundLayer);
    }

    #endregion
    protected void ResetHP(int _hpMax)
    {
        hpMax = _hpMax;
        hp = hpMax;
    }

    #region Animator
    public void SetInt(string _parameters, int _num) => animator.SetInteger(_parameters, _num);
    public void SetFloat(string _parameters, float value) => animator.SetFloat(_parameters, value);
    public void ZeroHorizontal() => animator.SetFloat("Horizontal", 0f);
    public void AnimaPlay(string _name) => animator.Play(_name);
    public void SetState2(int _num)
    {
        stateNum2 = _num;
        SetInt("State2", _num);
    }
    public void StateChange(EntityState _newState)
    {
        stateMachine.ChangeState(_newState);
        //Debug.Log("_newState = " + _newState);
    }
    #endregion
    //public void ZeroVelocity() => rb.velocity = Vector2.zero;
    
    //public void ZeroVelocityX() => rb.velocity = new Vector2(0f, rb.velocity.y);
    



}
