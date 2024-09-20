using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public float minChargeTime = 0.5f;
    public float maxChargeTime = 2.5f;
    public float minDamage = 10f;  // 최소 데미지
    public float maxDamage = 50f;  // 최대 데미지
    public float fireRate = 0.5f;  // 기본 자동 공격 속도
    private float originalFireRate;  // 원래 공격 속도 저장

    public float rapidFireRate = 0.2f;  // 속사 모드 시 공격 속도
    public float rapidFireDuration = 10f;  // 속사 지속 시간
    public float rapidFireCooldown = 30f;  // 속사 쿨타임
    private bool isRapidFireActive = false;  // 속사 모드 여부
    private bool isRapidFireOnCooldown = false;  // 쿨타임 여부

    public GameObject basicBulletPrefab;  // 기본 공격에 사용되는 총알 프리팹
    public GameObject chargedBulletPref1; // 차지 공격에 사용되는 총알 프리팹들
    public GameObject chargedBulletPref2;
    public GameObject chargedBulletPref3;

    public Transform firePoint;
    public float bulletSpeed = 20f;
    public Image chargeGaugeImage;
    public Image cooldownImage;  // 쿨타임을 표시하는 필어마운트 이미지

    public AudioClip shootSFX; // 총을 쏠 때 재생할 사운드 클립 (슈팅 사운드)
    private SoundManager soundManager; // SoundManager 참조

    private float chargeTime;
    private bool isCharging = false;
    private float nextFireTime = 0f;
    private float rapidFireCooldownTimer = 0f;  // 쿨타임 타이머

    private bool isAutoShooting = false; // 자동 공격 활성화 여부
    private int currentBulletIndex = 0;  // 현재 사용 중인 총알 프리팹 인덱스
    public GameObject[] bulletPrefabs;   // 변경 가능한 총알 프리팹 배열

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();  // SoundManager 스크립트 찾기
        originalFireRate = fireRate;  // 기본 공격 속도 저장
        cooldownImage.fillAmount = 0; // 쿨타임 UI 초기화
    }

    void Update()
    {
        HandleShooting();  // 공격 처리
        HandleRapidFireSkill();  // 속사 스킬 처리
        HandleCooldownUI();  // 쿨타임 UI 처리

        // 차지 공격 (스페이스바로 충전)
        if (chargeGaugeImage != null && !isCharging)
        {
            chargeGaugeImage.fillAmount = 0f;
        }

        // 차지 공격 충전 (스페이스바 누름)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = true;
            chargeTime = 0f;
        }

        if (isCharging && Input.GetKey(KeyCode.Space))
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime);

            if (chargeGaugeImage != null && chargeTime >= minChargeTime)
            {
                chargeGaugeImage.fillAmount = (chargeTime - minChargeTime) / (maxChargeTime - minChargeTime);
            }
        }

        // 스페이스바를 떼면 차지 공격 발사
        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;

            // 최소 차지 시간이 0.5초 이상일 때만 차지 공격 발사
            if (chargeTime >= minChargeTime)
            {
                ShootChargedBullet();  // 차지 공격
            }

            if (chargeGaugeImage != null)
            {
                chargeGaugeImage.fillAmount = 0f;
            }
        }
    }

    // E 키로 속사 스킬을 발동하고 쿨타임을 처리하는 함수
    void HandleRapidFireSkill()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isRapidFireOnCooldown)
        {
            ActivateRapidFire();
        }

        // 속사 모드가 활성화된 경우
        if (isRapidFireActive)
        {
            rapidFireDuration -= Time.deltaTime;

            if (rapidFireDuration <= 0f)
            {
                DeactivateRapidFire();
            }
        }

        // 쿨타임 처리
        if (isRapidFireOnCooldown)
        {
            rapidFireCooldownTimer -= Time.deltaTime;

            if (rapidFireCooldownTimer <= 0f)
            {
                isRapidFireOnCooldown = false;  // 쿨타임 종료
                cooldownImage.fillAmount = 0f;  // 쿨타임 UI 초기화
            }
        }
    }

    // 속사 모드 활성화
    void ActivateRapidFire()
    {
        fireRate = rapidFireRate;  // 공격 속도 증가
        isRapidFireActive = true;
        isRapidFireOnCooldown = true;
        rapidFireCooldownTimer = rapidFireCooldown;  // 쿨타임 시작
        rapidFireDuration = 10f;  // 속사 지속 시간 초기화
    }

    // 속사 모드 비활성화
    void DeactivateRapidFire()
    {
        fireRate = originalFireRate;  // 원래 공격 속도로 복구
        isRapidFireActive = false;
    }

    // 쿨타임 UI 처리
    void HandleCooldownUI()
    {
        if (isRapidFireOnCooldown)
        {
            cooldownImage.fillAmount = rapidFireCooldownTimer / rapidFireCooldown;  // 남은 쿨타임을 UI에 표시
        }
    }

    // 기본 공격 처리
    void HandleShooting()
    {
        // C키로 자동 공격 활성화/비활성화
        if (Input.GetKeyDown(KeyCode.C))
        {
            isAutoShooting = !isAutoShooting;
            Debug.Log("자동 공격: " + (isAutoShooting ? "활성화됨" : "비활성화됨"));
        }

        // 자동 공격이 활성화된 경우 일정 시간마다 기본 총알 발사
        if (isAutoShooting && Time.time >= nextFireTime)
        {
            ShootBasicBullet();
            nextFireTime = Time.time + fireRate;
        }

        // 자동 공격이 비활성화된 경우 마우스 좌클릭으로 수동 공격
        if (!isAutoShooting && Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            ShootBasicBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    // 기본 총알 발사
    void ShootBasicBullet()
    {
        GameObject bullet = Instantiate(basicBulletPrefab, firePoint.position, firePoint.rotation);
        PlayerBullet bulletScript = bullet.GetComponent<PlayerBullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDamage(minDamage);  // 기본 공격의 최소 데미지 설정
            bulletScript.SetDirection(firePoint.up);
        }

        // 슈팅 사운드 재생
        PlayShootingSound();
    }

    // 차지된 총알 발사
    void ShootChargedBullet()
    {
        float damage = Mathf.Lerp(minDamage, maxDamage, (chargeTime - minChargeTime) / (maxChargeTime - minChargeTime));

        GameObject selectedPrefab = null;

        if (chargeTime < minChargeTime + (maxChargeTime - minChargeTime) / 3f)
        {
            selectedPrefab = chargedBulletPref1;
        }
        else if (chargeTime < minChargeTime + 2 * (maxChargeTime - minChargeTime) / 3f)
        {
            selectedPrefab = chargedBulletPref2;
        }
        else
        {
            selectedPrefab = chargedBulletPref3;
        }

        GameObject bullet = Instantiate(selectedPrefab, firePoint.position, firePoint.rotation);
        PlayerBullet bulletScript = bullet.GetComponent<PlayerBullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDamage(damage);  // 차지된 데미지만 증가
            bulletScript.SetDirection(firePoint.up);
        }

        // 슈팅 사운드 재생
        PlayShootingSound();
    }

    // 슈팅 사운드를 재생하는 함수
    void PlayShootingSound()
    {
        if (shootSFX != null && soundManager != null)
        {
            soundManager.PlaySFX(shootSFX);  // SoundManager의 SFX 재생 함수 호출
        }
    }

    public void ChangeBasicBulletPrefab()
    {
        if (bulletPrefabs.Length == 0)
        {
            Debug.LogWarning("총알 프리팹 배열이 비어있습니다.");
            return;
        }

        // 현재 총알 인덱스를 증가시키고 배열의 범위를 초과하면 처음으로 돌아감
        currentBulletIndex = (currentBulletIndex + 1) % bulletPrefabs.Length;
        basicBulletPrefab = bulletPrefabs[currentBulletIndex];

        Debug.Log("기본 공격의 총알 프리팹이 변경되었습니다. 현재 프리팹 인덱스: " + currentBulletIndex);
    }
}
