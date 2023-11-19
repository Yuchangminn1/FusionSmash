using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandState : PlayerState
{
    [SerializeField] private float landTime = 1f;

    public LandState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
        isAbleFly = true;
        isAbleAttack = false;
        isAbleDodge = false;
    }

    public override void Enter()
    {
        base.Enter();
        player.isStop = true;
        //player.ZeroVelocityX();

    }
    public override bool Update()
    {
        if (base.Update())
            return true;
        if (!player.animationTrigger)
        {
            player.StateChange(player.moveState);
            return true;
        }
        return false;
        
    }
    public override void Exit()
    {
        base.Exit();
        player.SetState2(0);
        player.isStop = false;

    }


}
