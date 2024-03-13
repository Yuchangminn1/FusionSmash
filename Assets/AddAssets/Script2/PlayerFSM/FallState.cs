using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : PlayerState
{
    float gravityOrigin = 0.1f;
    

    public FallState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        endMotionChange = false;
        isAbleFly = true;
        isState2 = true;

    }

    public override void Enter()
    {
        base.Enter();
        startTime = Time.time;
        
    }
    public override bool Update()
    {
        if (player.isFireButtonPressed)
        {
            FinishAttackCount();
            //player.SetAnimationTrigger(false);
            player.nextState = player.attackState;
            return true;
        }
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
        
    }
    public override void LateUpdate()
    {
        base.LateUpdate();
    }
    public override void Exit()
    {
        
        player.SetState2(0);

        base.Exit();
    }
    private void FinishAttackCount()
    {
        player.SetAttackCount(player.maxAttackCount);
    }

}
