using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;





public enum EWeaponType
{
    None,
    Pistol,
    Rifle,
    Shotgun,
}
public class PlayerWeapon : NetworkBehaviour
{
    // 총기 유형
    public EWeaponType Type;

    [Header("Fire Setup")]
    public bool IsAutomatic = true; // 자동 발사 여부
    public float Damage = 10f; // 데미지
    public int FireRate = 100; // 발사 속도
    [Range(1, 20)]
    public int ProjectilesPerShot = 1; // 한 발에 발사되는 프로젝타일 수
    public float Dispersion = 0f; // 분산
    public LayerMask HitMask; // 히트 마스크
    public float MaxHitDistance = 100f; // 최대 히트 거리

    [Header("Ammo")]
    public int MaxClipAmmo = 12; // 최대 탄약 수
    public int StartAmmo = 25; // 시작 탄약 수
    public float ReloadTime = 2f; // 재장전 시간

    //[Header("Visuals")]
    //public Sprite Icon; // 아이콘
    //public string Name; // 이름
    //public Animator Animator; // 애니메이터

    [Header("Fire Effect")]
    //public Transform MuzzleTransform; // 3인칭 시점에서 발사되는 위치
    //public GameObject MuzzleEffectPrefab; // 발사 효과 프리팹
    public Projectile ProjectilePrefab; // 프로젝타일 시각화 프리팹

    //[Header("Sounds")]
    //public AudioSource FireSound; // 발사 사운드
    //public AudioSource ReloadingSound; // 재장전 사운드
    //public AudioSource EmptyClipSound; // 빈 탄창 사운드

    public bool HasAmmo => ClipAmmo > 0 || RemainingAmmo > 0; // 탄약이 있는지 여부

    [Networked]
    public NetworkBool IsCollected { get; set; } // 획득 여부
    [Networked]
    public NetworkBool IsReloading { get; set; } // 재장전 여부
    [Networked]
    public int ClipAmmo { get; set; } // 현재 탄창의 탄약 수
    [Networked]
    public int RemainingAmmo { get; set; } // 남은 탄약 수

    [Networked]
    private int _fireCount { get; set; } // 발사 횟수
    [Networked]
    private TickTimer _fireCooldown { get; set; } // 발사 쿨다운 타이머
    [Networked, Capacity(32)]
    private NetworkArray<ProjectileData> _projectileData { get; } // 프로젝타일 데이터 배열

    private int _fireTicks; // 발사 틱
    private int _visibleFireCount; // 시각화된 발사 횟수
    private bool _reloadingVisible; // 재장전 시각화 여부
    private GameObject _muzzleEffectInstance; // 발사 효과 인스턴스

    //private SceneObjects _sceneObjects; // SceneObjects 클래스

    // 총을 발사하는 메소드
    public void Fire(Vector3 firePosition, Vector3 fireDirection, bool justPressed)
    {
        // 총이 획득되지 않았거나 자동 발사가 아니거나 재장전 중이거나 발사 쿨다운이 남아있으면 무시
        if (IsCollected == false || (justPressed == false && !IsAutomatic) || IsReloading || !_fireCooldown.ExpiredOrNotRunning(Runner))
            return;

        // 탄창에 탄약이 없을 경우 빈 탄창 사운드 재생 후 리턴
        //if (ClipAmmo <= 0)
        //{
        //    PlayEmptyClipSound(justPressed);
        //    return;
        //}

        // 투사체 발사
        for (int i = 0; i < ProjectilesPerShot; i++)
        {
            var projectileDirection = fireDirection;

            if (Dispersion > 0f)
            {
                // 분산이 적용된 발사 방향 계산
                var dispersionRotation = Quaternion.Euler(Random.insideUnitSphere * Dispersion);
                projectileDirection = dispersionRotation * fireDirection;
            }

            FireProjectile(firePosition, projectileDirection);
        }

        // 발사 쿨다운 설정 및 탄약 감소
        _fireCooldown = TickTimer.CreateFromTicks(Runner, _fireTicks);
        ClipAmmo--;
    }

    // 재장전 메소드
    public void Reload()
    {
        // 총이 획득되지 않았거나 탄창이 가득 찼거나 남은 탄약이 없거나 재장전 중이거나 발사 쿨다운이 남아있으면 무시
        if (IsCollected == false || ClipAmmo >= MaxClipAmmo || RemainingAmmo <= 0 || IsReloading || !_fireCooldown.ExpiredOrNotRunning(Runner))
            return;

        // 재장전 중으로 설정하고 재장전 쿨다운 시작
        IsReloading = true;
        _fireCooldown = TickTimer.CreateFromSeconds(Runner, ReloadTime);
    }

    // 탄약 추가 메소드
    public void AddAmmo(int amount)
    {
        RemainingAmmo += amount;
    }


    // 재장전 진행 상태를 가져오는 메소드
    public float GetReloadProgress()
    {
        // 재장전 중이 아니면 1을 반환
        if (!IsReloading)
            return 1f;

        // 재장전 중인 경우 남은 재장전 시간의 진행 상태 반환
        return 1f - _fireCooldown.RemainingTime(Runner).GetValueOrDefault() / ReloadTime;
    }

