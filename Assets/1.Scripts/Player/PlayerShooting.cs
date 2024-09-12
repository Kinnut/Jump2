using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public float minChargeTime = 0.5f;
    public float maxChargeTime = 2.5f;
    public float minDamage = 10f;  // �ּ� ������
    public float maxDamage = 50f;  // �ִ� ������
    public float fireRate = 0.5f;

    public GameObject basicBulletPrefab;  // �⺻ ���ݿ� ���Ǵ� �Ѿ� ������
    public GameObject chargedBulletPref1; // ���� ���ݿ� ���Ǵ� �Ѿ� �����յ� (������� ����)
    public GameObject chargedBulletPref2;
    public GameObject chargedBulletPref3;

    public Transform firePoint;
    public float bulletSpeed = 20f;

    public Image chargeGaugeImage;

    private float chargeTime;
    private bool isCharging = false;
    private float nextFireTime = 0f;

    private int currentBulletIndex = 0;  // ���� ��� ���� �Ѿ� ������ �ε���
    public GameObject[] bulletPrefabs;   // ���� ������ �Ѿ� ������ �迭

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
                    ShootBasicBullet();  // �⺻ ����
                }
                else
                {
                    ShootChargedBullet();  // ���� ����
                }

                nextFireTime = Time.time + fireRate;

                if (chargeGaugeImage != null)
                {
                    chargeGaugeImage.fillAmount = 0f;
                }
            }
        }
    }

    // �⺻ �Ѿ� �߻�
    void ShootBasicBullet()
    {
        GameObject bullet = Instantiate(basicBulletPrefab, firePoint.position, firePoint.rotation);
        PlayerBullet bulletScript = bullet.GetComponent<PlayerBullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDamage(minDamage);  // �⺻ ������ �ּ� ������ ����
            bulletScript.SetDirection(firePoint.up);
        }
    }

    // ������ �Ѿ� �߻�
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
            bulletScript.SetDamage(damage);  // ������ �������� ����
            bulletScript.SetDirection(firePoint.up);
        }
    }

    public void ChangeBasicBulletPrefab()
    {
        if (currentBulletIndex < bulletPrefabs.Length - 1)
        {
            currentBulletIndex++;
            basicBulletPrefab = bulletPrefabs[currentBulletIndex];
            StrengthenBullets();
            Debug.Log("�⺻ ������ �Ѿ� �������� ����Ǿ����ϴ�.");
        }
        else
        {
            Debug.Log("�� �̻� ������ �������� �����ϴ�. ������ �������� �����մϴ�.");
        }
    }

    public void StrengthenBullets()
    {
        minDamage += 10f;  // �ּ� ������ 10 ����
        maxDamage += 10f;  // �ִ� ������ 10 ����
        Debug.Log("�Ѿ��� ��ȭ�Ǿ����ϴ�. �ּ� ������: " + minDamage + ", �ִ� ������: " + maxDamage);
    }
}
