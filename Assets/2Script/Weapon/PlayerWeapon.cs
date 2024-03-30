using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public enum EWeaponType
{
    Pistol,
    Sword,
    Gravity
}
public enum EAttackType
{
    Nomal,
    Knockback,
}

public class PlayerWeapon : NetworkBehaviour
{
    // �ѱ� ����
    public EAttackType AttackType;
    public EWeaponType Type;
    //public int maxAttackCount { get; protected set; } = 2;

    [Header("Weapon UI")]
    //public GameObject _weaponUI;    //��  ��������Ʈ �ް� ��ü�ϴ� �ɷ� �ٲ��� 
    //public Sprite _weaponSprite;    //���� �̹��� ��������Ʈ
    //public Image _killIcon;         //ų �̹���
    public int _weaponNum;
    protected HPHandler _hPHandler;

    [Header("Fire Setup")]
    public bool IsAutomatic = true; // �ڵ� �߻� ����
    public float Damage = 10f; // ������
    public int FireRate = 100; // �߻� �ӵ�
    public LayerMask HitMask; // ��Ʈ ����ũ
    [Networked]
    public NetworkBool IsCollected { get; set; }  // ȹ�� ����

    //private GameObject _muzzleEffectInstance; // �߻� ȿ�� �ν��Ͻ�

    //private SceneObjects _sceneObjects; // SceneObjects Ŭ����
    public virtual bool AbleFire() 
    {
        return true;
    }
    public virtual void Fire(Vector3 firePosition, Vector3 fireDirection)
    {
        ;
    }
    public virtual void Equip()
    {
        //_weaponUI.SetActive(true);
    }
    public virtual void DisEquip()
    {
        //_weaponUI.SetActive(false);
    }
    public override void Spawned()
    {
        base.Spawned();
        AttackType = EAttackType.Nomal;
        if (HasStateAuthority)
        {
            IsCollected = true;
        }
        _hPHandler = GetComponentInParent<HPHandler>();
        if(_hPHandler == null)
        {
            Debug.Log("_hPHandler Is Null");
        }
        _weaponNum = (int)Type;
    }
    public void SetAttackType(EAttackType eAttackType)
    {
        AttackType = eAttackType;
    }
    public virtual void SetCollistion(bool _tf)
    {
        ;
    }

    public virtual void OnRespawn()
    {
        ;
    }
    public override void FixedUpdateNetwork()
    {
        ;
    }
}

