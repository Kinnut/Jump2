using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyPlayer : MonoBehaviour
{
    // 기존 변수들
    public float health = 100f;
    public Image healthBarImage;
    private float maxHealth;

    private CinemachineImpulseSource impulseSource;
    public float speed = 5f;
    private float defaultSpeed;
    private bool isBoosted = false;

    private PlayerShooting playerShooting;
    private PlayerMovement playerMovement;  // PlayerMovement 스크립트 참조
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
        playerMovement = GetComponent<PlayerMovement>();  // PlayerMovement 스크립트 참조 추가
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
        Debug.Log("플레이어 체력 회복: " + health);
    }

    public void StrengthenBasicAttack()
    {
        playerShooting.ChangeBasicBulletPrefab();
        Debug.Log("기본 공격이 강화되었습니다.");
    }

    public void IncreaseSpeed(float amount, float duration)
    {
        if (!isBoosted)
        {
            speed += amount;
            isBoosted = true;
            Debug.Log("속도 증가: " + amount + " for " + duration + " seconds");
            Invoke("ResetSpeed", duration);
        }
    }

    private void ResetSpeed()
    {
        speed = defaultSpeed;
        isBoosted = false;
        Debug.Log("속도 원상복구");
    }

    void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = health / maxHealth;
        }
    }

    // 플레이어 사망 처리
    void Die()
    {
        Debug.Log("플레이어 사망");
        isDead = true;

        deathUI.SetActive(true);
        respawnTime = 10f;

        // 플레이어 이동 및 공격 비활성화
        DisablePlayer();
    }

    // 플레이어 부활 처리
    void Respawn()
    {
        Debug.Log("플레이어 부활");
        isDead = false;
        health = maxHealth;
        UpdateHealthBar();

        EnablePlayer();

        deathUI.SetActive(false);
    }

    // 플레이어 비활성화
    void DisablePlayer()
    {
        playerShooting.enabled = false;
        playerMovement.enabled = false;   // PlayerMovement 비활성화 추가
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        playerCollider.enabled = false;
    }

    // 플레이어 활성화
    void EnablePlayer()
    {
        playerShooting.enabled = true;
        playerMovement.enabled = true;    // PlayerMovement 활성화 추가
        rb.isKinematic = false;
        playerCollider.enabled = true;
    }
}
