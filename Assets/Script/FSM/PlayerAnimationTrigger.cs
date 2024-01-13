using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTrigger : NetworkBehaviour
{
    PlayerStateHandler player;
    Animator animator;
    TestWeapon testweapon;
    // Start is called before the first frame update
    private void Awake()
    {
        player = GetComponentInParent<PlayerStateHandler>();
        animator = transform.GetComponent<Animator>();
    }
    void Start()
    {
        testweapon = GetComponentInChildren<TestWeapon>();
        if(testweapon != null)
        {
            testweapon.meshcol.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    void AnimationTrigger()
    {
        if (Object.HasInputAuthority)
        {
            Debug.Log("트리거에서 OFF");
            player.animationTrigger = false;
        }
    }

    void AttackColOn()
    {
        if (Object.HasInputAuthority)
        {
            //임시로 보관 나중에 웨폰 핸들러든 다른 곳에 배치
            testweapon.SetDirect(true);
            //Debug.Log("공격시도");
            if (testweapon != null)
                testweapon.WeaponColOn();
            else
            {
                Debug.Log($"testweapon = Null");
            }
        }
    }
    void AttackColOff()
    {
        if (Object.HasInputAuthority)
        {
            //임시로 보관 나중에 웨폰 핸들러든 다른 곳에 배치
            testweapon.SetDirect(true);
            //Debug.Log("공격시도");
            if (testweapon != null)
                testweapon.WeaponColOff();
            else
            {
                Debug.Log($"testweapon = Null");
            }
        }
    }
}
