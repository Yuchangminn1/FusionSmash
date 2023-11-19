using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
        isAbleFly = true;
        endMotionChange = false;
        isAbleAttack = false;
        isAbleDodge = false;
    }

    public override void Enter()
    {

        player.isJumpButtonPressed = false;

        base.Enter();
        if (player.jumpCount <2) 
        {
            player.isJumping = true;
            player.SetState2(player.jumpCount);
            
            return;
        }
        if (!player.IsGround() )
        {
            player.StateChange(player.fallState);
            return;
        }
        
    }
    public override bool Update()
    {
        if (base.Update())
            return true;

        if (Input.GetKeyDown(KeyCode.C) && player.dodgeCount == 0f)
        {
            player.animationTrigger = false;
            player.StateChange(player.dodgeState);
            return true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && (player.jumpCount < 2))
        {
            player.SetState2(player.jumpCount);
            Debug.Log("DoubleJump");
            return true;

        }
        if (!player.animationTrigger)
        {
            if (player.IsGround())
            {
                player.StateChange(player.moveState);
                return true;

            }
            else
            {
                player.StateChange(player.fallState);
                return true;

            }
        }
        return false;


    }
    public override void FixedUpdate()
    {
        
    }
    public override void Exit()
    {
        base.Exit();
        ++player.jumpCount;
        player.isJumping = false;


    }


}
