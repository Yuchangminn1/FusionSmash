
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
        player.SetInt("Counter", counter);
        //player.Spawn(counter);
    }
    

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Combo.Enqueue(true);
            Debug.Log("ÄÞº¸ Ãß°¡");
        }
        base.Update();
        //player.ZeroVelocity();
        if (!player.animationTrigger)
        {
            if (Combo.TryDequeue(out bool Q) && counter < 2f)
            {
                ++counter;
                player.animationTrigger = true;
                player.SetInt("Counter", counter);
                //player.Spawn(counter);
                Debug.Log("counter = " + counter);

            }
            else
            {
                //player.EndSpawn();
                if (player.IsGround())
                    player.StateChange(player.moveState);
                else
                    player.StateChange(player.fallState);
            }
            
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    //void Spawn()
    //{
    //    player.Spawn(counter);
    //}
    public override void Exit()
    {
        base.Exit();
    }
}
