using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class HitState : PlayerState
{
    public int damage = 0;
    public HitState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        endMotionChange = true;
        isAbleAttack = false;
        isAbleDodge = false;
        isAbleJump = false;
    }

    public override void Enter()
    {
        base.Enter();
        player.isHit = false;

    }


    public override bool Update()
    {
        if (player.isHit)
        {
            player.AnimationTrigger = false;

            player.ChangeState();

            return true;
        }
        return base.Update();

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

    }
}
