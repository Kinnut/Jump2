using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public float minChargeTime = 0.5f;
    public float maxChargeTime = 2.5f;
    public float minDamage = 10f;  // 최소 데미지
    public float maxDamage = 50f;  // 최대 데미지
    public float fireRate = 0.5f;

    public GameObject basicBulletPrefab;  // 기본 공격에 사용되는 총알 프리팹
    public GameObject chargedBulletPref1; // 차지 공격에 사용되는 총알 프리팹들 (변경되지 않음)
    public GameObject chargedBulletPref2;
    public GameObject chargedBulletPref3;

    public Transform firePoint;
    public float bulletSpeed = 20f;

    public Image chargeGaugeImage;

    public AudioClip shootSFX; // 총을 쏠 때 재생할 사운드 클립 (슈팅 사운드)
    private SoundManager soundManager; // SoundManager 참조

    private float chargeTime;
    private bool isCharging = false;
    private float nextFireTime = 0f;

    private int currentBulletIndex = 0;  // 현재 사용 중인 총알 프리팹 인덱스
    public GameObject[] bulletPrefabs;   // 변경 가능한 총알 프리팹 배열

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();  // SoundManager 스크립트 찾기
    }

    void Update()
    {
        if (chargeGaugeImage != null && !isCharging)
        {
            chargeGaugeImage.fillAmount = 0f;
        }

        if (Time.time >= nextFireTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isCharging = true;
                chargeTime = 0f;
            }

            if (isCharging && Input.GetMouseButton(0))
            {
                chargeTime += Time.deltaTime;
                chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime);

                if (chargeGaugeImage != null && chargeTime >= minChargeTime)
                {
                    chargeGaugeImage.fillAmount = (chargeTime - minChargeTime) / (maxChargeTime - minChargeTime);
                }
            }

            if (Input.GetMouseButtonUp(0) && isCharging)
            {
                isCharging = false;

                if (chargeTime < minChargeTime)
                {
                    ShootBasicBullet();  // 기본 공격
                }
                else
                {
                    ShootChargedBullet();  // 차지 공격
                }

                nextFireTime = Time.time + fireRate;

                if (chargeGaugeImage != null)
                {
                    chargeGaugeImage.fillAmount = 0f;
                }
            }
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

    // 기본 총알 프리팹 변경 (아이템 먹을 때)
    public void ChangeBasicBulletPrefab()
    {
        if (currentBulletIndex < bulletPrefabs.Length - 1)
        {
            currentBulletIndex++;
            basicBulletPrefab = bulletPrefabs[currentBulletIndex];
            StrengthenBullets();
            Debug.Log("기본 공격의 총알 프리팹이 변경되었습니다.");
        }
        else
        {
            Debug.Log("더 이상 변경할 프리팹이 없습니다. 마지막 프리팹을 유지합니다.");
        }
    }

    // 총알 강화 (아이템 먹을 때)
    public void StrengthenBullets()
    {
        minDamage += 10f;  // 최소 데미지 10 증가
        maxDamage += 10f;  // 최대 데미지 10 증가
        Debug.Log("총알이 강화되었습니다. 최소 데미지: " + minDamage + ", 최대 데미지: " + maxDamage);
    }

    // 슈팅 사운드를 재생하는 함수
    void PlayShootingSound()
    {
        if (shootSFX != null && soundManager != null)
        {
            soundManager.PlaySFX(shootSFX);  // SoundManager의 SFX 재생 함수 호출
        }
    }
}
