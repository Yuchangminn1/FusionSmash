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


    //protected int currentStateNum;        ���� ������Ʈ ��
    //protected float stateTimer;
    //protected float startTime;                
    //protected bool endMotionChange = true; ������ �ִϸ��̼� �ٲ��Ʈ����
    //protected bool isAbleFly = false; �̰� ���� 
    //protected bool isAbleAttack = true;
    //protected bool isState2=false;
    public PlayerState(PlayerStateHandler _player, int _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
    }
    public override void Enter()
    {
        base.Enter();
        player.SetState(currentStateNum);
        startTime = Time.time;

        if (endMotionChange)
        {
            //player.SetAnimationTrigger(true);
            ;
        }
        if (!isState2) 
        {
            player.SetState2(0);
        } 
        //Debug.Log($"Enter{currentStateNum}State");
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
            //ü�� �Ұ��� �����϶�
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
            //����������
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
            Debug.Log("attackCoolDownOn �Ƚ�");
            //player.attackComboCount = 0;
        }
        //Ʋ���� �ִϸ��̼� �����ֱ� ? 
        //if (player.state != currentStateNum)
        //{
        //    //Debug.Log($"player.state = {player.state} currentStateNum{currentStateNum}");
        //    player.SetState(currentStateNum);
        //}
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
    }



    protected bool BaseState()
    {
        if (!player.Isvisi() && endMotionChange)
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
