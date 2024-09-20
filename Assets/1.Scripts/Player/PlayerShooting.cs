using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public float minChargeTime = 0.5f;
    public float maxChargeTime = 2.5f;
    public float minDamage = 10f;  // �ּ� ������
    public float maxDamage = 50f;  // �ִ� ������
    public float fireRate = 0.5f;  // �⺻ �ڵ� ���� �ӵ�
    private float originalFireRate;  // ���� ���� �ӵ� ����

    public float rapidFireRate = 0.2f;  // �ӻ� ��� �� ���� �ӵ�
    public float rapidFireDuration = 10f;  // �ӻ� ���� �ð�
    public float rapidFireCooldown = 30f;  // �ӻ� ��Ÿ��
    private bool isRapidFireActive = false;  // �ӻ� ��� ����
    private bool isRapidFireOnCooldown = false;  // ��Ÿ�� ����

    public GameObject basicBulletPrefab;  // �⺻ ���ݿ� ���Ǵ� �Ѿ� ������
    public GameObject chargedBulletPref1; // ���� ���ݿ� ���Ǵ� �Ѿ� �����յ�
    public GameObject chargedBulletPref2;
    public GameObject chargedBulletPref3;

    public Transform firePoint;
    public float bulletSpeed = 20f;
    public Image chargeGaugeImage;
    public Image cooldownImage;  // ��Ÿ���� ǥ���ϴ� �ʾ��Ʈ �̹���

    public AudioClip shootSFX; // ���� �� �� ����� ���� Ŭ�� (���� ����)
    private SoundManager soundManager; // SoundManager ����

    private float chargeTime;
    private bool isCharging = false;
    private float nextFireTime = 0f;
    private float rapidFireCooldownTimer = 0f;  // ��Ÿ�� Ÿ�̸�

    private bool isAutoShooting = false; // �ڵ� ���� Ȱ��ȭ ����
    private int currentBulletIndex = 0;  // ���� ��� ���� �Ѿ� ������ �ε���
    public GameObject[] bulletPrefabs;   // ���� ������ �Ѿ� ������ �迭

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();  // SoundManager ��ũ��Ʈ ã��
        originalFireRate = fireRate;  // �⺻ ���� �ӵ� ����
        cooldownImage.fillAmount = 0; // ��Ÿ�� UI �ʱ�ȭ
    }

    void Update()
    {
        HandleShooting();  // ���� ó��
        HandleRapidFireSkill();  // �ӻ� ��ų ó��
        HandleCooldownUI();  // ��Ÿ�� UI ó��

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

    // E Ű�� �ӻ� ��ų�� �ߵ��ϰ� ��Ÿ���� ó���ϴ� �Լ�
    void HandleRapidFireSkill()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isRapidFireOnCooldown)
        {
            ActivateRapidFire();
        }

        // �ӻ� ��尡 Ȱ��ȭ�� ���
        if (isRapidFireActive)
        {
            rapidFireDuration -= Time.deltaTime;

            if (rapidFireDuration <= 0f)
            {
                DeactivateRapidFire();
            }
        }

        // ��Ÿ�� ó��
        if (isRapidFireOnCooldown)
        {
            rapidFireCooldownTimer -= Time.deltaTime;

            if (rapidFireCooldownTimer <= 0f)
            {
                isRapidFireOnCooldown = false;  // ��Ÿ�� ����
                cooldownImage.fillAmount = 0f;  // ��Ÿ�� UI �ʱ�ȭ
            }
        }
    }

    // �ӻ� ��� Ȱ��ȭ
    void ActivateRapidFire()
    {
        fireRate = rapidFireRate;  // ���� �ӵ� ����
        isRapidFireActive = true;
        isRapidFireOnCooldown = true;
        rapidFireCooldownTimer = rapidFireCooldown;  // ��Ÿ�� ����
        rapidFireDuration = 10f;  // �ӻ� ���� �ð� �ʱ�ȭ
    }

    // �ӻ� ��� ��Ȱ��ȭ
    void DeactivateRapidFire()
    {
        fireRate = originalFireRate;  // ���� ���� �ӵ��� ����
        isRapidFireActive = false;
    }

    // ��Ÿ�� UI ó��
    void HandleCooldownUI()
    {
        if (isRapidFireOnCooldown)
        {
            cooldownImage.fillAmount = rapidFireCooldownTimer / rapidFireCooldown;  // ���� ��Ÿ���� UI�� ǥ��
        }
    }

    // �⺻ ���� ó��
    void HandleShooting()
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

    // ���� ���带 ����ϴ� �Լ�
    void PlayShootingSound()
    {
        if (shootSFX != null && soundManager != null)
        {
            soundManager.PlaySFX(shootSFX);  // SoundManager�� SFX ��� �Լ� ȣ��
        }
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
}
