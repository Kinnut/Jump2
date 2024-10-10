using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;  // 씬 관리 추가
using System.Collections.Generic;

public class Crystal : MonoBehaviourPun
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
        if (PhotonNetwork.IsMasterClient)
        {
            currentHealth -= damage;           // 체력 감소
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // 체력이 0 아래로 내려가지 않도록 처리
            photonView.RPC("UpdateHealth", RpcTarget.All, currentHealth);  // 모든 클라이언트에게 체력 동기화

            // 체력이 0이 되면 모든 플레이어를 엔딩 씬으로 이동
            if (currentHealth <= 0)
            {
                photonView.RPC("TriggerGameEnd", RpcTarget.All);  // 모든 클라이언트에 엔딩 씬 이동 명령
            }
        }
    }

    // 모든 클라이언트에서 체력 UI 업데이트 (RPC로 동기화)
    [PunRPC]
    public void UpdateHealth(float health)
    {
        currentHealth = health;
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
    private void CheckPlayersInRange()
    {
        // 모든 플레이어를 태그를 통해 찾습니다. 플레이어에게 "Player" 태그가 있다고 가정
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerObj in players)
        {
            if (playerObj != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, playerObj.transform.position);

                // 현재 플레이어가 로컬 클라이언트인지 확인 (PhotonView 사용)
                PhotonView playerPhotonView = playerObj.GetComponent<PhotonView>();
                if (playerPhotonView != null && playerPhotonView.IsMine)
                {
                    // 플레이어가 상호작용 범위 안에 들어온 경우
                    if (distanceToPlayer <= interactionRange)
                    {
                        if (!isPlayerInRange)
                        {
                            interactionText.gameObject.SetActive(true); // 상호작용 텍스트 표시
                            isPlayerInRange = true;
                        }

                        // F키를 눌렀을 때 상점 UI 활성화 (로컬 플레이어일 때만)
                        if (Input.GetKeyDown(KeyCode.F))
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
    }

    private void OpenShop()
    {
        shopUI.gameObject.SetActive(true);
        interactionText.gameObject.SetActive(false);
    }

    // 모든 플레이어에게 엔딩 씬 이동 명령
    [PunRPC]
    void TriggerGameEnd()
    {
        // 엔딩 씬으로 이동 (씬 번호 또는 이름을 사용)
        SceneManager.LoadScene("2.EndingScene");
    }
}
