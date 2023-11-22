using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : PlayerState
{
    public DeathState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
        endMotionChange = false;
        isAbleFly = true;
        isAbleAttack = false;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override bool Update()
    {

        if (!player.animationTrigger)
        {

            ;

        }
        return false;
    }
    

    public override void FixedUpdate()
    {

    }
    public override void LateUpdate()
    {
        base.LateUpdate();
    }
    public override void Exit()
    {

    }

}
