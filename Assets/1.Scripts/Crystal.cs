using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Crystal : MonoBehaviour
{
    public float maxHealth = 1000f;         // 크리스탈의 최대 체력
    private float currentHealth;           // 크리스탈의 현재 체력
    public Image healthBar;                // 체력 fillamount 이미지
    public TextMeshProUGUI healthPercent;  // 체력 %

    void Start()
    {
        currentHealth = maxHealth;         // 시작 시 체력을 최대 값으로 설정
        UpdateTextUI();
        UpdateHealthUI();                  // 체력 UI 업데이트
    }

    // 크리스탈이 데미지를 받을 때 호출되는 메서드
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;           // 체력 감소
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // 체력이 0 아래로 내려가지 않도록 처리
        UpdateHealthUI();                  // 체력 UI 업데이트
        UpdateTextUI();
    }

    // 체력 UI 업데이트
    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;  // 체력에 비례해 FillAmount 조정
        }
    }

    void UpdateTextUI()
    {
        healthPercent.text = $"크리스탈 체력 : {currentHealth}%";
    }
}
