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
using System.IO;

public class WeaponHandler : NetworkBehaviour, IPlayerActionListener
{

    [Networked(OnChanged = nameof(ChangedHideWeapon))]
    public NetworkBool hideWeapon { get; set; } = false;
    public List<NetworkObject> playerWeaponPrefab;
    public PlayerWeapon equipWeapon { get; private set; }

    public LayerMask collisionLayer;

    Vector3 firePosition;

    Vector3 fireDirection;


    bool justPressed;

    //float lastTimeFire = 0;
    static void ChangedHideWeapon(Changed<WeaponHandler> changed)
    {
        bool newS = changed.Behaviour.hideWeapon;
        changed.Behaviour.SetEquipWeapon(newS);
    }

    void SetEquipWeapon(bool _tf)
    {
        equipWeapon.gameObject.SetActive(!_tf);
    }
    public override void Spawned()
    {
        SetEq();
        ChangeWeapon((int)EWeaponType.Sword);
    }
    

    public void SetFire(Vector3 _firePosition, Vector3 _fireDirection,bool _justPressed)
    {
        firePosition = _firePosition;
        fireDirection = _fireDirection;
        justPressed = _justPressed;
    }



    public bool AbleFire()
    {
        if (equipWeapon != null)
        {
            return equipWeapon.AbleFire();
        }
        else return false;
    }

    public void ChangeWeapon(int num)
    {
        //�� ������Ʈ ü����
        if (equipWeapon != null)
        {

            equipWeapon.gameObject.SetActive(false);
            equipWeapon.DisEquip();

        }
        equipWeapon = playerWeaponPrefab[num].GetComponent<PlayerWeapon>();
        equipWeapon.gameObject.SetActive(true);
        equipWeapon.Equip();
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
        return equipWeapon;
    }

    #region Event
    public void SubscribeToPlayerActionEvents(ref PlayerActionEvents _playerActionEvents)
    {
        _playerActionEvents.OnPlayerAttack += OnPlayerAttack;
        _playerActionEvents.OnPlyaerRespawn += OnPlyaerRespawn;
        _playerActionEvents.OnVictory += OnVictory;
    }
    void OnVictory()
    {

        if (equipWeapon != null)
        {
            hideWeapon = true;
        }
    }
    public void OnPlayerAttack()
    {
        if (equipWeapon != null)
        {
            //Debug.Log("Attack Event");
            equipWeapon.Fire(firePosition, fireDirection);
        }
    }
    void OnPlyaerRespawn()
    {
        hideWeapon = false;
        equipWeapon.OnRespawn();
    }
    #endregion
}
