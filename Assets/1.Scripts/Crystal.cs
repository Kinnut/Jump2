using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;  // �� ���� �߰�
using System.Collections.Generic;

public class Crystal : MonoBehaviourPun
{
    public float maxHealth = 1000f;         // ũ����Ż�� �ִ� ü��
    private float currentHealth;            // ũ����Ż�� ���� ü��
    public Image healthBar;                 // ü�� fillamount �̹���
    public TextMeshProUGUI healthPercent;   // ü�� %

    public float interactionRange = 3f;     // ��ȣ�ۿ� ������ �Ÿ�
    public GameObject shopUI;               // ���� UI
    public TextMeshProUGUI interactionText; // ��ȣ�ۿ� �ؽ�Ʈ UI
    private bool isPlayerInRange = false;   // �÷��̾ ��ȣ�ۿ� ������ �ִ��� ����

    private List<Transform> playersInRange = new List<Transform>(); // ��ȣ�ۿ� ���� ���� �÷��̾��

    void Start()
    {
        currentHealth = maxHealth;         // ���� �� ü���� �ִ� ������ ����
        UpdateTextUI();
        UpdateHealthUI();                  // ü�� UI ������Ʈ
    }

    private void Update()
    {
        CheckPlayersInRange();
    }

    // ũ����Ż�� �������� ���� �� ȣ��Ǵ� �޼���
    public void TakeDamage(float damage)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentHealth -= damage;           // ü�� ����
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // ü���� 0 �Ʒ��� �������� �ʵ��� ó��
            photonView.RPC("UpdateHealth", RpcTarget.All, currentHealth);  // ��� Ŭ���̾�Ʈ���� ü�� ����ȭ

            // ü���� 0�� �Ǹ� ��� �÷��̾ ���� ������ �̵�
            if (currentHealth <= 0)
            {
                photonView.RPC("TriggerGameEnd", RpcTarget.All);  // ��� Ŭ���̾�Ʈ�� ���� �� �̵� ���
            }
        }
    }

    // ��� Ŭ���̾�Ʈ���� ü�� UI ������Ʈ (RPC�� ����ȭ)
    [PunRPC]
    public void UpdateHealth(float health)
    {
        currentHealth = health;
        UpdateHealthUI();                  // ü�� UI ������Ʈ
        UpdateTextUI();
    }

    // ü�� UI ������Ʈ
    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;  // ü�¿� ����� FillAmount ����
        }
    }

    void UpdateTextUI()
    {
        healthPercent.text = $"ũ����Ż ü�� : {currentHealth}%";
    }

    // ���� �� �÷��̾ Ȯ���ϰ� ���� UI�� ���� �Լ�
    private void CheckPlayersInRange()
    {
        // ��� �÷��̾ �±׸� ���� ã���ϴ�. �÷��̾�� "Player" �±װ� �ִٰ� ����
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject playerObj in players)
        {
            if (playerObj != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, playerObj.transform.position);

                // ���� �÷��̾ ���� Ŭ���̾�Ʈ���� Ȯ�� (PhotonView ���)
                PhotonView playerPhotonView = playerObj.GetComponent<PhotonView>();
                if (playerPhotonView != null && playerPhotonView.IsMine)
                {
                    // �÷��̾ ��ȣ�ۿ� ���� �ȿ� ���� ���
                    if (distanceToPlayer <= interactionRange)
                    {
                        if (!isPlayerInRange)
                        {
                            interactionText.gameObject.SetActive(true); // ��ȣ�ۿ� �ؽ�Ʈ ǥ��
                            isPlayerInRange = true;
                        }

                        // FŰ�� ������ �� ���� UI Ȱ��ȭ (���� �÷��̾��� ����)
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            OpenShop();
                        }
                    }
                    else
                    {
                        // ��ȣ�ۿ� ������ ����� �ؽ�Ʈ �����
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

    // ��� �÷��̾�� ���� �� �̵� ���
    [PunRPC]
    void TriggerGameEnd()
    {
        // ���� ������ �̵� (�� ��ȣ �Ǵ� �̸��� ���)
        SceneManager.LoadScene("2.EndingScene");
    }
}
