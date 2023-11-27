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
    public override bool Update()
    {
        if (player.nextState != this)
        {
            return true;
        }
        if (base.Update())
            return true;
        if (player.IsGround())
        {
           // Debug.Log("Time = " + (Time.time - startTime));
            if (Time.time - startTime > 1f)
            {
                player.nextState = player.landState;

                //player.StateChange(player.landState);
                return true;
            }
            else
            {
                //Debug.Log("Fall to MoveState IS Ground On");
                player.nextState = player.moveState;

                //player.StateChange(player.moveState);
                return true;


            }
        }
        if ((Input.GetKeyDown(KeyCode.C))  && player.dodgeCount == 0f)
        {
            player.nextState = player.dodgeState;

            //player.StateChange(player.dodgeState);
            return true;

        }

        if (player.isJumpButtonPressed && (player.jumpCount < 2))
        {
            player.nextState = player.jumpState;

            //player.StateChange(player.jumpState);
            return true;

        }
        return false;

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //Debug.Log($"playerIsGround {player.IsGround()}");
        //player.CCMove();
        //if (!player.IsGround())
        //{
        //    player.CCGravity(player.gravity);
        //    player.gravity += 0.1f;
        //}
    }
    public override void LateUpdate()
    {
        base.LateUpdate();
    }
    public override void Exit()
    {
        //player.PlayerAnimaSetBool("Falling", false);
        //player.ResetGravity();
        base.Exit();
    }

    
}
