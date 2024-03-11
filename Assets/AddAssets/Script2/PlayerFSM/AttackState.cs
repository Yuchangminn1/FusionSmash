
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerState
{
    int counter;
    Queue<bool> Combo;
    public AttackState(PlayerStateHandler _player,  int _currentStateNum) : base(_player,  _currentStateNum)
    {
        endMotionChange = true;
        isAbleFly = true;
        isAbleAttack = false;
        isAbleJump = false;
        isState2 = true;
    }

    public override void Enter()
    {
        base.Enter();
        player.AttackEnter();
    }
    public override bool Update()
    {
        AnimationTriggerErrorCheck();
        if (!player.Isvisi())
        {
            if (base.Update()) return true;
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
        player.SetStopMove(false);
        player.lastAttackTime = Time.time;
    }
    private void AnimationTriggerErrorCheck()
    {
        if (startTime + 5f < Time.time)
        {
            player.SetAnimationTrigger(false);
            
            Debug.Log("AnimationTrigger Error State =  " + player.GetCurrentState().currentState);
        }
        
    }
}
