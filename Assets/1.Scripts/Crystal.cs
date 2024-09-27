using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections.Generic;

public class Crystal : MonoBehaviour
{
    public float maxHealth = 1000f;         // 크리스탈의 최대 체력
    private float currentHealth;            // 크리스탈의 현재 체력
    public Image healthBar;                 // 체력 fillamount 이미지
    public TextMeshProUGUI healthPercent;   // 체력 %

    public float interactionRange = 3f;     // 상호작용 가능한 거리
    public GameObject shopUI;               // 상점 UI
    public TextMeshProUGUI interactionText; // 상호작용 텍스트 UI
    private bool isPlayerInRange = false;   // 플레이어가 상호작용 범위에 있는지 여부

    private List<Transform> playersInRange = new List<Transform>(); // 상호작용 범위 내의 플레이어들

    void Start()
    {
        currentHealth = maxHealth;         // 시작 시 체력을 최대 값으로 설정
        UpdateTextUI();
        UpdateHealthUI();                  // 체력 UI 업데이트
    }

    private void Update()
    {
        CheckPlayersInRange();
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

    // 범위 내 플레이어를 확인하고 상점 UI를 띄우는 함수
    void CheckPlayersInRange()
    {
        // 현재 게임 내 모든 플레이어를 추적
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            // 각 플레이어의 위치를 가져와 크리스탈과의 거리를 계산
            GameObject playerObj = PhotonView.Find(player.ActorNumber)?.gameObject;
            if (playerObj != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, playerObj.transform.position);

                // 플레이어가 상호작용 범위 안에 들어온 경우
                if (distanceToPlayer <= interactionRange)
                {
                    if (!isPlayerInRange)
                    {
                        interactionText.gameObject.SetActive(true); // 상호작용 텍스트 표시
                        isPlayerInRange = true;
                    }

                    // F키를 눌렀을 때 상점 UI 활성화
                    if (Input.GetKeyDown(KeyCode.F) && playerObj.GetComponent<PhotonView>().IsMine)
                    {
                        OpenShop();
                    }
                }
                else
                {
                    // 상호작용 범위를 벗어나면 텍스트 숨기기
                    interactionText.gameObject.SetActive(false);
                    isPlayerInRange = false;
                }
            }
        }
    }

    private void OpenShop()
    {
        shopUI.gameObject.SetActive(true);
        interactionText.gameObject.SetActive(false);
    }
}
