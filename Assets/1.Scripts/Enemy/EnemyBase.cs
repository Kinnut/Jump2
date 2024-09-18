using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public float maxHealth = 100f;
    protected float currentHealth;
    public float moveSpeed = 2f;
    public GameObject[] itemPrefabs;
    private ScoreManager scoreManager;

    public event Action OnEnemyDestroyed;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
    }

    // ���⿡�� virtual�� Update �޼��� ����
    protected virtual void Update()
    {
        // �⺻���� ���� ������Ʈ ������ �ۼ��ϰų�, �ڽ� Ŭ�������� �������� �� �ְ� �մϴ�.
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        scoreManager.AddKillScore();
        DropItem();
        OnEnemyDestroyed?.Invoke();
        gameObject.SetActive(false); // ������Ʈ Ǯ�� ���
    }

    protected void DropItem()
    {
        float dropChance = UnityEngine.Random.Range(0f, 1f);
        if (dropChance <= 0.2f)
        {
            int randomIndex = UnityEngine.Random.Range(0, itemPrefabs.Length);
            Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }

    protected abstract void Move();
}
