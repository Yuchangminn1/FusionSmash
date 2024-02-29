using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public enum EWeaponType
{
    Pistol,
    Rifle,
    Shotgun,
    Gravity
}
public class PlayerWeapon : NetworkBehaviour
{
    // �ѱ� ����
    public EWeaponType Type;

    [Header("Weapon UI")]
    public GameObject _weaponUI;    //��  ��������Ʈ �ް� ��ü�ϴ� �ɷ� �ٲ��� 
    //public Sprite _weaponSprite;    //���� �̹��� ��������Ʈ
    public TMP_Text _weaponAmmoText;//�Ѿ� ����
    public Image _weaponAimImage;   //ũ�ν����
    public Image _killIcon;         //ų �̹���
    public int _weaponNum = 0;


    [Header("Fire Setup")]
    public GameObject _localCameraRotation;

    public bool IsAutomatic = true; // �ڵ� �߻� ����
    public float Damage = 10f; // ������
    public int FireRate = 100; // �߻� �ӵ�
    [Range(1, 20)]
    public int ProjectilesPerShot = 1; // �� �߿� �߻�Ǵ� ������Ÿ�� ��
    public float Dispersion = 0f; // �л�
    public LayerMask HitMask; // ��Ʈ ����ũ
    public float MaxHitDistance = 100f; // �ִ� ��Ʈ �Ÿ�

    [Header("Ammo")]
    public int MaxClipAmmo = 12; // �ִ� ź�� ��
    public int StartAmmo = 25; // ���� ź�� ��
    public float ReloadTime = 2f; // ������ �ð�

    //[Header("Visuals")]
    //public Sprite Icon; // ������
    //public string Name; // �̸�
    //public Animator Animator; // �ִϸ�����

    [Header("Fire Effect")]
    //public Transform MuzzleTransform; // 3��Ī �������� �߻�Ǵ� ��ġ
    //public GameObject MuzzleEffectPrefab; // �߻� ȿ�� ������
    public Projectile ProjectilePrefab; // ������Ÿ�� �ð�ȭ ������

    //[Header("Sounds")]
    //public AudioSource FireSound; // �߻� ����
    //public AudioSource ReloadingSound; // ������ ����
    //public AudioSource EmptyClipSound; // �� źâ ����

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
    private TickTimer _fireCooldown { get; set; } // �߻� ��ٿ� Ÿ�̸�
    [Networked, Capacity(32)]
    private NetworkArray<ProjectileData> _projectileData { get; } // ������Ÿ�� ������ �迭

    private int _fireTicks; // �߻� ƽ
    private int _visibleFireCount; // �ð�ȭ�� �߻� Ƚ��
    private bool _reloadingVisible; // ������ �ð�ȭ ����
    private GameObject _muzzleEffectInstance; // �߻� ȿ�� �ν��Ͻ�

    //private SceneObjects _sceneObjects; // SceneObjects Ŭ����

    // ���� �߻��ϴ� �޼ҵ�
    public void Fire(Vector3 firePosition, Vector3 fireDirection, bool justPressed)
    {

        // ���� ȹ����� �ʾҰų� �ڵ� �߻簡 �ƴϰų� ������ ���̰ų� �߻� ��ٿ��� ���������� ����
        if (IsCollected == false || (justPressed == false && !IsAutomatic) || IsReloading || !_fireCooldown.ExpiredOrNotRunning(Runner))
            return;

        //źâ�� ź���� ���� ��� �� źâ ���� ��� �� ����
        if (ClipAmmo <= 0)
        {
            //PlayEmptyClipSound(justPressed);
            return;
        }

        // ����ü �߻�
        for (int i = 0; i < ProjectilesPerShot; i++)
        {
            var projectileDirection = fireDirection;

            if (Dispersion > 0f)
            {
                // �л��� ����� �߻� ���� ���
                var dispersionRotation = Quaternion.Euler(Random.insideUnitSphere * Dispersion);
                projectileDirection = dispersionRotation * fireDirection;
            }

            //FireProjectile(firePosition, projectileDirection);
            FireProjectile(firePosition, fireDirection);

        }

        // �߻� ��ٿ� ���� �� ź�� ����
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
        // ���� ȹ����� �ʾҰų� źâ�� ���� á�ų� ���� ź���� ���ų� ������ ���̰ų� �߻� ��ٿ��� ���������� ����
        if (IsCollected == false || ClipAmmo >= MaxClipAmmo || RemainingAmmo <= 0 || IsReloading || !_fireCooldown.ExpiredOrNotRunning(Runner))
            return;

        // ������ ������ �����ϰ� ������ ��ٿ� ����
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

        // ������ ���� ��� ���� ������ �ð��� ���� ���� ��ȯ
        return 1f - _fireCooldown.RemainingTime(Runner).GetValueOrDefault() / ReloadTime;
    }

