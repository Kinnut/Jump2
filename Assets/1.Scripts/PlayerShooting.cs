using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public float minChargeTime = 0.5f;  // 차지 공격이 시작되는 최소 시간
    public float maxChargeTime = 2.5f;  // 최대 충전 시간
    public float minDamage = 10f;       // 최소 데미지
    public float maxDamage = 50f;       // 최대 데미지
    public float fireRate = 0.5f;       // 총을 다시 쏘기까지의 대기시간(쿨타임)

    public GameObject basicBulletPrefab;
    public GameObject chargedBulletPref1;
    public GameObject chargedBulletPref2;
    public GameObject chargedBulletPref3;

    public Transform firePoint;
    public float bulletSpeed = 20f;

    public Image chargeGaugeImage;      // UI에서 충전 게이지 이미지 참조

    private float chargeTime;           // 마우스를 누르고 있는 시간
    private bool isCharging = false;    // 충전 중인지 확인
    private float nextFireTime = 0f;    // 다음 발사가 가능한 시간

    void Update()
    {
        // 게이지 초기화: 충전 중이 아니면 게이지를 초기화
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
                chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime); // 최대 충전 시간 제한

                // 충전 시간이 최소 충전 시간 이상일 때만 게이지 업데이트
                if (chargeGaugeImage != null && chargeTime >= minChargeTime)
                {
                    // 충전 시간이 0.5초일 때는 fillAmount가 0, 2.5초일 때는 fillAmount가 1이 되도록 계산
                    chargeGaugeImage.fillAmount = (chargeTime - minChargeTime) / (maxChargeTime - minChargeTime);
                }
            }

            if (Input.GetMouseButtonUp(0) && isCharging)
            {
                isCharging = false;

                if (chargeTime < minChargeTime)
                {
                    ShootBasicBullet(); // 기본 총알 발사
                }
                else
                {
                    ShootChargedBullet(); // 충전 총알 발사
                }

                // 발사 후 쿨타임 설정
                nextFireTime = Time.time + fireRate;

                // 충전이 완료되었으니 게이지 초기화
                if (chargeGaugeImage != null)
                {
                    chargeGaugeImage.fillAmount = 0f;
                }
            }
        }
    }

    void ShootBasicBullet()
    {
        GameObject bullet = Instantiate(basicBulletPrefab, firePoint.position, firePoint.rotation);
        PlayerBullet bulletScript = bullet.GetComponent<PlayerBullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDamage(minDamage); // 기본 데미지 설정
            bulletScript.SetDirection(firePoint.up); // 발사 방향 설정
        }

        Debug.Log("기본 총알 발사");
    }

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
            bulletScript.SetDamage(damage);
            bulletScript.SetDirection(firePoint.up);
        }

        Debug.Log($"충전 총알 발사! 데미지: {damage}, 사용된 프리팹: {selectedPrefab.name}");
    }
}
