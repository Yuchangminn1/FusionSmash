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
        if (player.nextState != this)
        {
            return true;
        }
        if (base.Update())
            return true;

        if (player.isJumpButtonPressed)
        {
            player.nextState = player.jumpState;
            return true;
        }
        return false;
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (player.IsGround())
        {
            player.jumpCount = 0;
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
