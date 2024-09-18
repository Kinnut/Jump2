using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Crystal : MonoBehaviour
{
    public float maxHealth = 1000f;         // ũ����Ż�� �ִ� ü��
    private float currentHealth;           // ũ����Ż�� ���� ü��
    public Image healthBar;                // ü�� fillamount �̹���
    public TextMeshProUGUI healthPercent;  // ü�� %

    void Start()
    {
        currentHealth = maxHealth;         // ���� �� ü���� �ִ� ������ ����
        UpdateTextUI();
        UpdateHealthUI();                  // ü�� UI ������Ʈ
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
}
