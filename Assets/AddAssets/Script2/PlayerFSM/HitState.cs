using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class HitState : PlayerState
{
    public int damage = 0;
    public HitState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        isAbleAttack = false;  
        isAbleDodge = false;
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
        if(!player.Isvisi())
        {
            if (player.isdead)
            {

                player.nextState = player.deathState;

                return true;
            }
            player.nextState = player.moveState;

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

    }
}
