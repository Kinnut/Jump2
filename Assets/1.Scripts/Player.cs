using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float health = 100f;                    // �÷��̾��� ü��
    public Image healthBarImage;                   // ü�¹� �̹���
    private float maxHealth;                       // �ִ� ü��

    private CinemachineImpulseSource impulseSource; // Impulse Source ����

    void Start()
    {
        maxHealth = health;                        // �ִ� ü�� ����
        UpdateHealthBar();                         // ü�¹� ������Ʈ
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage; // ������ ����

        // ü���� 0 ���Ϸ� �������� �ʵ��� ����
        if (health < 0)
        {
            health = 0;
        }

        // ī�޶� ��鸲 Ʈ����
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();  // ī�޶� ��鸲 �߻�
        }

        UpdateHealthBar(); // ü�¹� ������Ʈ

        if (health <= 0)
        {
            Die();  // �÷��̾� ��� ó��
        }
    }

    void UpdateHealthBar()
    {
        healthBarImage.fillAmount = health / maxHealth;  // ü�¹� ������Ʈ
    }

    void Die()
    {
        Debug.Log("�÷��̾� ���");
        // ��� ó�� ����
    }
}
