using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEditor;
using System;
using Unity.VisualScripting;
using System.IO.Pipes;
using TMPro;
using UnityEngine.UI;

public class WeaponHandler : NetworkBehaviour
{
    public List<GameObject> playerWeaponPrefab;
    public PlayerWeapon _equipWeapon { get; private set; }

    //public ParticleSystem fireParticleSystem;
    public LayerMask collisionLayer;

    float lastTimeFire = 0;

    


    //HPHandler hpHandler;

    void Awake()
    {
        //hpHandler = GetComponent<HPHandler>();

    }



    public override void FixedUpdateNetwork()
    {

        //if (hpHandler.isDead)
        //{
        //    return;
        //}

    }
    //�÷��̾ �߻��ư�� ����
    public void Fire(Vector3 _firePosition, Vector3 _fireDirection, bool _isFireButtonPressed)
    {
        float hitDistance = 100;
        _equipWeapon.Fire(_firePosition, _fireDirection, _isFireButtonPressed);
    }
    public void ChangeWeapon(int num)
    {
        //�� ������Ʈ ü����
        if (_equipWeapon != null)
        {
            _equipWeapon.gameObject.SetActive(false);
            _equipWeapon.DisEquip();

        }
        _equipWeapon = playerWeaponPrefab[num].GetComponent<PlayerWeapon>();

        _equipWeapon.gameObject.SetActive(true);
        _equipWeapon.Equip();
    }


    public void SetEq()
    {
        //�Ⱦ��� ���� ���
        if (playerWeaponPrefab != null)
        {
            foreach (GameObject t in playerWeaponPrefab)
            {
                t.SetActive(false);
            }
        }
        

    }

}
