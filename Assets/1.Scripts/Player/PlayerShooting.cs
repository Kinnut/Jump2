using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public float minChargeTime = 0.5f;
    public float maxChargeTime = 2.5f;
    public float minDamage = 10f;  // �ּ� ������
    public float maxDamage = 50f;  // �ִ� ������
    public float fireRate = 0.5f;  // �ڵ� ���� �ӵ�

    public GameObject basicBulletPrefab;  // �⺻ ���ݿ� ���Ǵ� �Ѿ� ������
    public GameObject chargedBulletPref1; // ���� ���ݿ� ���Ǵ� �Ѿ� �����յ� (������� ����)
    public GameObject chargedBulletPref2;
    public GameObject chargedBulletPref3;

    public Transform firePoint;
    public float bulletSpeed = 20f;

    public Image chargeGaugeImage;

    public AudioClip shootSFX; // ���� �� �� ����� ���� Ŭ�� (���� ����)
    private SoundManager soundManager; // SoundManager ����

    private float chargeTime;
    private bool isCharging = false;
    private float nextFireTime = 0f;

    private bool isAutoShooting = false; // �ڵ� ���� Ȱ��ȭ ����
    private int currentBulletIndex = 0;  // ���� ��� ���� �Ѿ� ������ �ε���
    public GameObject[] bulletPrefabs;   // ���� ������ �Ѿ� ������ �迭

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();  // SoundManager ��ũ��Ʈ ã��
    }

    void Update()
    {
        // CŰ�� �ڵ� ���� Ȱ��ȭ/��Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.C))
        {
            isAutoShooting = !isAutoShooting;
            Debug.Log("�ڵ� ����: " + (isAutoShooting ? "Ȱ��ȭ��" : "��Ȱ��ȭ��"));
        }

        // �ڵ� ������ Ȱ��ȭ�� ��� ���� �ð����� �⺻ �Ѿ� �߻�
        if (isAutoShooting && Time.time >= nextFireTime)
        {
            ShootBasicBullet();
            nextFireTime = Time.time + fireRate;
        }

        // �ڵ� ������ ��Ȱ��ȭ�� ��� ���콺 ��Ŭ������ ���� ����
        if (!isAutoShooting && Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            ShootBasicBullet();
            nextFireTime = Time.time + fireRate;
        }

        // ���� ���� (�����̽��ٷ� ����)
        if (chargeGaugeImage != null && !isCharging)
        {
            chargeGaugeImage.fillAmount = 0f;
        }

        // ���� ���� ���� (�����̽��� ����)
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

        // �����̽��ٸ� ���� ���� ���� �߻�
        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;

            // �ּ� ���� �ð��� 0.5�� �̻��� ���� ���� ���� �߻�
            if (chargeTime >= minChargeTime)
            {
                ShootChargedBullet();  // ���� ����
            }

            if (chargeGaugeImage != null)
            {
                chargeGaugeImage.fillAmount = 0f;
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

        // ���� ���� ���
        PlayShootingSound();
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

        // ���� ���� ���
        PlayShootingSound();
    }

    public void ChangeBasicBulletPrefab()
    {
        if (bulletPrefabs.Length == 0)
        {
            Debug.LogWarning("�Ѿ� ������ �迭�� ����ֽ��ϴ�.");
            return;
        }

        // ���� �Ѿ� �ε����� ������Ű�� �迭�� ������ �ʰ��ϸ� ó������ ���ư�
        currentBulletIndex = (currentBulletIndex + 1) % bulletPrefabs.Length;
        basicBulletPrefab = bulletPrefabs[currentBulletIndex];

        Debug.Log("�⺻ ������ �Ѿ� �������� ����Ǿ����ϴ�. ���� ������ �ε���: " + currentBulletIndex);
    }


    // ���� ���带 ����ϴ� �Լ�
    void PlayShootingSound()
    {
        if (shootSFX != null && soundManager != null)
        {
            soundManager.PlaySFX(shootSFX);  // SoundManager�� SFX ��� �Լ� ȣ��
        }
    }
}
