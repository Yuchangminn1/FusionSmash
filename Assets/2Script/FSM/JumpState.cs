using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class JumpState : PlayerState
{
    public JumpState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        isAbleFly = true;
        endMotionChange = true;
        isState2 = true;
        isCancel = true;
    }

    public override void Enter()
    {
        base.Enter();
        player.isJumpButtonPressed = false;
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
