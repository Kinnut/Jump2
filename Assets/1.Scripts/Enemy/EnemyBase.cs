using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public float maxHealth = 100f;
    protected float currentHealth;
    public float moveSpeed = 2f;
    public GameObject[] itemPrefabs;
    private ScoreManager scoreManager;
    private PlayerUltimate playerUltimate;
    private CoinManager coinManager;  // ���� �Ŵ��� ����

    public int coinReward = 10;  // ���� óġ �� �����ϴ� ���� ��
    private bool lastHitByPlayer = false;  // ���������� ������ ����� �÷��̾����� ����

    public event Action OnEnemyDestroyed;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        scoreManager = FindObjectOfType<ScoreManager>();
        playerUltimate = FindObjectOfType<PlayerUltimate>();
        coinManager = FindObjectOfType<CoinManager>();  // ���� �Ŵ����� ã��
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        lastHitByPlayer = false;  // �ʱ�ȭ
    }

    protected virtual void Update()
    {
        // �⺻���� ���� ������Ʈ ������ �ۼ��ϰų�, �ڽ� Ŭ�������� ������
    }

    public void TakeDamage(float damage, bool isPlayer)
    {
        currentHealth -= damage;

        if (isPlayer)
        {
            lastHitByPlayer = true;  // �÷��̾ ���������� ���������� ���
            playerUltimate.OnHitEnemy();  // �÷��̾� �ñر� ������ ����
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (lastHitByPlayer)
        {
            playerUltimate.OnKillEnemy();

            // ���� ȹ�� ���� �߰�
            if (coinManager != null)
            {
                coinManager.AddCoins(coinReward);  // �÷��̾ óġ���� ���� ���� �߰�
            }
        }

        DropItem();
        OnEnemyDestroyed?.Invoke();
        gameObject.SetActive(false);
    }

    protected void DropItem()
    {
        float dropChance = UnityEngine.Random.Range(0f, 1f);
        if (dropChance <= 0.05f)
        {
            int randomIndex = UnityEngine.Random.Range(0, itemPrefabs.Length);
            Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }

    protected abstract void Move();
}
