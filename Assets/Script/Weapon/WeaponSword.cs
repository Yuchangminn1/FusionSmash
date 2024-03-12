using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSword : PlayerWeapon
{
    MeshCollider meshCollider;
    public override bool AbleFire(bool justPressed)
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
        SetCollistion(true);
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
        meshCollider = GetComponent<MeshCollider>();
        SetCollistion(false);
    }
    public override void SetCollistion(bool _tf)
    {
        meshCollider.enabled = _tf;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (_hPHandler == null)
        {
            Debug.Log("attackHP Is NUll");
            return;
        }
        Debug.Log("OnTriggerEnter");
        HPHandler hitHP = other.GetComponent<HPHandler>();
        if (hitHP != null && hitHP != _hPHandler)
        {
            hitHP.enemyHPHandler = _hPHandler;
            Vector3 tmp = other.transform.position - transform.position;
            //tmp.y = 0;
            //other.GetComponent<Rigidbody>().AddForce(tmp.normalized * hitHP.AddForce, ForceMode.VelocityChange);
            if (_hPHandler._nickName == null)
            {
                Debug.Log("attackHP._nickName Is Null");
                _hPHandler._nickName = "tmp";
                return;
            }
            if ((int)Type < 0 || (int)Type > 3)
            {
                Debug.Log("Type Error");

                return;
            }
            hitHP.OnTakeDamage(_hPHandler._nickName, (int)Type);
            other.GetComponent<CharacterMovementHandler>().HitAddForce(tmp, hitHP.AddForce);

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
