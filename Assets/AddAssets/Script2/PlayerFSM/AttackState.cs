
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
        Debug.Log("어택엔터");
        player.attackTime = Time.time;
        //Combo = new Queue<bool>();
        //counter = 0;
        //player.SetState2(counter);

        //player.SetInt("Counter", counter);
        //Debug.Log($"State2 = {player.state2}");
        player.isFireButtonPressed = false;


        if (player.attackComboTime != 0 && player.attackTime + player.attackComboTime < Time.time)
        {
            player.attackComboCount = 0;
            Debug.Log("어택카운트 초기화 무브 스테이트");
        }

        Debug.Log($"player.attackComboCount = {player.attackComboCount}");
        player.SetState2(player.attackComboCount);
        player.attackComboCount += 1;
        //player.Spawn(counter);
    }


    public override bool Update()
    {

        //if (player.isFireButtonPressed && counter < 2)
        //{
        //    //++counter;
        //    //Combo.Enqueue(true);
        //    Debug.Log("콤보 추가");
        //    player.isFireButtonPressed = false;

        //}

        //player.ZeroVelocity();


        //if (startTime + 2f < Time.time)
        //{
        //    Debug.Log("애니메이션트리거 버그");
        //    player.animationTrigger = false;
        //}
        if (startTime + 0.3f > Time.time)
        {
            player.isFireButtonPressed = false;
        }
        if (!player.animationTrigger)
        {
            if (player.isFireButtonPressed && player.attackComboCount < 3 && !player.attackCoolDownOn)
            {
                if (!player.attackCoolDownOn)
                {
                    player.nextState = player.attackState;
                    player.ChangeState();
                    startTime = Time.time;
                    return true;
                }
                
            }

            if (base.Update())
            {
                return true;
            }
            if (player.IsGround())
            {
                player.nextState = player.moveState;
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
        if (player.attackComboCount > 2)
        {
            player.attackCoolDownOn = true;
            Debug.Log("어택 엑시트에서 어택 카운트 초기화");

            player.attackComboCount = 0;
            player.isFireButtonPressed = false;
        }
        player.attackTime = Time.time;
        //Debug.Log("Attack Exit");

    }
}
