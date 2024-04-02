using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : PlayerState
{
    //float gravityOrigin = 0.1f;
    

    public FallState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        endMotionChange = false;
        isAbleFly = true;
        isState2 = true;
        isCancel = true;
    }

    public override void Enter()
    {
        base.Enter();
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
