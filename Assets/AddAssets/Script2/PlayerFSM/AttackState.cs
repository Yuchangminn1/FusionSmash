
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerState
{
    int counter;
    Queue<bool> Combo;

    public AttackState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
        endMotionChange = false;
        isAbleFly = true;
        isAbleAttack = false;
        isAbleDodge = false;
    }

    public override void Enter()
    {
        base.Enter();
        Combo = new Queue<bool>();
        counter = 0;
        player.SetState2(counter);

        player.SetInt("Counter", counter);
        player.isFireButtonPressed = false;

        //player.Spawn(counter);
    }


    public override bool Update()
    {
        if (player.nextState != this)
        {
            return true;
        }
        if (player.isJumpButtonPressed)
        {
            Combo.Enqueue(true);
            Debug.Log("ÄÞº¸ Ãß°¡");
            player.isFireButtonPressed = false;

        }

        //player.ZeroVelocity();
        if (!player.animationTrigger)
        {
            
            if (Combo.TryDequeue(out bool Q) && counter < 2)
            {
            
                ++counter;
                player.SetState2(counter);
                player.animationTrigger = true;
                player.SetInt("Counter", counter);
                //player.Spawn(counter);
                Debug.Log("counter = " + counter);
                return true;

            }
            else
            {
                if (base.Update())
                {
                    return true;
                }

                //player.EndSpawn();
                if (player.IsGround())
                    player.nextState = player.moveState;

                //player.StateChange(player.moveState);
                else
                    player.nextState = player.fallState;

                //player.StateChange(player.fallState);

                return true;

            }

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
    //void Spawn()
    //{
    //    player.Spawn(counter);
    //}
    public override void Exit()
    {
        base.Exit();
        player.animationTrigger = false;

    }
}
