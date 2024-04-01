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
        isState2 = true;

    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("���� ����");
        player.isStop = true;

    }
    public override bool Update()
    {

        player.nextState = player.moveState;
        return true;
        if (!player.Isvisi())
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
        player.SetState2(0);

        player.isStop = false;

    }


}
