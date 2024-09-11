using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public float minChargeTime = 0.5f;  // ���� ������ ���۵Ǵ� �ּ� �ð�
    public float maxChargeTime = 2.5f;  // �ִ� ���� �ð�
    public float minDamage = 10f;       // �ּ� ������
    public float maxDamage = 50f;       // �ִ� ������
    public float fireRate = 0.5f;       // ���� �ٽ� �������� ���ð�(��Ÿ��)

    public GameObject basicBulletPrefab;
    public GameObject chargedBulletPref1;
    public GameObject chargedBulletPref2;
    public GameObject chargedBulletPref3;

    public Transform firePoint;
    public float bulletSpeed = 20f;

    public Image chargeGaugeImage;      // UI���� ���� ������ �̹��� ����

    private float chargeTime;           // ���콺�� ������ �ִ� �ð�
    private bool isCharging = false;    // ���� ������ Ȯ��
    private float nextFireTime = 0f;    // ���� �߻簡 ������ �ð�

    void Update()
    {
        // ������ �ʱ�ȭ: ���� ���� �ƴϸ� �������� �ʱ�ȭ
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
                chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime); // �ִ� ���� �ð� ����

                // ���� �ð��� �ּ� ���� �ð� �̻��� ���� ������ ������Ʈ
                if (chargeGaugeImage != null && chargeTime >= minChargeTime)
                {
                    // ���� �ð��� 0.5���� ���� fillAmount�� 0, 2.5���� ���� fillAmount�� 1�� �ǵ��� ���
                    chargeGaugeImage.fillAmount = (chargeTime - minChargeTime) / (maxChargeTime - minChargeTime);
                }
            }

            if (Input.GetMouseButtonUp(0) && isCharging)
            {
                isCharging = false;

                if (chargeTime < minChargeTime)
                {
                    ShootBasicBullet(); // �⺻ �Ѿ� �߻�
                }
                else
                {
                    ShootChargedBullet(); // ���� �Ѿ� �߻�
                }

                // �߻� �� ��Ÿ�� ����
                nextFireTime = Time.time + fireRate;

                // ������ �Ϸ�Ǿ����� ������ �ʱ�ȭ
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
            bulletScript.SetDamage(minDamage); // �⺻ ������ ����
            bulletScript.SetDirection(firePoint.up); // �߻� ���� ����
        }

        Debug.Log("�⺻ �Ѿ� �߻�");
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

        Debug.Log($"���� �Ѿ� �߻�! ������: {damage}, ���� ������: {selectedPrefab.name}");
    }
}
