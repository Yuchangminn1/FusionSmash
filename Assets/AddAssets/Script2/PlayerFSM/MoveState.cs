using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveState : PlayerState
{
    
    public MoveState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
    }

    public override void Enter()
    {
        
        base.Enter();
    }

    public override bool Update()
    {
        base.Update();
        //if(Input.GetKeyDown(KeyCode.E)) 
        //{
        //    //player.healNum += UIScript.instance.ResetHPHealNumIcon(1);
        //    return;
        //}
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    //player.healNum += UIScript.instance.ResetHPHealNumIcon(player.healNumMax);

        //    return;
        //}
        if (player.isJumpButtonPressed)
        {
            player.nextState = player.jumpState;

            //player.StateChange(player.jumpState);
            return true;
        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    //회복
        //    player.StateChange(player.healState);
        //    return;
        //}
        return false;
    }
    public override void FixedUpdate()
    {
        if (player.IsGround())
        {
            //if(player.characterMovementHandler.jumpcount != 0)
            //{
            //    //player.characterMovementHandler.jumpcount = 0;
            //    Debug.Log("점프 카운트 초기화");
            //}

            //player.jumpCount = 0;
            player.dodgeCount = 0f;
        }
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
