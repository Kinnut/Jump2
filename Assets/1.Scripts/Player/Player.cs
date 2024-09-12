using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float health = 100f;                    // �÷��̾��� ü��
    public Image healthBarImage;                   // ü�¹� �̹���
    private float maxHealth;                       // �ִ� ü��

    private CinemachineImpulseSource impulseSource; // Impulse Source ����
    public float speed = 5f;                       // �⺻ �̵� �ӵ�
    private float defaultSpeed;                    // �⺻ �ӵ� ����
    private bool isBoosted = false;                // �ӵ� �ν�Ʈ ����

    private PlayerShooting playerShooting;         // PlayerShooting ��ũ��Ʈ ����

    void Start()
    {

        maxHealth = health;                        // �ִ� ü�� ����
        defaultSpeed = speed;                      // �⺻ �ӵ� ����
        UpdateHealthBar();                         // ü�¹� ������Ʈ
        impulseSource = GetComponent<CinemachineImpulseSource>();  // Impulse Source ����
        playerShooting = GetComponent<PlayerShooting>();           // PlayerShooting ��ũ��Ʈ ����
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

        Debug.Log("�÷��̾� ü�� ȸ��: " + health);
    }

    // Ammo �������� �Ծ��� ��, �⺻ ������ �Ѿ� �������� ����
    public void StrengthenBasicAttack()
    {
        playerShooting.ChangeBasicBulletPrefab();  // �⺻ �Ѿ� ������ ����
        Debug.Log("�⺻ ������ ��ȭ�Ǿ����ϴ�.");
    }

    public void IncreaseSpeed(float amount, float duration)
    {
        if (!isBoosted)
        {
            speed += amount;
            isBoosted = true;
            Debug.Log("�ӵ� ����: " + amount + " for " + duration + " seconds");
            Invoke("ResetSpeed", duration);
        }
    }

    private void ResetSpeed()
    {
        speed = defaultSpeed;
        isBoosted = false;
        Debug.Log("�ӵ� ���󺹱�");
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
        Debug.Log("�÷��̾� ���");
    }
}
