using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealState : PlayerState
{
    
    public HealState(PlayerStateHandler _player, int _currentStateNum) : base(_player, _currentStateNum)
    {
        player = _player;
        currentStateNum = _currentStateNum;
        endMotionChange = false;
        isAbleAttack = false;
    }

    public override void Enter()
    {
        base.Enter();
        //if(player.Hp >= player.HpMax || player.healNum == 0)
        //{
        //    Debug.Log("체력이 이미 가득 찼습니다");
        //    player.StateChange(player.moveState);
        //    return;
        //}
        player.isHeal = true;
        player.isStop = true;
        //player.ZeroVelocity();
    }
    public override void Update()
    {
        base.Update();
        if (!player.animationTrigger)
        {
            ReCoverHP();
            player.StateChange(player.moveState);
            return;
        }
    }
    
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();
        player.isHeal = false;
        player.isStop = false;

    }

    private void ReCoverHP()
    {
        player.healNum -= 1;
        //UIScript.instance.HPHealNumIcon(player.healNum);
        //UIScript.instance.PlayerEffect(2);
        //if (player.Hp + player.fHpHeal > player.HpMax)
        //{
        //    player.Hp = player.HpMax;
        //}
        //else
        //{
        //    player.Hp += player.fHpHeal;
        //}
        //UIScript.instance.HpBarReset(player.Hp, player.HpMax);
    }
}
