using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : PlayerState
{
    Vector3 dodgeDir;
    public DodgeState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
        isAbleAttack = false;
        isAbleFly = true;
        isAbleDodge = false;

    }
    public override void Enter()
    {
        base.Enter();
        
        ++player.dodgeCount;
        
        //if (dodgeDir == null || dodgeDir == Vector3.zero)
        //{
        //    player.StateChange(player.moveState);
        //}
    }
    public override void Update()
    {
        base.Update();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //player.CCDodge(dodgeDir);
    }
    public override void Exit()
    {
        base.Exit();
        //player.isDodge = false;

    }
}
