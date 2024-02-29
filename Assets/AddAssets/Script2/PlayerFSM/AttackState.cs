
using JetBrains.Annotations;
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

        player.SetCanMove(false);
        //Debug.Log("���ÿ���");
        player.attackTime = Time.time;
        //Combo = new Queue<bool>();
        //counter = 0;
        //player.SetState2(counter);

        //player.SetInt("Counter", counter);
        //Debug.Log($"State2 = {player.state2}");
        player.isFireButtonPressed = false;


        //if (player.attackComboTime != 0 && player.attackTime + player.attackComboTime < Time.time)
        //{
        //    player.attackComboCount = 0;
        //    Debug.Log("����ī��Ʈ �ʱ�ȭ ���� ������Ʈ");
        //}

       // Debug.Log($"player.attackComboCount = {player.attackComboCount}");
        //player.SetState2(player.attackComboCount);
        //player.attackComboCount += 1;

        //player.Spawn(counter);
    }


    public override bool Update()
    {

        if (startTime + 0.1f > Time.time)
        {
            player.isFireButtonPressed = false;
        }
        if (!player.Isvisi())
        {
            
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
        player.SetCanMove(true);

        player.attackTime = Time.time;

    }
}
