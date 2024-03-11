using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.UIElements;
//using static Fusion.NetworkCharacterController;

//public enum EWeaponGunType
//{
//    Pistol,
//    Rifle,
//    Shotgun,
//    Gravity
//}

public class Projectile : NetworkBehaviour
{
    public EWeaponType Type;
    PlayerWeapon parentWeapon;
    [Header("Projectile Setup")]
    public float Speed = 10f;
    public float MaxDistance = 100f;
    public float LifeTimeAfterHit = 2f;

    [Header("Impact Setup")]
    public GameObject ProjectileObject;
    public GameObject HitEffectPrefab;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private Vector3 _hitNormal;

    private bool _showHitEffect;
    private GameObject _hitEffectPrefab;
    private NetworkTransform _transform;

    private float _startTime;
    private float _lifetime = 3f;
    private HPHandler attackHP;

    float raycastDistance = 1f;
    public LayerMask layerMask;
    float radius = 0.25f;
   public void SetHit(Vector3 hitPosition, Vector3 hitNormal, bool showHitEffect)
    {
        _targetPosition = hitPosition;
        _showHitEffect = showHitEffect;
        _hitNormal = hitNormal;
    }
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            if (attackHP != null)
            {
                _transform.transform.position += _targetPosition.normalized * Speed;
                LifeTime();
            }
        }
    }
    private void Update()
    {
        if (HasStateAuthority && attackHP !=null)
        {
            CheckRay();
        }
    }
    
    public void SetTarget(Vector3 _vector3, PlayerWeapon _parentWeapon, HPHandler _attackHP)
    {
        parentWeapon = _parentWeapon;
        attackHP = _attackHP;
        _targetPosition = _vector3;
        _targetPosition -= _startPosition;
        //CheckRay();
    }
    public override void Spawned()
    {
        base.Spawned();
        _startTime = Time.time;
        _transform = GetComponent<NetworkTransform>();
        _startPosition = _transform.transform.position;
    }
    public void LifeTime()
    {
        if (_startTime + _lifetime < Time.time)
        {
            Runner.Despawn(Object);
        }
    }
    private void CheckRay()
    {
        Debug.Log("CheckRay()");
        if (attackHP == null)
        {
            Debug.Log("noHit");
            return;

        }
        RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, layerMask))
        Vector3 Dir = transform.position - _startPosition;
        Dir.Normalize();

        if (Physics.SphereCast(transform.position, radius, Dir, out hit, raycastDistance, layerMask))
        {

            HPHandler hitHP = hit.transform.GetComponent<HPHandler>();
            if (hitHP != null && hitHP != attackHP)
            {
                hitHP.enemyHPHandler = attackHP;
                Vector3 tmp = hit.transform.position - _startPosition;
                //tmp.y = 0;
                //other.GetComponent<Rigidbody>().AddForce(tmp.normalized * hitHP.AddForce, ForceMode.VelocityChange);
                if (attackHP._nickName == null)
                {
                    attackHP._nickName = "tmp";
                    return;
                }
                if ((int)Type < 0 || (int)Type > 3)
                {

                    return;
                }
                hitHP.OnTakeDamage(attackHP._nickName, (int)Type);
                hit.transform.GetComponent<CharacterMovementHandler>().HitAddForce(tmp, hitHP.AddForce);

                if (hitHP.isDead)
                {
                    //parentWeapon.KillEffect();
                }
                else
                {
                    //parentWeapon.HitEffect();
                }
                Runner.Despawn(Object);
            }
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (attackHP==null)
    //    {
    //        Debug.Log("attackHP Is NUll");
    //        return;
    //    }
    //    Debug.Log("OnTriggerEnter");
    //    HPHandler hitHP = other.GetComponent<HPHandler>();
    //    if (hitHP != null && hitHP != attackHP)
    //    {
    //        hitHP.enemyHPHandler = attackHP;
    //        Vector3 tmp = other.transform.position- _startPosition ;
    //        //tmp.y = 0;
    //        //other.GetComponent<Rigidbody>().AddForce(tmp.normalized * hitHP.AddForce, ForceMode.VelocityChange);
    //        if (attackHP._nickName == null)
    //        {
    //            Debug.Log("attackHP._nickName Is Null");
    //            attackHP._nickName = "tmp";
    //            return;
    //        }
    //        if((int)Type<0|| (int)Type > 3)
    //        {
    //            Debug.Log("Type Error");

    //            return;
    //        }
    //        hitHP.OnTakeDamage(attackHP._nickName, (int)Type);
    //        other.GetComponent<CharacterMovementHandler>().HitAddForce(tmp, hitHP.AddForce);

    //        if (hitHP.isDead)
    //        {
    //            //parentWeapon.KillEffect();
    //        }
    //        else
    //        {
    //            //parentWeapon.HitEffect();
    //        }
    //        Runner.Despawn(Object);
    //    }

    //}


    //private void Start()
    //{
    //    _startPosition = transform.position;

    //    if (_targetPosition == Vector3.zero)
    //    {
    //        _targetPosition = _startPosition + transform.forward * MaxDistance;
    //    }

    //    _duration = Vector3.Distance(_startPosition, _targetPosition) / Speed;
    //    _startTime = Time.timeSinceLevelLoad;
    //}

    //private void Update()
    //{
    //    float time = Time.timeSinceLevelLoad - _startTime;

    //    if (time < _duration)
    //    {

    //        transform.position = Vector3.Lerp(_startPosition, _targetPosition, time / _duration);
    //    }
    //    else
    //    {
    //        transform.position = _targetPosition;
    //        FinishProjectile();
    //    }
    //}

    //private void FinishProjectile()
    //{
    //    if (_showHitEffect == false)
    //    {
    //        //No hit effect, destroy immediately.
    //        Runner.Despawn(Object);
    //        return;
    //    }

    //    //Stop updating projectile visual.
    //    enabled = false;

    //    if (ProjectileObject != null)
    //    {
    //        ProjectileObject.SetActive(false);
    //    }

    //    if (HitEffectPrefab != null)
    //    {
    //        Instantiate(HitEffectPrefab, _targetPosition, Quaternion.LookRotation(_hitNormal), transform);
    //    }

    //    Runner.Despawn(Object);

    //}
}

