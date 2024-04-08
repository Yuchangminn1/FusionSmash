using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimationTrigger : NetworkBehaviour
{
    PlayerStateHandler player;
    Animator animator;
    TestWeapon testweapon;
    WeaponHandler weaponHandler;
    // Start is called before the first frame update
    private void Awake()
    {
        player = GetComponentInParent<PlayerStateHandler>();
        weaponHandler = GetComponentInParent<WeaponHandler>();

        animator = transform.GetComponent<Animator>();
    }
    void Start()
    {
        testweapon = GetComponentInChildren<TestWeapon>();
        if (testweapon != null)
        {
            testweapon.meshcol.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    void AnimationTriggerOFF()
    {
        if (HasStateAuthority || HasInputAuthority)
        {
            player.AnimationTrigger = false;
        }
    }

    void AnimationTriggerOn()
    {
        if (HasStateAuthority || HasInputAuthority)
        {
            player.AnimationTrigger = true;
        }
    }

    void AttackColOn()
    {
        weaponHandler.GetEquipWeapon().SetCollistion(true);
    }
    void AttackColOff()
    {
        weaponHandler.GetEquipWeapon().SetCollistion(false);
    }
    //void AttackColOn()
    //{
    //    if (Object.HasInputAuthority)
    //    {
    //        //�ӽ÷� ���� ���߿� ���� �ڵ鷯�� �ٸ� ���� ��ġ
    //        testweapon.SetDirect(true);
    //        //Debug.Log("���ݽõ�");
    //        if (testweapon != null)
    //            testweapon.WeaponColOn();
    //        else
    //        {
    //            Debug.Log($"testweapon = Null");
    //        }
    //    }
    //}
    //void AttackColOff()
    //{
    //    if (Object.HasInputAuthority)
    //    {
    //        //�ӽ÷� ���� ���߿� ���� �ڵ鷯�� �ٸ� ���� ��ġ
    //        testweapon.SetDirect(true);
    //        //Debug.Log("���ݽõ�");
    //        if (testweapon != null)
    //            testweapon.WeaponColOff();
    //        else
    //        {
    //            Debug.Log($"testweapon = Null");
    //        }
    //    }
    //}
}
