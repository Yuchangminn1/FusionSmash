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
            //점프 스테이트 체인지할떄 여기서 캐릭터 무브먼트 핸들러로 점프 명령을 보내야함  
            //bool값으로 fixednetwork 에다가 
            //저거 틱이 이상해서 답이 없음 
            //아니면 JUMP함수 있는곳에다가 조건을 거는게 맞을 수 있음 
            
            Debug.Log($"State 2 = {player.jumpCount}");

            player.SetState2(player.jumpCount);
            ++player.jumpCount;
            player.isJumping = true;
            //++player.jumpCount;
            //player.SetState2(player.characterMovementHandler.jumpcountHas -1);

            //player.SetState2(player.jumpCount);
            return;
        }
        //if (!player.IsGround())
        //{
        //    player.nextState = player.fallState;

        //    //player.StateChange(player.fallState);
        //    return;
        //}

    }
    public override bool Update()
    {
        if(player.nextState != this)
        {
            return true;
        }

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
        if (player.IsGround())
        {
            player.nextState = player.moveState;

            //player.StateChange(player.moveState);
            return true;
        }
        if (!player.animationTrigger)
        {
            if (base.Update())
                return true;

            player.nextState = player.fallState;
            //player.StateChange(player.fallState);
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
        player.animationTrigger = false;
        player.isJumping = false;

    }


}