    // 오브젝트가 스폰될 때 호출되는 메소드
    public override void Spawned()
    {
        // 상태 권한이 있는 경우에만 초기화 코드 실행
        if (HasStateAuthority)
        {
            ClipAmmo = Mathf.Clamp(StartAmmo, 0, MaxClipAmmo);
            RemainingAmmo = StartAmmo - ClipAmmo;
        }
        
        _visibleFireCount = _fireCount;

        float fireTime = 60f / FireRate;
        _fireTicks = Mathf.CeilToInt(fireTime / Runner.DeltaTime);

        // 발사 효과 인스턴스 생성 및 비활성화
       // _muzzleEffectInstance = Instantiate(MuzzleEffectPrefab, MuzzleTransform);
        _muzzleEffectInstance.SetActive(false);

        // SceneObjects 클래스 참조
        //_sceneObjects = Runner.GetSingleton<SceneObjects>();
    }

    // 네트워크 업데이트 메소드
    public override void FixedUpdateNetwork()
    {
        // 총이 획득되지 않았다면 무시
        if (IsCollected == false)
            return;

        // 탄창이 비어있으면 자동으로 재장전 시도
        if (ClipAmmo == 0)
            Reload();

        // 재장전 중이며 재장전 쿨다운이 끝났을 때
        if (IsReloading && _fireCooldown.ExpiredOrNotRunning(Runner))
        {
            // 재장전 완료
            IsReloading = false;

            // 재장전할 수 있는 최대 탄약 수 계산
            int reloadAmmo = MaxClipAmmo - ClipAmmo;
            reloadAmmo = Mathf.Min(reloadAmmo, RemainingAmmo);

            // 탄약 추가 및 남은 탄약 감소
            ClipAmmo += reloadAmmo;
            RemainingAmmo -= reloadAmmo;

            // 재장전 후 준비 시간 추가
            _fireCooldown = TickTimer.CreateFromSeconds(Runner, 0.25f);
        }
    }

    // 렌더링 메소드
    public override void Render()
    {
        // 발사 횟수가 변경되었으면 발사 효과 재생
        //if (_visibleFireCount < _fireCount)
        //{
        //    PlayFireEffect();
        //}

        // 아직 표시되지 않은 모든 프로젝타일에 대한 시각적인 처리
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
            // 새로운 재장전 상태에 따라 애니메이션 및 소리를 처리합니다.
            //Animator.SetBool("IsReloading", IsReloading);

            if (IsReloading)
            {
                // 재장전 중에는 리로딩 사운드를 재생합니다.
               // ReloadingSound.Play();
            }

            _reloadingVisible = IsReloading;
        }
    }

    private void FireProjectile(Vector3 firePosition, Vector3 fireDirection)
    {
        var projectileData = new ProjectileData();

        var hitOptions = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority;

        // 전체 발사체 경로 및 효과를 즉시 처리합니다(히트스캔 발사체).
        if (Runner.LagCompensation.Raycast(firePosition, fireDirection, MaxHitDistance,
                Object.InputAuthority, out var hit, HitMask, hitOptions))
        {
            projectileData.HitPosition = hit.Point;
            projectileData.HitNormal = hit.Normal;

            if (hit.Hitbox != null)
            {
                // 히트박스가 있는 경우 데미지를 적용합니다.
                ApplyDamage(hit.Hitbox, hit.Point, fireDirection);
            }
            else
            {
                // 플레이어가 단단한 물체에 충돌했을 때만 히트 효과를 표시합니다.
                projectileData.ShowHitEffect = true;
            }
        }

        _projectileData.Set(_fireCount % _projectileData.Length, projectileData);
        _fireCount++;
    }

    //private void PlayFireEffect()
    //{
    //    if (FireSound != null)
    //    {
    //        // 발사 사운드를 한 번 재생합니다.
    //        FireSound.PlayOneShot(FireSound.clip);
    //    }

    //    // 총구 이펙트 가시성을 재설정합니다.
    //    _muzzleEffectInstance.SetActive(false);
    //    _muzzleEffectInstance.SetActive(true);

    //    // 발사 애니메이션을 재생합니다.
    //    Animator.SetTrigger("Fire");

    //    // 부모 Player의 발사 이펙트를 재생합니다.
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
        //    // 더블 데미지 활성화 중이면 데미지를 두 배로 증가시킵니다.
        //    damage *= 2f;
        //}

        //if (enemyHealth.ApplyDamage(Object.InputAuthority, damage, position, direction, Type, isCriticalHit) == false)
        //    return;

        //if (HasInputAuthority && Runner.IsForward)
        //{
        //    // 로컬 플레이어에게 UI 히트 이펙트를 표시합니다.
        //   // _sceneObjects.GameUI.PlayerView.Crosshair.ShowHit(enemyHealth.IsAlive == false, isCriticalHit);
        //}
    }

    //private void PlayEmptyClipSound(bool fireJustPressed)
    //{
    //    // 자동 무기의 경우 마지막 발사 후에 빈 탄창 사운드를 한 번 재생하려고 합니다.
    //    bool firstEmptyShot = _fireCooldown.TargetTick.GetValueOrDefault() == Runner.Tick - 1;

    //    if (fireJustPressed == false && firstEmptyShot == false)
    //        return;

    //    if (EmptyClipSound == null || EmptyClipSound.isPlaying)
    //        return;

    //    if (Runner.IsForward && HasInputAuthority)
    //    {
    //        // 빈 탄창 사운드를 재생합니다.
    //        EmptyClipSound.Play();
    //    }
    //}

    /// <summary>
    /// 단일 발사체 발사를 나타내는 구조체입니다.
    /// </summary>
    private struct ProjectileData : INetworkStruct
    {
        public Vector3 HitPosition;
        public Vector3 HitNormal;
        public NetworkBool ShowHitEffect;
    }
}

