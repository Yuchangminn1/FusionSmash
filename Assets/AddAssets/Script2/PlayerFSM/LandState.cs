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
        isAbleFly = false;
        endMotionChange = true;
        isAbleAttack = false;
        isAbleDodge = false;
        isAbleJump = false;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("랜드 진입");
        player.isStop = true;

    }
    public override bool Update()
    {

        player.nextState = player.moveState;
        return true;
        if (!player.animationTrigger)
        {
            if (base.Update())
                return true;

            player.nextState = player.moveState;
            return true;
        }
        return false;
        
    }
    public override void LateUpdate()
    {
        base.LateUpdate();
    }
    public override void Exit()
    {
        base.Exit();
        player.state2 = 0;
        player.isStop = false;
        player.animationTrigger = false;

    }


}
