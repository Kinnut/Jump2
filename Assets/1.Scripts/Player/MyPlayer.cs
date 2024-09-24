using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyPlayer : MonoBehaviour
{
    // ���� ������
    public float health = 100f;
    public Image healthBarImage;
    private float maxHealth;

    private CinemachineImpulseSource impulseSource;
    public float speed = 5f;
    private float defaultSpeed;
    private bool isBoosted = false;

    private PlayerShooting playerShooting;
    private PlayerMovement playerMovement;  // PlayerMovement ��ũ��Ʈ ����
    public ScoreManager scoreManager;

    private float playTime = 0f;

    public GameObject deathUI;
    public TextMeshProUGUI countdownText;
    private float respawnTime = 10f;
    public bool isDead = false;

    private Rigidbody2D rb;
    private Collider2D playerCollider;

    void Start()
    {
        maxHealth = health;
        defaultSpeed = speed;
        UpdateHealthBar();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        playerShooting = GetComponent<PlayerShooting>();
        playerMovement = GetComponent<PlayerMovement>();  // PlayerMovement ��ũ��Ʈ ���� �߰�
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        deathUI.SetActive(false);
    }

    void Update()
    {
        playTime += Time.deltaTime;

        if (isDead && respawnTime > 0)
        {
            respawnTime -= Time.deltaTime;
            countdownText.text = "RESPAWN : " + Mathf.Ceil(respawnTime).ToString();

            if (respawnTime <= 0)
            {
                Respawn();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

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
        if (isDead) return;

        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthBar();
        Debug.Log("�÷��̾� ü�� ȸ��: " + health);
    }

    public void StrengthenBasicAttack()
    {
        playerShooting.ChangeBasicBulletPrefab();
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

        deathUI.SetActive(true);
        respawnTime = 10f;

        // �÷��̾� �̵� �� ���� ��Ȱ��ȭ
        DisablePlayer();
    }

    // �÷��̾� ��Ȱ ó��
    void Respawn()
    {
        Debug.Log("�÷��̾� ��Ȱ");
        isDead = false;
        health = maxHealth;
        UpdateHealthBar();

        EnablePlayer();

        deathUI.SetActive(false);
    }

    // �÷��̾� ��Ȱ��ȭ
    void DisablePlayer()
    {
        playerShooting.enabled = false;
        playerMovement.enabled = false;   // PlayerMovement ��Ȱ��ȭ �߰�
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        playerCollider.enabled = false;
    }

    // �÷��̾� Ȱ��ȭ
    void EnablePlayer()
    {
        playerShooting.enabled = true;
        playerMovement.enabled = true;    // PlayerMovement Ȱ��ȭ �߰�
        rb.isKinematic = false;
        playerCollider.enabled = true;
    }
}
