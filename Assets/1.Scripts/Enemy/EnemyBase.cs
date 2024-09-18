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

    public event Action OnEnemyDestroyed;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        scoreManager = FindObjectOfType<ScoreManager>();
        playerUltimate = FindObjectOfType<PlayerUltimate>();
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
    }

    // 여기에서 virtual로 Update 메서드 정의
    protected virtual void Update()
    {
        // 기본적인 적의 업데이트 로직을 작성하거나, 자식 클래스에서 재정의할 수 있게 합니다.
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        playerUltimate.OnHitEnemy();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        playerUltimate.OnKillEnemy();
        scoreManager.AddKillScore();
        DropItem();
        OnEnemyDestroyed?.Invoke();
        gameObject.SetActive(false); // 오브젝트 풀링 사용
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
