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

    }
    public override bool Update()
    {
        if (base.Update())
            return true;

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
