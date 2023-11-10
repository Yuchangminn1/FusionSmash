using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class HitState : PlayerState
{
    public int damage = 0;
    public HitState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
        isAbleAttack = false;  
        isAbleDodge = false;
    }

    public override void Enter()
    {
        base.Enter();
       // player.transform.localScale = new Vector3(player.entityDir * -1f, 1f,1f);

    }


    public override void Update()
    {
        base.Update();
        if(!player.animationTrigger)
        {
            if (player.isdead)
            {
                player.StateChange(player.deathState);
                return;
            }
            player.StateChange(player.moveState);
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();
       // player.transform.localScale = new Vector3(player.entityDir, 1f,1f);

    }
}
