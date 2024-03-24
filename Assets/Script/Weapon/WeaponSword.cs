using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSword : PlayerWeapon
{
    
    MeshCollider meshCollider;
    PlayerInfo playerInfo;


    public override bool AbleFire()
    {
        return true;
    }

    public override void DisEquip()
    {
        base.DisEquip();
    }

    public override void Equip()
    {
        base.Equip();
    }

    public override void Fire(Vector3 firePosition, Vector3 fireDirection)
    {
        //Debug.Log("Collistion " + true);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }

    public override void OnRespawn()
    {
        base.OnRespawn();
    }

    public override void Spawned()
    {
        base.Spawned();
        //maxAttackCount = 2;

        _weaponNum = (int)EWeaponType.Sword;
        meshCollider = GetComponent<MeshCollider>();
        playerInfo = GetComponentInParent<PlayerInfo>();
        SetCollistion(false);


    }
    public override void SetCollistion(bool _tf)
    {
        meshCollider.enabled = _tf;
        Debug.Log("Collistion " + _tf);


    }
    private void OnTriggerEnter(Collider other)
    {
        if (_hPHandler == null)
        {
            Debug.Log("attackHP Is NUll");
            return;
        }
        // Debug.Log("OnTriggerEnter");
        HPHandler hitHP = other.GetComponent<HPHandler>();
        if (hitHP != null && hitHP != _hPHandler)
        {
            Vector3 tmp = other.transform.position - transform.position;
            if (tmp.x < 0.03)
            {//너무 가까우면 무기 기준으로 해서 반대로 날아감 
                tmp *= -1;
            }
            if ((int)Type < 0 || (int)Type > 3)
            {
                Debug.Log("Type Error");

                return;
            }
            hitHP.OnTakeDamage(playerInfo.GetName(), (int)Type, tmp, AttackType);

            if (hitHP.isDead)
            {
                //parentWeapon.KillEffect();
            }
            else
            {
                //parentWeapon.HitEffect();
            }

            //Runner.Despawn(Object);
        }
    }



}