    // ������Ʈ�� ������ �� ȣ��Ǵ� �޼ҵ�
    public override void Spawned()
    {
        // ���� ������ �ִ� ��쿡�� �ʱ�ȭ �ڵ� ����

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

        // �߻� ȿ�� �ν��Ͻ� ���� �� ��Ȱ��ȭ
        // _muzzleEffectInstance = Instantiate(MuzzleEffectPrefab, MuzzleTransform);
        // _muzzleEffectInstance.SetActive(false);

        // SceneObjects Ŭ���� ����
        //_sceneObjects = Runner.GetSingleton<SceneObjects>();
    }

    // ��Ʈ��ũ ������Ʈ �޼ҵ�
    public override void FixedUpdateNetwork()
    {
        // ���� ȹ����� �ʾҴٸ� ����
        if (IsCollected == false)
            return;

        // źâ�� ��������� �ڵ����� ������ �õ�
        if (ClipAmmo == 0)
            Reload();

        // ������ ���̸� ������ ��ٿ��� ������ ��
        if (IsReloading && _fireCooldown.ExpiredOrNotRunning(Runner))
        {
            // ������ �Ϸ�
            IsReloading = false;

            // �������� �� �ִ� �ִ� ź�� �� ���
            int reloadAmmo = MaxClipAmmo - ClipAmmo;
            reloadAmmo = Mathf.Min(reloadAmmo, RemainingAmmo);

            // ź�� �߰� �� ���� ź�� ����
            ClipAmmo += reloadAmmo;
            RemainingAmmo -= reloadAmmo;
            AmmoInfoUpdate();

            // ������ �� �غ� �ð� �߰�
            _fireCooldown = TickTimer.CreateFromSeconds(Runner, 0.25f);
        }
    }

    // ������ �޼ҵ�
    public override void Render()
    {
        // �߻� Ƚ���� ����Ǿ����� �߻� ȿ�� ���
        //if (_visibleFireCount < _fireCount)
        //{
        //    PlayFireEffect();
        //}

        // ���� ǥ�õ��� ���� ��� ������Ÿ�Ͽ� ���� �ð����� ó��
        //for (int i = _visibleFireCount; i < _fireCount; i++)
        //{
        //    var data = _projectileData[i % _projectileData.Length];
        //    var muzzleTransform = MuzzleTransform;

        //    var projectileVisual = Instantiate(ProjectilePrefab, muzzleTransform.position, muzzleTransform.rotation);
        //    projectileVisual.SetHit(data.HitPosition, data.HitNormal, data.ShowHitEffect);
        //}

        _visibleFireCount = _fireCount;

        if (_reloadingVisible != IsReloading)
        {
            // ���ο� ������ ���¿� ���� �ִϸ��̼� �� �Ҹ��� ó���մϴ�.
            //Animator.SetBool("IsReloading", IsReloading);

            if (IsReloading)
            {
                // ������ �߿��� ���ε� ���带 ����մϴ�.
                // ReloadingSound.Play();
            }

            _reloadingVisible = IsReloading;
        }
    }
   
    private void FireProjectile(Vector3 firePosition, Vector3 fireDirection)
    {

        var projectileData = new ProjectileData();

        var hitOptions = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority;

        // ��ü �߻�ü ��� �� ȿ���� ��� ó���մϴ�(��Ʈ��ĵ �߻�ü).
        //Runner.LagCompensation.Raycast(aimPoint.position + aimForwardVector * 2.5f, aimForwardVector,
        //hitDistance, Object.InputAuthority, out var hitnfo, collisionLayer, HitOptions.IncludePhysX);
        Debug.DrawRay(firePosition + fireDirection * 2.5f, fireDirection * MaxHitDistance, Color.green, 1);
        
        
        if (Runner.LagCompensation.Raycast(firePosition, fireDirection, MaxHitDistance,
                Object.InputAuthority, out var hit, HitMask, hitOptions))
        {
            projectileData.HitPosition = hit.Point;
            projectileData.HitNormal = hit.Normal;

            if (hit.Hitbox != null)
            {
                HPHandler tmpHP = hit.Hitbox.transform.root.GetComponent<HPHandler>();

                if (tmpHP.isDead)
                {
                    return;
                }
                HPHandler KN = gameObject.GetComponentInParent<HPHandler>();
                tmpHP.enemyHPHandler = KN;

                tmpHP.OnTakeDamage(KN._nickName, ((int)Type));
                if (tmpHP.isDead)
                {
                    if (KN.HasStateAuthority)
                    {
                        
                    }

                    if (HasInputAuthority)
                        StartCoroutine(KillEffect());
                }
                else
                {
                    if (HasInputAuthority)
                        StartCoroutine(EnemyHitEffect());
                }

                // ��Ʈ�ڽ��� �ִ� ��� �������� �����մϴ�.
                ApplyDamage(hit.Hitbox, hit.Point, fireDirection);
            }
            else
            {
                Debug.Log("����");

                // �÷��̾ �ܴ��� ��ü�� �浹���� ���� ��Ʈ ȿ���� ǥ���մϴ�.
                projectileData.ShowHitEffect = true;
            }
        }
        
        Instantiate(ProjectilePrefab, firePosition + fireDirection * 2.5f, _localCameraRotation.transform.rotation);

        _projectileData.Set(_fireCount % _projectileData.Length, projectileData);
        _fireCount++;
        Debug.Log($"�߻�Ƚ�� {_fireCount}");
    }
    

    
    IEnumerator EnemyHitEffect()
    {
        int i = 0;
        int goal = 20;
        while (i < goal)
        {
            _weaponAimImage.color = Color.Lerp(_weaponAimImage.color, Color.red, 0.3f);
            ++i;
            yield return null;
        }
        i = 0;
        while (i < goal)
        {
            _weaponAimImage.color = Color.Lerp(_weaponAimImage.color, Color.white, 0.3f);
            ++i;
            yield return null;
        }
        _weaponAimImage.color = Color.white;

        yield return null;
    }
    IEnumerator KillEffect()
    {
        Color mirror = new Color(1, 1, 1, 0);
        int i = 0;
        int goal = 100;
        _weaponAimImage.color = mirror;
        while (i < goal)
        {
            _killIcon.color = Color.Lerp(_killIcon.color, Color.white, 0.25f);
            ++i;
            yield return null;
        }
        i = 0;
        while (i < goal / 4)
        {
            _killIcon.color = Color.Lerp(_killIcon.color, mirror, 0.5f);
            ++i;
            yield return null;
        }
        _weaponAimImage.color = Color.white;
        _killIcon.color = mirror;

        yield return null;
    }

