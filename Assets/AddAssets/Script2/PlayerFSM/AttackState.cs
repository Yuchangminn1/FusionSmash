
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
        endMotionChange = true;
        isAbleFly = true;
        isAbleAttack = false;
        isAbleDodge = false;
        isAbleJump = false;
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
        if (player.isFireButtonPressed && counter < 2)
        {
            ++counter;
            Combo.Enqueue(true);
            Debug.Log("콤보 추가");
            player.isFireButtonPressed = false;

        }

        //player.ZeroVelocity();
        if (!player.animationTrigger)
        {
            Debug.Log("애니메이션 트리거 끝남");

            if (Combo.TryDequeue(out bool Q))
            {
                Debug.Log($"카운터 = {counter}");

                player.animationTrigger = true;

                return true;

            }
            else
            {
                Debug.Log("카운터 없음");
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
       // Debug.Log("AttackState");
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
        Debug.Log("Attack Exit");

    }
}
