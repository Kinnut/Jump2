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
    private CoinManager coinManager;  // 코인 매니저 참조

    public int coinReward = 10;  // 몬스터 처치 시 제공하는 코인 수
    private bool lastHitByPlayer = false;  // 마지막으로 공격한 대상이 플레이어인지 여부

    public event Action OnEnemyDestroyed;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        scoreManager = FindObjectOfType<ScoreManager>();
        playerUltimate = FindObjectOfType<PlayerUltimate>();
        coinManager = FindObjectOfType<CoinManager>();  // 코인 매니저를 찾음
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        lastHitByPlayer = false;  // 초기화
    }

    protected virtual void Update()
    {
        // 기본적인 적의 업데이트 로직을 작성하거나, 자식 클래스에서 재정의
    }

    public void TakeDamage(float damage, bool isPlayer)
    {
        currentHealth -= damage;

        if (isPlayer)
        {
            lastHitByPlayer = true;  // 플레이어가 마지막으로 공격했음을 기록
            playerUltimate.OnHitEnemy();  // 플레이어 궁극기 게이지 충전
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

            // 코인 획득 로직 추가
            if (coinManager != null)
            {
                coinManager.AddCoins(coinReward);  // 플레이어가 처치했을 때만 코인 추가
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
