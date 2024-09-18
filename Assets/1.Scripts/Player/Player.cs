using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    void Start()
    {
        maxHealth = health;                        // 최대 체력 설정
        defaultSpeed = speed;                      // 기본 속도 저장
        UpdateHealthBar();                         // 체력바 업데이트
        impulseSource = GetComponent<CinemachineImpulseSource>();  // Impulse Source 설정
        playerShooting = GetComponent<PlayerShooting>();           // PlayerShooting 스크립트 참조
    }

    void Update()
    {
        playTime += Time.deltaTime;                // 플레이 타임 측정
    }

    public void TakeDamage(float damage)
    {
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

    void Die()
    {
        Debug.Log("플레이어 사망");

        // 사망 시 점수 및 플레이 타임을 저장하고 엔딩 씬으로 이동
        PlayerPrefs.SetFloat("FinalScore", scoreManager.GetCurrentScore());
        PlayerPrefs.SetFloat("PlayTime", playTime);

        // 엔딩 씬으로 이동
        SceneManager.LoadScene("2.EndingScene");
    }
}
