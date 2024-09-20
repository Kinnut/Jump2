using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Experimental.GlobalIllumination;
using System;

public class Crystal : MonoBehaviour
{
    public float maxHealth = 1000f;         // 크리스탈의 최대 체력
    private float currentHealth;           // 크리스탈의 현재 체력
    public Image healthBar;                // 체력 fillamount 이미지
    public TextMeshProUGUI healthPercent;  // 체력 %

    public float interactionRange = 3f;               // 상호작용 가능한 거리
    public GameObject shopUI;                         // 상점 UI
    public TextMeshProUGUI interactionText;           // 상호작용 텍스트 UI
    private Transform player;                         // 플레이어 위치 참조
    private bool isPlayerInRange = false;             // 플레이어가 상호작용 범위에 있는지 여부

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;         // 시작 시 체력을 최대 값으로 설정
        UpdateTextUI();
        UpdateHealthUI();                  // 체력 UI 업데이트
    }

    private void Update()
    {
        OpenShopDistance();
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

    void OpenShopDistance()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 플레이어가 상호작용 범위 안에 들어오면
        if (distanceToPlayer <= interactionRange)
        {
            if (!isPlayerInRange)
            {
                interactionText.gameObject.SetActive(true); // 상호작용 텍스트 표시
                isPlayerInRange = true;
            }

            // F키를 눌렀을 때 상점 UI 활성화
            if (Input.GetKeyDown(KeyCode.F))
            {
                OpenShop();
            }
        }
        else if (isPlayerInRange)
        {
            // 상호작용 범위를 벗어나면 텍스트 숨기기
            interactionText.gameObject.SetActive(false);
            isPlayerInRange = false;
        }
    }

    private void OpenShop()
    {
        shopUI.gameObject.SetActive(true);
        interactionText.gameObject.SetActive(false);
    }
}
