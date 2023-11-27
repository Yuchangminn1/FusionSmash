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
        endMotionChange = false;
        isAbleAttack = false;
        isAbleDodge = false;
    }

    public override void Enter()
    {
        player.isJumpButtonPressed = false;
        if (player.jumpCount >= 2)
            return;
        base.Enter();
        //player.SetState2(player.jumpCount);
        //player.jumpCount += 1;

        player.isJumping = true;
       
    }
    public override bool Update()
    {
        

        if (Input.GetKeyDown(KeyCode.C) && player.dodgeCount == 0f)
        {

            player.nextState = player.dodgeState;

            //player.StateChange(player.dodgeState);
            return true;
        }
        //if (Input.GetKeyDown(KeyCode.Space) && (player.jumpCount < 2))
        //{
        //    player.SetState2(player.jumpCount);
        //    Debug.Log("DoubleJump");
        //    return true;

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
                //player.StateChange(player.moveState);
                return true;
            }
            player.nextState = player.fallState;
            //player.StateChange(player.fallState);
            return true;
        }
        if (player.isJumpButtonPressed)
        {
            player.StateChange(player.jumpState);
            //player.animationTrigger = false;

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
        player.animationTrigger = false;
        player.isJumping = false;
        chageCount = 0;
    }


}