    public void OnRespawn()
    {
        RemainingAmmo = StartAmmo- MaxClipAmmo;
        ClipAmmo = MaxClipAmmo;
        AmmoInfoUpdate();
    }

    //private void PlayFireEffect()
    //{
    //    if (FireSound != null)
    //    {
    //        // �߻� ���带 �� �� ����մϴ�.
    //        FireSound.PlayOneShot(FireSound.clip);
    //    }

    //    // �ѱ� ����Ʈ ���ü��� �缳���մϴ�.
    //    _muzzleEffectInstance.SetActive(false);
    //    _muzzleEffectInstance.SetActive(true);

    //    // �߻� �ִϸ��̼��� ����մϴ�.
    //    Animator.SetTrigger("Fire");

    //    // �θ� Player�� �߻� ����Ʈ�� ����մϴ�.
    //    GetComponentInParent<CharacterMovementHandler>().PlayFireEffect();
    //}

    private void ApplyDamage(Hitbox enemyHitbox, Vector3 position, Vector3 direction)
    {
        //var enemyHealth = enemyHitbox.Root.GetComponent<Health>();
        //if (enemyHealth == null || enemyHealth.IsAlive == false)
        //    return;

        //float damageMultiplier = enemyHitbox is BodyHitbox bodyHitbox ? bodyHitbox.DamageMultiplier : 1f;
        //bool isCriticalHit = damageMultiplier > 1f;

        //float damage = Damage * damageMultiplier;
        //if (_sceneObjects.Gameplay.DoubleDamageActive)
        //{
        //    // ���� ������ Ȱ��ȭ ���̸� �������� �� ��� ������ŵ�ϴ�.
        //    damage *= 2f;
        //}

        //if (enemyHealth.ApplyDamage(Object.InputAuthority, damage, position, direction, Type, isCriticalHit) == false)
        //    return;

        //if (HasInputAuthority && Runner.IsForward)
        //{
        //    // ���� �÷��̾�� UI ��Ʈ ����Ʈ�� ǥ���մϴ�.
        //   // _sceneObjects.GameUI.PlayerView.Crosshair.ShowHit(enemyHealth.IsAlive == false, isCriticalHit);
        //}
    }

    //private void PlayEmptyClipSound(bool fireJustPressed)
    //{
    //    // �ڵ� ������ ��� ������ �߻� �Ŀ� �� źâ ���带 �� �� ����Ϸ��� �մϴ�.
    //    bool firstEmptyShot = _fireCooldown.TargetTick.GetValueOrDefault() == Runner.Tick - 1;

    //    if (fireJustPressed == false && firstEmptyShot == false)
    //        return;

    //    if (EmptyClipSound == null || EmptyClipSound.isPlaying)
    //        return;

    //    if (Runner.IsForward && HasInputAuthority)
    //    {
    //        // �� źâ ���带 ����մϴ�.
    //        EmptyClipSound.Play();
    //    }
    //}

    /// <summary>
    /// ���� �߻�ü �߻縦 ��Ÿ���� ����ü�Դϴ�.
    /// </summary>
    private struct ProjectileData : INetworkStruct
    {
        public Vector3 HitPosition;
        public Vector3 HitNormal;
        public NetworkBool ShowHitEffect;
    }
}

