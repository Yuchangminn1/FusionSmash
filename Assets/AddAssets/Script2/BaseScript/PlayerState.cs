using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Fusion.Simulation.Statistics;

public class PlayerState : EntityState
{
    protected PlayerStateHandler player;

    protected float airTime;


    //protected int currentStateNum;        현재 스테이트 넘
    //protected float stateTimer;
    //protected float startTime;                
    //protected bool endMotionChange = true; 끝나면 애니메이션 바꿔라트리거
    //protected bool isAbleFly = false; 이거 제거 
    //protected bool isAbleAttack = true;

    public PlayerState(PlayerStateHandler _player, int _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
    }
    public override void Enter()
    {
        base.Enter();
        player.state = currentStateNum;
        player.SetState(player.state);
        startTime = Time.time;

        if (endMotionChange)
        {
            player.animationTrigger = true;

        }
    }
    public override bool Update()
    {
        base.Update();

        stateTimer = Time.time;

        if (!isAbleAttack)
        {
            player.isFireButtonPressed = false;
        }

        if (player.nextState != this)
        {
            return true;
        }
        if (!isAbleFly)
        {
            //체공 불가능 상태일때
            if (!player.IsGround())
            {
                if (airTime == 0f)
                {
                    airTime = Time.time;
                }
                if (Time.time - airTime > 0.1f)
                {
                    player.nextState = player.fallState;

                    return true;
                }
            }
            else
            {
                airTime = 0f;
            }
        }
        if (player.isJumpButtonPressed && isAbleJump)
        {
            //더블점프용
            player.nextState = player.jumpState;
            return true;
        }
        if (player.isFireButtonPressed && isAbleAttack)
        {
            if (!player.attackCoolDownOn)
            {

                player.nextState = player.attackState;

                return true;
            }
        }
        if (player.isDodgeButtonPressed && isAbleDodge)
        {
            player.nextState = player.dodgeState;
            return true;
        }


        return BaseState();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (player.attackCoolDownOn && player.attackCoolDown + player.attackTime < Time.time)
        {
            player.attackCoolDownOn = false;
            Debug.Log("attackCoolDownOn 팔스");
            //player.attackComboCount = 0;
        }
        //틀어진 애니메이션 맞춰주기 ? 
        if (player.state != currentStateNum)
        {
            //Debug.Log($"player.state = {player.state} currentStateNum{currentStateNum}");
            player.SetState(currentStateNum);
        }
        //Debug.Log($"currentNum = {currentStateNum}");
        //Debug.Log(currentStateNum);

    }
    public override void LateUpdate()
    {

        if (player.nextState != this)
        {
            player.ChangeState();
        }
    }
    public override void Exit()
    {
        base.Exit();

        if (endMotionChange)
        {
            if (player.animationTrigger)
            {
                Debug.Log($"{currentStateNum}에서 버그로 에니매이션 트리거 펄스로 바꿨음");
                player.animationTrigger = false;

            }
        }
        
    }



    protected bool BaseState()
    {
        if (!player.animationTrigger && endMotionChange)
        {
            if (player.IsGround())
            {
                player.nextState = player.moveState;
                return true;
            }
            else
            {
                player.nextState = player.fallState;
                return true;
            }
        }
        return false;
    }
}
