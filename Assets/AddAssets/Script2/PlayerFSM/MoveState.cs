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
        isAbleFly = false;

    }

    public override void Enter()
    {
        base.Enter();
        
       // Debug.Log("����ī��Ʈ ���� �߾��");
    }

    public override bool Update()
    {
        
        if (player.nextState != this)
        {
            return true;
        }
        if (base.Update())
            return true;
        
        return false;
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if(player.state2 != 0 && player.IsGround())
        {
            player.state2 = 0;
            player.SetState2(player.state2);
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
