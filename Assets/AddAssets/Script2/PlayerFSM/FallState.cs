using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : PlayerState
{
    float gravityOrigin = 0.1f;
    

    public FallState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
        endMotionChange = false;
        isAbleFly = true;
    }

    public override void Enter()
    {
        base.Enter();
        startTime = Time.time;
        //Debug.Log("endMotionChange " + endMotionChange);
       // player.gravity = gravityOrigin;
    }
    public override void Update()
    {
        base.Update();
        if (player.IsGround())
        {
           // Debug.Log("Time = " + (Time.time - startTime));
            if (Time.time - startTime > 1f)
            {
                player.StateChange(player.landState);
                return;
            }
            else
            {
                //Debug.Log("Fall to MoveState IS Ground On");
                player.StateChange(player.moveState);
                return;

            }
        }
        if ((Input.GetKeyDown(KeyCode.C))  && player.dodgeCount == 0f)
        {
            player.animationTrigger = false;
            player.StateChange(player.dodgeState);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && (player.jumpCount < 2))
        {
            player.StateChange(player.jumpState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        //player.CCMove();
        //if (!player.IsGround())
        //{
        //    player.CCGravity(player.gravity);
        //    player.gravity += 0.1f;
        //}
    }
    public override void Exit()
    {
        //player.PlayerAnimaSetBool("Falling", false);
        //player.ResetGravity();
        base.Exit();
    }

    
}
