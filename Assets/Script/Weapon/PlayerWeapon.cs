using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerWeapon : NetworkBehaviour
{
    // �ѱ� ����
    public EWeaponType Type;

    [Header("Weapon UI")]
    public GameObject _weaponUI;    //��  ��������Ʈ �ް� ��ü�ϴ� �ɷ� �ٲ��� 
    //public Sprite _weaponSprite;    //���� �̹��� ��������Ʈ
    public TMP_Text _weaponAmmoText;//�Ѿ� ����
    public Image _weaponAimImage;   //ũ�ν����?
    public Image _killIcon;         //ų �̹���
    public int _weaponNum = 0;
    private HPHandler _hPHandler;

    [Header("Fire Setup")]
    public GameObject _localCameraRotation;

    public bool IsAutomatic = true; // �ڵ� �߻� ����
    public float Damage = 10f; // ������
    public int FireRate = 100; // �߻� �ӵ�
    [Range(1, 20)]
    public int ProjectilesPerShot = 1; // �� �߿� �߻�Ǵ�?������Ÿ�� ��
    public float Dispersion = 0f; // �л�
    public LayerMask HitMask; // ��Ʈ ����ũ
    public float MaxHitDistance = 100f; // �ִ� ��Ʈ �Ÿ�

    [Header("Ammo")]
    public int MaxClipAmmo = 12; // �ִ� ź�� ��
    public int StartAmmo = 25; // ���� ź�� ��
    public float ReloadTime = 2f; // ������ �ð�


    [Header("Fire Effect")]
    
    public Projectile ProjectilePrefab; // ������Ÿ�� �ð�ȭ ������

    public bool HasAmmo => ClipAmmo > 0 || RemainingAmmo > 0; // ź���� �ִ��� ����

    [Networked]
    public NetworkBool IsCollected { get; set; }  // ȹ�� ����
    [Networked]
    public NetworkBool IsReloading { get; set; } // ������ ����
    [Networked]
    public int ClipAmmo { get; set; } // ���� źâ�� ź�� ��
    [Networked]
    public int RemainingAmmo { get; set; } // ���� ź�� ��

    [Networked]
    private int _fireCount { get; set; } // �߻� Ƚ��
    [Networked]
    private TickTimer _fireCooldown { get; set; } // �߻� ��ٿ�?Ÿ�̸�
    [Networked, Capacity(32)]
    private NetworkArray<ProjectileData> _projectileData { get; } // ������Ÿ�� ������ �迭

    private int _fireTicks; // �߻� ƽ
    private int _visibleFireCount; // �ð�ȭ�� �߻� Ƚ��
    private bool _reloadingVisible; // ������ �ð�ȭ ����
    private GameObject _muzzleEffectInstance; // �߻� ȿ�� �ν��Ͻ�

    //private SceneObjects _sceneObjects; // SceneObjects Ŭ����
    public bool AbleFire(bool justPressed) 
    {
        //IsCollected == false ||
        if ( (justPressed == false && !IsAutomatic) || IsReloading || !_fireCooldown.ExpiredOrNotRunning(Runner))
            return false;

        if (ClipAmmo <= 0)
        {
            return false;
        }
        return true;
    }
    public void Fire(Vector3 firePosition, Vector3 fireDirection)
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

    public void Equip()
    {
        _weaponUI.SetActive(true);
        AmmoInfoUpdate();
    }

    public void DisEquip()
    {
        _weaponUI.SetActive(false);
    }
    public void AmmoInfoUpdate()
    {
        _weaponAmmoText.text = $"   {ClipAmmo}  /  {RemainingAmmo}";
    }
    // ������ �޼ҵ�
    public void Reload()
    {
        //IsCollected == false ||
        if ( ClipAmmo >= MaxClipAmmo || RemainingAmmo <= 0 || IsReloading || !_fireCooldown.ExpiredOrNotRunning(Runner))
            return;

        IsReloading = true;
        _fireCooldown = TickTimer.CreateFromSeconds(Runner, ReloadTime);
    }

    public void AmmoInfo(TMP_Text _text)
    {
        _text.text = $"{ClipAmmo} / {MaxClipAmmo}";
    }

    // ź�� �߰� �޼ҵ�
    public void AddAmmo(int amount)
    {
        RemainingAmmo += amount;
    }


    // ������ ���� ���¸� �������� �޼ҵ�
    public float GetReloadProgress()
    {
        // ������ ���� �ƴϸ� 1�� ��ȯ
        if (!IsReloading)
            return 1f;

        // ������ ���� ���?���� ������ �ð��� ���� ���� ��ȯ
        return 1f - _fireCooldown.RemainingTime(Runner).GetValueOrDefault() / ReloadTime;
    }

    // ������Ʈ�� ������ �� ȣ��Ǵ�?�޼ҵ�
    public override void Spawned()
    {
        base.Spawned();
        // ���� ������ �ִ� ��쿡��?�ʱ�ȭ �ڵ� ����

        if (HasStateAuthority)
        {
            ClipAmmo = Mathf.Clamp(StartAmmo, 0, MaxClipAmmo);
            RemainingAmmo = StartAmmo - ClipAmmo;
        }

        _visibleFireCount = _fireCount;

        float fireTime = 60f / FireRate;
        _fireTicks = Mathf.CeilToInt(fireTime / Runner.DeltaTime);

        if (HasStateAuthority)
        {
            IsCollected = true;
        }
        _hPHandler = GetComponentInParent<HPHandler>();
        if(_hPHandler == null)
        {
            Debug.Log("_hPHandler Is Null");
        }
        
    }
    public void OnRespawn()
    {
        RemainingAmmo = StartAmmo - MaxClipAmmo;
        ClipAmmo = MaxClipAmmo;
        AmmoInfoUpdate();
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

    // ������ �޼ҵ�
    public override void Render()
    {
        
        _visibleFireCount = _fireCount;

        if (_reloadingVisible != IsReloading)
        {
            //Animator.SetBool("IsReloading", IsReloading);

            if (IsReloading)
            {
                // ReloadingSound.Play();
            }

            _reloadingVisible = IsReloading;
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
            Projectile QQ = Runner.Spawn(ProjectilePrefab, firePosition , Quaternion.identity);//+ fireDirection * 2.5f
            QQ.SetTarget(firePosition + fireDirection * MaxHitDistance, this, _hPHandler);

        }
        
        
        _projectileData.Set(_fireCount % _projectileData.Length, projectileData);
        _fireCount++;
    }

    //public void HitEffect()
    //{
    //    if (HasInputAuthority)
    //        StartCoroutine(EnemyHitEffect());
    //}

    //public void KillEffect()
    //{
    //    if (HasInputAuthority)
    //        StartCoroutine(EnemyKillEffect());
    //}

    //IEnumerator EnemyHitEffect()
    //{
    //    int i = 0;
    //    int goal = 20;
    //    while (i < goal)
    //    {
    //        _weaponAimImage.color = Color.Lerp(_weaponAimImage.color, Color.red, 0.3f);
    //        ++i;
    //        yield return null;
    //    }
    //    i = 0;
    //    while (i < goal)
    //    {
    //        _weaponAimImage.color = Color.Lerp(_weaponAimImage.color, Color.white, 0.3f);
    //        ++i;
    //        yield return null;
    //    }
    //    _weaponAimImage.color = Color.white;

    //    yield return null;
    //}
    //IEnumerator EnemyKillEffect()
    //{
    //    Color mirror = new Color(1, 1, 1, 0);
    //    int i = 0;
    //    int goal = 100;
    //    _weaponAimImage.color = mirror;
    //    while (i < goal)
    //    {
    //        _killIcon.color = Color.Lerp(_killIcon.color, Color.white, 0.25f);
    //        ++i;
    //        yield return null;
    //    }
    //    i = 0;
    //    while (i < goal / 4)
    //    {
    //        _killIcon.color = Color.Lerp(_killIcon.color, mirror, 0.5f);
    //        ++i;
    //        yield return null;
    //    }
    //    _weaponAimImage.color = Color.white;
    //    _killIcon.color = mirror;

    //    yield return null;
    //}

    

    //private void PlayFireEffect()
    //{
    //    if (FireSound != null)
    //    {
    //        // �߻� ���带 �� �� ����մϴ�?
    //        FireSound.PlayOneShot(FireSound.clip);
    //    }

    //    // �ѱ� ����Ʈ ���ü��� �缳���մϴ�.
    //    _muzzleEffectInstance.SetActive(false);
    //    _muzzleEffectInstance.SetActive(true);

    //    // �߻� �ִϸ��̼��� ����մϴ�?
    //    Animator.SetTrigger("Fire");

    //    // �θ� Player�� �߻� ����Ʈ�� ����մϴ�?
    //    GetComponentInParent<CharacterMovementHandler>().PlayFireEffect();
    //}

    private struct ProjectileData : INetworkStruct
    {
        public Vector3 HitPosition;
        public Vector3 HitNormal;
        public NetworkBool ShowHitEffect;
    }
    
    
}

