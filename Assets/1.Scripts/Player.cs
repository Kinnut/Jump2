using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float health = 100f;                    // 플레이어의 체력
    public Image healthBarImage;                   // 체력바 이미지
    private float maxHealth;                       // 최대 체력

    private CinemachineImpulseSource impulseSource; // Impulse Source 참조

    void Start()
    {
        maxHealth = health;                        // 최대 체력 설정
        UpdateHealthBar();                         // 체력바 업데이트
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage; // 데미지 적용

        // 체력이 0 이하로 내려가지 않도록 설정
        if (health < 0)
        {
            health = 0;
        }

        // 카메라 흔들림 트리거
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();  // 카메라 흔들림 발생
        }

        UpdateHealthBar(); // 체력바 업데이트

        if (health <= 0)
        {
            Die();  // 플레이어 사망 처리
        }
    }

    void UpdateHealthBar()
    {
        healthBarImage.fillAmount = health / maxHealth;  // 체력바 업데이트
    }

    void Die()
    {
        Debug.Log("플레이어 사망");
        // 사망 처리 로직
    }
}
