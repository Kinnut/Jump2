using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviourPun
{
    // �⺻ ������
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
    private int currentBulletIndex = 0;  // ���� ��� ���� �Ѿ� ������ �ε���
    public GameObject[] bulletPrefabs;   // ���� ������ �Ѿ� ������ �迭
    private GameObject basicBulletPrefab; // ���� ��� ���� �⺻ �Ѿ� ������

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        originalFireRate = fireRate;
        cooldownImage.fillAmount = 0;
        basicBulletPrefab = bulletPrefabs[0];  // �⺻ �Ѿ� ������ ����
    }

    void Update()
    {
        HandleShooting();
        HandleRapidFireSkill();
        HandleCooldownUI();
        HandleCharging();
    }

    // E Ű�� �ӻ� ��ų�� �ߵ��ϰ� ��Ÿ���� ó���ϴ� �Լ�
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

    // ��Ÿ�� UI ó��
    void HandleCooldownUI()
    {
        if (isRapidFireOnCooldown)
        {
            cooldownImage.fillAmount = rapidFireCooldownTimer / rapidFireCooldown;
        }
    }

    // �⺻ ���� ó��
    void HandleShooting()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            isAutoShooting = !isAutoShooting;
            Debug.Log("�ڵ� ����: " + (isAutoShooting ? "Ȱ��ȭ��" : "��Ȱ��ȭ��"));
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

    // �⺻ �Ѿ� �߻�
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

    // �⺻ ���� �������� ����ȭ�ϸ鼭 �����ϴ� �޼���
    public void ChangeBasicBulletPrefab()
    {
        // �Ѿ��� ������ �ܰ迡 �������� �ʾ��� ��쿡�� ��ȭ
        if (currentBulletIndex < bulletPrefabs.Length - 1)
        {
            currentBulletIndex++;  // ���� �Ѿ˷� ��ȯ
            basicBulletPrefab = bulletPrefabs[currentBulletIndex];  // ���ο� �Ѿ� ������ ����
            Debug.Log("�Ѿ��� ��ȭ�Ǿ����ϴ�. ���� �Ѿ� �ε���: " + currentBulletIndex);
        }
        else
        {
            Debug.Log("�Ѿ��� �ִ� ��ȭ �ܰ迡 �����߽��ϴ�.");  // �� �̻� ��ȭ���� ����
        }
    }

    // ���� �÷��̾ �Ѿ��� ��ȭ�� �� RPC�� ȣ���Ͽ� ����ȭ
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

    // ������ �Ѿ� �߻�
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
