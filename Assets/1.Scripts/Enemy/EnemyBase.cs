using System;
using UnityEngine;
using Photon.Pun;

public abstract class EnemyBase : MonoBehaviourPun
{
    public float maxHealth = 100f;
    protected float currentHealth;
    public float moveSpeed = 2f;
    public GameObject[] itemPrefabs;
    private PlayerUltimate playerUltimate;
    private CoinManager coinManager;  // 코인 매니저 참조

    public int coinReward = 10;  // 몬스터 처치 시 제공하는 코인 수
    private bool lastHitByPlayer = false;  // 마지막으로 공격한 대상이 플레이어인지 여부

    public event Action OnEnemyDestroyed;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
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

    [PunRPC]
    public void TakeDamage(float damage, bool isPlayer)
    {
        currentHealth -= damage;

        if (isPlayer)
        {
            lastHitByPlayer = true;  // 플레이어가 마지막으로 공격했음을 기록
            playerUltimate.OnHitEnemy();
        }

        if (currentHealth <= 0)
        {
            // 마스터 클라이언트에서만 Die RPC를 호출
            photonView.RPC("Die", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    protected virtual void Die()
    {
        if (lastHitByPlayer)
        {
            playerUltimate.OnKillEnemy();

            if (coinManager != null)
            {
                coinManager.AddCoins(coinReward);
            }
        }

        DropItem();
        OnEnemyDestroyed?.Invoke();  // 이벤트 호출
        gameObject.SetActive(false);
    }


    [PunRPC]
    protected void DropItem()
    {
        // 아이템 드롭 동기화
        float dropChance = UnityEngine.Random.Range(0f, 1f);
        if (dropChance <= 0.05f)
        {
            int randomIndex = UnityEngine.Random.Range(0, itemPrefabs.Length);
            PhotonNetwork.Instantiate(itemPrefabs[randomIndex].name, transform.position, Quaternion.identity);
        }
    }

    protected abstract void Move();
}
