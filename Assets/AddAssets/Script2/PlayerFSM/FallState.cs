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
        isAbleDodge = true;
        isAbleJump = true;
        isAbleAttack = true;
        isState2 = true;

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
        if (base.Update())
            return true;

        if (player.IsGround())
        {
            player.nextState = player.moveState;
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
        player.SetState2(0);

        base.Exit();
    }

    
}
