using System.Collections;
using System.Collections.Generic;
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
        Debug.Log("Move Enter");
        if (player.IsGround())
        {
            player.jumpCount = 0;
            player.dodgeCount = 0f;
        }
    }

    public override void Update()
    {
        base.Update();
        if(Input.GetKeyDown(KeyCode.E)) 
        {
            //player.healNum += UIScript.instance.ResetHPHealNumIcon(1);
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            //player.healNum += UIScript.instance.ResetHPHealNumIcon(player.healNumMax);

            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.StateChange(player.jumpState);
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //È¸º¹
            player.StateChange(player.healState);
            return;
        }
        
    }

    public override void Exit()
    {
        base.Exit();
    }
}
