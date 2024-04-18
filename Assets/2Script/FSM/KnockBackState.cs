using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackState : PlayerState
{
    public KnockBackState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        isAbleFly = true;
        endMotionChange = true;
        isAbleAttack = false;
        isAbleDodge = false;
        isAbleJump = false;
    }
    public override void Enter()
    {
        base.Enter();
        player.PlayKnockBackParticle();
    }
    public override bool Update()
    {
        if (!player.AnimationTrigger)
        {
            player.isKnockBack = false;

            return base.Update();
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
        base.Exit();
        player.SetCanMove(true);
        player.StopKnockBackParticle();

    }
}
