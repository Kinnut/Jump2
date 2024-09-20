using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Experimental.GlobalIllumination;
using System;

public class Crystal : MonoBehaviour
{
    public float maxHealth = 1000f;         // ũ����Ż�� �ִ� ü��
    private float currentHealth;           // ũ����Ż�� ���� ü��
    public Image healthBar;                // ü�� fillamount �̹���
    public TextMeshProUGUI healthPercent;  // ü�� %

    public float interactionRange = 3f;               // ��ȣ�ۿ� ������ �Ÿ�
    public GameObject shopUI;                         // ���� UI
    public TextMeshProUGUI interactionText;           // ��ȣ�ۿ� �ؽ�Ʈ UI
    private Transform player;                         // �÷��̾� ��ġ ����
    private bool isPlayerInRange = false;             // �÷��̾ ��ȣ�ۿ� ������ �ִ��� ����

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;         // ���� �� ü���� �ִ� ������ ����
        UpdateTextUI();
        UpdateHealthUI();                  // ü�� UI ������Ʈ
    }

    private void Update()
    {
        OpenShopDistance();
    }

    // ũ����Ż�� �������� ���� �� ȣ��Ǵ� �޼���
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;           // ü�� ����
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);  // ü���� 0 �Ʒ��� �������� �ʵ��� ó��
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

    void OpenShopDistance()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // �÷��̾ ��ȣ�ۿ� ���� �ȿ� ������
        if (distanceToPlayer <= interactionRange)
        {
            if (!isPlayerInRange)
            {
                interactionText.gameObject.SetActive(true); // ��ȣ�ۿ� �ؽ�Ʈ ǥ��
                isPlayerInRange = true;
            }

            // FŰ�� ������ �� ���� UI Ȱ��ȭ
            if (Input.GetKeyDown(KeyCode.F))
            {
                OpenShop();
            }
        }
        else if (isPlayerInRange)
        {
            // ��ȣ�ۿ� ������ ����� �ؽ�Ʈ �����
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
