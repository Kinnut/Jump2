using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public float health = 100f;                    // 플레이어의 체력
    public Image healthBarImage;                   // 체력바 이미지
    private float maxHealth;                       // 최대 체력

    private CinemachineImpulseSource impulseSource; // Impulse Source 참조
    public float speed = 5f;                       // 기본 이동 속도
    private float defaultSpeed;                    // 기본 속도 저장
    private bool isBoosted = false;                // 속도 부스트 여부

    private PlayerShooting playerShooting;         // PlayerShooting 스크립트 참조
    public ScoreManager scoreManager;              // ScoreManager 참조 (점수 관리)

    private float playTime = 0f;                   // 플레이 타임 기록

    // 사망 UI 및 카운트다운 관련 변수
    public GameObject deathUI;                     // 사망 UI 패널
    public TextMeshProUGUI countdownText;          // 카운트다운 텍스트 (TMP)
    private float respawnTime = 10f;               // 부활까지의 시간
    private bool isDead = false;                   // 플레이어 사망 상태 여부

    private Rigidbody2D rb;                        // 플레이어 Rigidbody2D
    private Collider2D playerCollider;             // 플레이어 충돌체

    void Start()
    {
        maxHealth = health;                        // 최대 체력 설정
        defaultSpeed = speed;                      // 기본 속도 저장
        UpdateHealthBar();                         // 체력바 업데이트
        impulseSource = GetComponent<CinemachineImpulseSource>();  // Impulse Source 설정
        playerShooting = GetComponent<PlayerShooting>();           // PlayerShooting 스크립트 참조
        rb = GetComponent<Rigidbody2D>();          // Rigidbody2D 참조
        playerCollider = GetComponent<Collider2D>(); // Collider2D 참조

        deathUI.SetActive(false);                  // 시작 시 사망 UI 비활성화
    }

    void Update()
    {
        playTime += Time.deltaTime;                // 플레이 타임 측정

        if (isDead && respawnTime > 0)
        {
            respawnTime -= Time.deltaTime;
            countdownText.text = "RESPAWN : " + Mathf.Ceil(respawnTime).ToString();  // 남은 시간을 CoolTime 형식으로 표시

            if (respawnTime <= 0)
            {
                Respawn();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // 사망 중에는 데미지를 받지 않음

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
        if (isDead) return; // 사망 중에는 회복 불가

        health += amount;

        health = Mathf.Clamp(health, 0, maxHealth);

        UpdateHealthBar();
        Debug.Log("플레이어 체력 회복: " + health);
    }

    public void StrengthenBasicAttack()
    {
        playerShooting.ChangeBasicBulletPrefab();  // 기본 총알 프리팹 변경
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

        // 사망 UI 활성화 및 카운트다운 초기화
        deathUI.SetActive(true);
        respawnTime = 10f;  // 10초로 초기화

        // 플레이어 이동 및 공격 비활성화
        DisablePlayer();
    }

    // 플레이어 부활 처리
    void Respawn()
    {
        Debug.Log("플레이어 부활");
        isDead = false;
        health = maxHealth;      // 체력 회복
        UpdateHealthBar();       // 체력바 업데이트

        // 플레이어 이동 및 공격 활성화
        EnablePlayer();

        deathUI.SetActive(false); // 사망 UI 비활성화
    }

    // 플레이어 비활성화
    void DisablePlayer()
    {
        playerShooting.enabled = false;   // 플레이어 슈팅 비활성화
        rb.velocity = Vector2.zero;       // 플레이어 이동 중지
        rb.isKinematic = true;            // Rigidbody의 물리 반응 비활성화
        playerCollider.enabled = false;   // 충돌 비활성화
    }

    // 플레이어 활성화
    void EnablePlayer()
    {
        playerShooting.enabled = true;    // 플레이어 슈팅 활성화
        rb.isKinematic = false;           // Rigidbody의 물리 반응 활성화
        playerCollider.enabled = true;    // 충돌 활성화
    }
}
