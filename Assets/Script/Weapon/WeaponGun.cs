using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponGun : PlayerWeapon
{

    [Header("Weapon UI")]
    public TMP_Text _weaponAmmoText;//�Ѿ� ����
    [Range(1, 20)]
    public int ProjectilesPerShot = 1; // �� �߿� �߻�Ǵ�?������Ÿ�� ��
    public float Dispersion = 0f; // �л�
    public float MaxHitDistance = 100f; // �ִ� ��Ʈ �Ÿ�
    [Header("Ammo")]
    public int MaxClipAmmo = 12; // �ִ� ź�� ��
    public int StartAmmo = 25; // ���� ź�� ��
    public float ReloadTime = 2f; // ������ �ð�
    [Header("Fire Effect")]

    public Projectile ProjectilePrefab; // ������Ÿ�� �ð�ȭ ������

    public bool HasAmmo => ClipAmmo > 0 || RemainingAmmo > 0; // ź���� �ִ��� ����
    [Networked]
    public NetworkBool IsReloading { get; set; } // ������ ����
    [Networked]
    public int ClipAmmo { get; set; } // ���� źâ�� ź�� ��
    [Networked]
    public int RemainingAmmo { get; set; } // ���� ź�� ��

    private int _fireTicks; // �߻� ƽ
    private int _visibleFireCount; // �ð�ȭ�� �߻� Ƚ��
    private bool _reloadingVisible; // ������ �ð�ȭ ����

    [Networked]
    private int _fireCount { get; set; } // �߻� Ƚ��
    [Networked]
    private TickTimer _fireCooldown { get; set; } // �߻� ��ٿ�?Ÿ�̸�
    [Networked, Capacity(32)]
    private NetworkArray<ProjectileData> _projectileData { get; } // ������Ÿ�� ������ �迭
    // Start is called before the first frame update
    public override void Equip()
    {
        base.Equip();
        AmmoInfoUpdate();

    }
    public override bool AbleFire()
    {
        //IsCollected == false ||
        if ( !IsAutomatic || IsReloading || !_fireCooldown.ExpiredOrNotRunning(Runner))
            return false;

        if (ClipAmmo <= 0)
        {
            return false;
        }
        return true;
    }
    public override void Fire(Vector3 firePosition, Vector3 fireDirection)
    {
        for (int i = 0; i < ProjectilesPerShot; i++)
        {
            var projectileDirection = fireDirection;

            if (Dispersion > 0f)
            {
                var dispersionRotation = Quaternion.Euler(Random.insideUnitSphere * Dispersion);
                projectileDirection = dispersionRotation * fireDirection;
            }

            //FireProjectile(firePosition, projectileDirection);
            FireProjectile(firePosition, fireDirection);

        }

        _fireCooldown = TickTimer.CreateFromTicks(Runner, _fireTicks);
        ClipAmmo--;
        AmmoInfoUpdate();
        Debug.Log($"ClipAmmo = {ClipAmmo}");
    }
    public override void DisEquip()
    {
        base.Equip();
    }
    public override void FixedUpdateNetwork()
    {
        //if (IsCollected == false)
        //    return;

        if (ClipAmmo == 0)
            Reload();

        if (IsReloading && _fireCooldown.ExpiredOrNotRunning(Runner))
        {
            IsReloading = false;

            int reloadAmmo = MaxClipAmmo - ClipAmmo;
            reloadAmmo = Mathf.Min(reloadAmmo, RemainingAmmo);

            ClipAmmo += reloadAmmo;
            RemainingAmmo -= reloadAmmo;
            AmmoInfoUpdate();

            _fireCooldown = TickTimer.CreateFromSeconds(Runner, 0.25f);
        }
    }
    private void FireProjectile(Vector3 firePosition, Vector3 fireDirection)
    {

        var projectileData = new ProjectileData();

        var hitOptions = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority;

        Debug.DrawRay(firePosition + fireDirection * 2.5f, fireDirection * MaxHitDistance, Color.green, 1);

        if (HasStateAuthority)
        {
            Quaternion cmRotation = Quaternion.FromToRotation(firePosition, fireDirection);
            Projectile QQ = Runner.Spawn(ProjectilePrefab, firePosition, Quaternion.identity);//+ fireDirection * 2.5f
            QQ.SetTarget(firePosition + fireDirection * MaxHitDistance, this, _hPHandler);

        }


        _projectileData.Set(_fireCount % _projectileData.Length, projectileData);
        _fireCount++;
    }
    private struct ProjectileData : INetworkStruct
    {
        public Vector3 HitPosition;
        public Vector3 HitNormal;
        public NetworkBool ShowHitEffect;
    }
    public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority)
        {
            ClipAmmo = Mathf.Clamp(StartAmmo, 0, MaxClipAmmo);
            RemainingAmmo = StartAmmo - ClipAmmo;
        }

        _visibleFireCount = _fireCount;

        float fireTime = 60f / FireRate;
        _fireTicks = Mathf.CeilToInt(fireTime / Runner.DeltaTime);
    }
    
    public override void OnRespawn()
    {
        RemainingAmmo = StartAmmo - MaxClipAmmo;
        ClipAmmo = MaxClipAmmo;
        AmmoInfoUpdate();
    }
    public void AmmoInfoUpdate()
    {
        _weaponAmmoText.text = $"   {ClipAmmo}  /  {RemainingAmmo}";
    }
    public void Reload()
    {
        //IsCollected == false ||
        if (ClipAmmo >= MaxClipAmmo || RemainingAmmo <= 0 || IsReloading || !_fireCooldown.ExpiredOrNotRunning(Runner))
            return;

        IsReloading = true;
        _fireCooldown = TickTimer.CreateFromSeconds(Runner, ReloadTime);
    }

    public void AmmoInfo(TMP_Text _text)
    {
        _text.text = $"{ClipAmmo} / {MaxClipAmmo}";
    }

    public void AddAmmo(int amount)
    {
        RemainingAmmo += amount;
    }


    public float GetReloadProgress()
    {
        if (!IsReloading)
            return 1f;

        return 1f - _fireCooldown.RemainingTime(Runner).GetValueOrDefault() / ReloadTime;
    }
}
