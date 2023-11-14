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
        base.Enter();
        if(player.jumpCount < 2) 
        {
            player.SetState2(player.jumpCount);
            ++player.jumpCount;
            return;
        }
        if (!player.IsGround() )
        {
            player.StateChange(player.fallState);
            return;
        }
        
    }
    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.C) && player.dodgeCount == 0f)
        {
            player.animationTrigger = false;
            player.StateChange(player.dodgeState);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && (player.jumpCount < 2))
        {
            player.SetState2(player.jumpCount);
            ++player.jumpCount;
            Debug.Log("DoubleJump");
            return;
        }
        if (!player.animationTrigger)
        {
            if (player.IsGround())
            {
                player.StateChange(player.moveState);
            }
            else
            {
                player.StateChange(player.fallState);
            }
        }


    }
    public override void FixedUpdate()
    {
        
    }
    public override void Exit()
    {
        base.Exit();
    }

    
}
