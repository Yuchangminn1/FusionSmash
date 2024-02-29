using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class JumpState : PlayerState
{
    public JumpState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
        isAbleFly = true;
        endMotionChange = true;
        isAbleAttack = true;
        isAbleDodge = true;
        isAbleJump = false; 
        isState2 = true;
    }

    public override void Enter()
    {
        base.Enter();

        player.isJumpButtonPressed = false;
    }
    public override bool Update()
    {
        if (player.isDodgeButtonPressed)
        {
            player.nextState = player.dodgeState;
            return true;
        }
        if (player.isJumpButtonPressed)
        {
            player.isJumpButtonPressed = false;
            player.SetAnimationTrigger(false);
            player.ChangeState();
            return true;
        }
        if (player.isFireButtonPressed && !player.attackCoolDownOn)
        {
            if (!player.attackCoolDownOn)
            {
                //player.SetAnimationTrigger(false);
                player.nextState = player.attackState;
                return true;
            }

        }
        if (!player.Isvisi())
        {
            if (player.nextState != this)
            {
                return true;
            }

            if (base.Update())
                return true;

            if (player.IsGround())
            {
                player.nextState = player.moveState;
                return true;
            }
            player.nextState = player.fallState;
            return true;
        }

        return false;


    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

    }
    public override void LateUpdate()
    {


        base.LateUpdate();
    }
    public override void Exit()
    {
        base.Exit();

    }


}
