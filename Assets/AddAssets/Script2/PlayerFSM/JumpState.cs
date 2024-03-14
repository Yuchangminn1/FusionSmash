using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class JumpState : PlayerState
{
    public JumpState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        isAbleFly = true;
        endMotionChange = true;
        isState2 = true;
        isCancel = true;
    }

    public override void Enter()
    {
        base.Enter();

        player.isJumpButtonPressed = false;
    }
    public override bool Update()
    {
        if (player.jumpCount < player.maxJumpCount)
        {
            return false;
        }

        if (player.isDodgeButtonPressed)
        {
            player.nextState = player.dodgeState;
            return true;
        }
        if (player.isJumpButtonPressed)
        {
            player.isJumpButtonPressed = false;
            player.nextState = player.jumpState;
            player.ChangeState();
            return true;
        }
        if (player.isFireButtonPressed)
        {
            FinishAttackCount();
            player.nextState = player.attackState;
            return true;
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

    private void FinishAttackCount()
    {
        player.SetAttackCount(player.maxAttackCount);
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
