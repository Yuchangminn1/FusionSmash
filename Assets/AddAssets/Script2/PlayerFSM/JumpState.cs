using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class JumpState : PlayerState
{
    int chageCount = 0; 
    public JumpState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
        isAbleFly = true;
        endMotionChange = true;
        isAbleAttack = true;
        isAbleDodge = true;
        isAbleJump = false; // 플레이어 스테이트에서 영향받기 싫어서 
    }

    public override void Enter()
    {
        player.isJumpButtonPressed = false;
        base.Enter();
        //player.SetState2(player.jumpCount);
        //player.jumpCount += 1;

        player.isJumping = true;
       
    }
    public override bool Update()
    {
        if (player.isDodgeButtonPressed )
        {
            player.nextState = player.dodgeState;
            return true;
        }
        if (player.isJumpButtonPressed)
        {
            player.nextState = player.jumpState;
            return true;
        }
        //if (player.isJumpButtonPressed)
        //{
        //    player.StateChange(player.jumpState);
        //    //player.animationTrigger = false;
        //}
        if (!player.animationTrigger)
        {
            if (player.nextState != this)
            {
                return true;
            }

            if (base.Update())
                return true;

            if (player.IsGround() && player.cc.velocity.y < 0.01f)
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
        //Debug.Log($"이건 점프 스테이트 레이트  {player.jumpCount}");


        base.LateUpdate();
    }
    public override void Exit()
    {
        base.Exit();
        player.isJumping = false;
        chageCount = 0;
    }


}
