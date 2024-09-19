using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public float health = 100f;                    // �÷��̾��� ü��
    public Image healthBarImage;                   // ü�¹� �̹���
    private float maxHealth;                       // �ִ� ü��

    private CinemachineImpulseSource impulseSource; // Impulse Source ����
    public float speed = 5f;                       // �⺻ �̵� �ӵ�
    private float defaultSpeed;                    // �⺻ �ӵ� ����
    private bool isBoosted = false;                // �ӵ� �ν�Ʈ ����

    private PlayerShooting playerShooting;         // PlayerShooting ��ũ��Ʈ ����
    public ScoreManager scoreManager;              // ScoreManager ���� (���� ����)

    private float playTime = 0f;                   // �÷��� Ÿ�� ���

    // ��� UI �� ī��Ʈ�ٿ� ���� ����
    public GameObject deathUI;                     // ��� UI �г�
    public TextMeshProUGUI countdownText;          // ī��Ʈ�ٿ� �ؽ�Ʈ (TMP)
    private float respawnTime = 10f;               // ��Ȱ������ �ð�
    private bool isDead = false;                   // �÷��̾� ��� ���� ����

    private Rigidbody2D rb;                        // �÷��̾� Rigidbody2D
    private Collider2D playerCollider;             // �÷��̾� �浹ü

    void Start()
    {
        maxHealth = health;                        // �ִ� ü�� ����
        defaultSpeed = speed;                      // �⺻ �ӵ� ����
        UpdateHealthBar();                         // ü�¹� ������Ʈ
        impulseSource = GetComponent<CinemachineImpulseSource>();  // Impulse Source ����
        playerShooting = GetComponent<PlayerShooting>();           // PlayerShooting ��ũ��Ʈ ����
        rb = GetComponent<Rigidbody2D>();          // Rigidbody2D ����
        playerCollider = GetComponent<Collider2D>(); // Collider2D ����

        deathUI.SetActive(false);                  // ���� �� ��� UI ��Ȱ��ȭ
    }

    void Update()
    {
        playTime += Time.deltaTime;                // �÷��� Ÿ�� ����

        if (isDead && respawnTime > 0)
        {
            respawnTime -= Time.deltaTime;
            countdownText.text = "RESPAWN : " + Mathf.Ceil(respawnTime).ToString();  // ���� �ð��� CoolTime �������� ǥ��

            if (respawnTime <= 0)
            {
                Respawn();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // ��� �߿��� �������� ���� ����

        health -= damage;

        if (health < 0)
        {
            health = 0;
        }

        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        UpdateHealthBar();

        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return; // ��� �߿��� ȸ�� �Ұ�

        health += amount;

        health = Mathf.Clamp(health, 0, maxHealth);

        UpdateHealthBar();
        Debug.Log("�÷��̾� ü�� ȸ��: " + health);
    }

    public void StrengthenBasicAttack()
    {
        playerShooting.ChangeBasicBulletPrefab();  // �⺻ �Ѿ� ������ ����
        Debug.Log("�⺻ ������ ��ȭ�Ǿ����ϴ�.");
    }

    public void IncreaseSpeed(float amount, float duration)
    {
        if (!isBoosted)
        {
            speed += amount;
            isBoosted = true;
            Debug.Log("�ӵ� ����: " + amount + " for " + duration + " seconds");
            Invoke("ResetSpeed", duration);
        }
    }

    private void ResetSpeed()
    {
        speed = defaultSpeed;
        isBoosted = false;
        Debug.Log("�ӵ� ���󺹱�");
    }

    void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = health / maxHealth;
        }
    }

    // �÷��̾� ��� ó��
    void Die()
    {
        Debug.Log("�÷��̾� ���");
        isDead = true;

        // ��� UI Ȱ��ȭ �� ī��Ʈ�ٿ� �ʱ�ȭ
        deathUI.SetActive(true);
        respawnTime = 10f;  // 10�ʷ� �ʱ�ȭ

        // �÷��̾� �̵� �� ���� ��Ȱ��ȭ
        DisablePlayer();
    }

    // �÷��̾� ��Ȱ ó��
    void Respawn()
    {
        Debug.Log("�÷��̾� ��Ȱ");
        isDead = false;
        health = maxHealth;      // ü�� ȸ��
        UpdateHealthBar();       // ü�¹� ������Ʈ

        // �÷��̾� �̵� �� ���� Ȱ��ȭ
        EnablePlayer();

        deathUI.SetActive(false); // ��� UI ��Ȱ��ȭ
    }

    // �÷��̾� ��Ȱ��ȭ
    void DisablePlayer()
    {
        playerShooting.enabled = false;   // �÷��̾� ���� ��Ȱ��ȭ
        rb.velocity = Vector2.zero;       // �÷��̾� �̵� ����
        rb.isKinematic = true;            // Rigidbody�� ���� ���� ��Ȱ��ȭ
        playerCollider.enabled = false;   // �浹 ��Ȱ��ȭ
    }

    // �÷��̾� Ȱ��ȭ
    void EnablePlayer()
    {
        playerShooting.enabled = true;    // �÷��̾� ���� Ȱ��ȭ
        rb.isKinematic = false;           // Rigidbody�� ���� ���� Ȱ��ȭ
        playerCollider.enabled = true;    // �浹 Ȱ��ȭ
    }
}
