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
    public List<NetworkObject> playerWeaponPrefab;
    public PlayerWeapon _equipWeapon { get; private set; }

    public LayerMask collisionLayer;

    Vector3 firePosition;

    Vector3 fireDirection;

    bool justPressed;

    float lastTimeFire = 0;
    public int weaponNum { get; private set; } = 0;

    public override void FixedUpdateNetwork()
    {


    }
    public void SetFire(Vector3 _firePosition, Vector3 _fireDirection,bool _justPressed)
    {
        firePosition = _firePosition;
        fireDirection = _fireDirection;
        justPressed = _justPressed;
    }

    public void Fire()
    {
        if(_equipWeapon != null)
        {
            _equipWeapon.Fire(firePosition, fireDirection);
        }
    }

    public bool AbleFire()
    {
        if (_equipWeapon != null)
        {
            return _equipWeapon.AbleFire();
        }
        else return false;
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
            foreach (NetworkObject t in playerWeaponPrefab)
            {
                t.gameObject.SetActive(false);
            }
        }
    }
    public PlayerWeapon GetEquipWeapon()
    {
        return _equipWeapon;
    }
}
