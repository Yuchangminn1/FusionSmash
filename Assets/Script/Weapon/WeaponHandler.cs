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

    public LayerMask collisionLayer;

    float lastTimeFire = 0;

    Vector3 firePosition;

    Vector3 fireDirection;

    bool justPressed;
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
    public void SetFire(Vector3 _firePosition, Vector3 _fireDirection,bool _justPressed)
    {
        firePosition = _firePosition;
        fireDirection = _fireDirection;
        justPressed = _justPressed;
    }

    public void Fire()
    {
        float hitDistance = 100;
        _equipWeapon.Fire(firePosition, fireDirection);
    }

    public bool AbleFire()
    {
        return _equipWeapon.AbleFire(justPressed);
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
        if (playerWeaponPrefab != null)
        {
            foreach (GameObject t in playerWeaponPrefab)
            {
                t.SetActive(false);
            }
        }
    }

}
