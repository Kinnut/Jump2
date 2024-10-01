using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviourPun
{
    // 기본 변수들
    public float minChargeTime = 0.5f;
    public float maxChargeTime = 2.5f;
    public float minDamage = 10f;
    public float maxDamage = 50f;
    public float fireRate = 0.5f;
    private float originalFireRate;

    public float rapidFireRate = 0.2f;
    public float rapidFireDuration = 10f;
    public float rapidFireCooldown = 30f;
    private bool isRapidFireActive = false;
    private bool isRapidFireOnCooldown = false;

    public Transform firePoint;
    public float bulletSpeed = 20f;
    public Image chargeGaugeImage;
    public Image cooldownImage;

    public AudioClip shootSFX;
    private SoundManager soundManager;

    private float chargeTime;
    private bool isCharging = false;
    private float nextFireTime = 0f;
    private float rapidFireCooldownTimer = 0f;

    private bool isAutoShooting = false;
    private int currentBulletIndex = 0;  // 현재 사용 중인 총알 프리팹 인덱스
    public GameObject[] bulletPrefabs;   // 변경 가능한 총알 프리팹 배열
    private GameObject basicBulletPrefab; // 현재 사용 중인 기본 총알 프리팹

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        originalFireRate = fireRate;
        cooldownImage.fillAmount = 0;
        basicBulletPrefab = bulletPrefabs[0];  // 기본 총알 프리팹 설정
    }

    void Update()
    {
        HandleShooting();
        HandleRapidFireSkill();
        HandleCooldownUI();
        HandleCharging();
    }

    // E 키로 속사 스킬을 발동하고 쿨타임을 처리하는 함수
    void HandleRapidFireSkill()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isRapidFireOnCooldown)
        {
            ActivateRapidFire();
        }

        if (isRapidFireActive)
        {
            rapidFireDuration -= Time.deltaTime;
            if (rapidFireDuration <= 0f)
            {
                DeactivateRapidFire();
            }
        }

        if (isRapidFireOnCooldown)
        {
            rapidFireCooldownTimer -= Time.deltaTime;
            if (rapidFireCooldownTimer <= 0f)
            {
                isRapidFireOnCooldown = false;
                cooldownImage.fillAmount = 0f;
            }
        }
    }

    void ActivateRapidFire()
    {
        fireRate = rapidFireRate;
        isRapidFireActive = true;
        isRapidFireOnCooldown = true;
        rapidFireCooldownTimer = rapidFireCooldown;
        rapidFireDuration = 10f;
    }

    void DeactivateRapidFire()
    {
        fireRate = originalFireRate;
        isRapidFireActive = false;
    }

    // 쿨타임 UI 처리
    void HandleCooldownUI()
    {
        if (isRapidFireOnCooldown)
        {
            cooldownImage.fillAmount = rapidFireCooldownTimer / rapidFireCooldown;
        }
    }

    // 기본 공격 처리
    void HandleShooting()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            isAutoShooting = !isAutoShooting;
            Debug.Log("자동 공격: " + (isAutoShooting ? "활성화됨" : "비활성화됨"));
        }

        if (isAutoShooting && Time.time >= nextFireTime)
        {
            ShootBasicBullet();
            nextFireTime = Time.time + fireRate;
        }

        if (!isAutoShooting && Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            ShootBasicBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    // 기본 총알 발사
    void ShootBasicBullet()
    {
        if (photonView.IsMine)
        {
            RPC_ShootBasicBullet();
        }
    }

    void RPC_ShootBasicBullet()
    {
        GameObject bullet = PhotonNetwork.Instantiate(basicBulletPrefab.name, firePoint.position, firePoint.rotation);
        PlayerBullet bulletScript = bullet.GetComponent<PlayerBullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDamage(minDamage);
            bulletScript.SetDirection(firePoint.up);
        }

        PlayShootingSound();
    }

    // 기본 공격 프리팹을 동기화하면서 변경하는 메서드
    public void ChangeBasicBulletPrefab()
    {
        // 총알이 마지막 단계에 도달하지 않았을 경우에만 강화
        if (currentBulletIndex < bulletPrefabs.Length - 1)
        {
            currentBulletIndex++;  // 다음 총알로 전환
            basicBulletPrefab = bulletPrefabs[currentBulletIndex];  // 새로운 총알 프리팹 설정
            Debug.Log("총알이 강화되었습니다. 현재 총알 인덱스: " + currentBulletIndex);
        }
        else
        {
            Debug.Log("총알이 최대 강화 단계에 도달했습니다.");  // 더 이상 강화되지 않음
        }
    }

    // 로컬 플레이어가 총알을 강화할 때 RPC를 호출하여 동기화
    public void StrengthenBasicBullet()
    {
        if (photonView.IsMine)
        {
            int newIndex = currentBulletIndex + 1;
            photonView.RPC("ChangeBasicBulletPrefab", RpcTarget.All, newIndex);
        }
    }

    void PlayShootingSound()
    {
        if (shootSFX != null && soundManager != null)
        {
            soundManager.PlaySFX(shootSFX);
        }
    }

    void HandleCharging()
    {
        if (chargeGaugeImage != null && !isCharging)
        {
            chargeGaugeImage.fillAmount = 0f;
        }

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

        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;

            if (chargeTime >= minChargeTime)
            {
                ShootChargedBullet();
            }

            if (chargeGaugeImage != null)
            {
                chargeGaugeImage.fillAmount = 0f;
            }
        }
    }

    // 차지된 총알 발사
    void ShootChargedBullet()
    {
        if (photonView.IsMine)
        {
            RPC_ShootChargedBullet(chargeTime);
        }
    }

    void RPC_ShootChargedBullet(float chargeTime)
    {
        float damage = Mathf.Lerp(minDamage, maxDamage, (chargeTime - minChargeTime) / (maxChargeTime - minChargeTime));
        GameObject selectedPrefab = PhotonNetwork.Instantiate("ChargedBullet1", firePoint.position, firePoint.rotation);
        PlayerBullet bulletScript = selectedPrefab.GetComponent<PlayerBullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDamage(damage);
            bulletScript.SetDirection(firePoint.up);
        }

        PlayShootingSound();
    }
}
