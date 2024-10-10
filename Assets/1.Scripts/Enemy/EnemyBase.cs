using System;
using UnityEngine;
using Photon.Pun;
using System.Collections;

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
    private bool isDead = false;  // 적의 사망 상태 체크

    public event Action OnEnemyDestroyed;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        playerUltimate = FindObjectOfType<PlayerUltimate>();
        coinManager = FindObjectOfType<CoinManager>();
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        lastHitByPlayer = false;
        isDead = false;  // 적이 살아 있는 상태로 초기화
    }

    // 클라이언트마다 적의 체력을 줄이는 TakeDamage
    [PunRPC]
    public void TakeDamage(float damage, bool isPlayer)
    {
        if (isDead) return; // 이미 죽은 상태라면 더 이상 피해를 받지 않음

        currentHealth -= damage;  // 모든 클라이언트에서 체력 감소

        if (isPlayer)
        {
            lastHitByPlayer = true;  // 플레이어가 마지막으로 공격했음을 기록
            playerUltimate.OnHitEnemy();
        }

        if (currentHealth <= 0)
        {
            Die();  // 모든 클라이언트에서 적이 죽도록 처리
        }
    }

    protected virtual void Update()
    {
        // 기본적인 적의 업데이트 로직을 작성하거나, 자식 클래스에서 재정의
    }

    // 모든 클라이언트에서 실행되는 Die 메서드
    protected virtual void Die()
    {
        if (isDead) return;  // 이미 죽은 상태이면 리턴
        isDead = true;  // 죽은 상태로 설정

        if (lastHitByPlayer)
        {
            playerUltimate.OnKillEnemy();

            // 코인 분배는 각 클라이언트에서 처리
            photonView.RPC("DistributeCoins", RpcTarget.All);
        }

        DropItem();
        OnEnemyDestroyed?.Invoke();  // 이벤트 호출

        // 모든 클라이언트에서 적 비활성화
        photonView.RPC("DeactivateEnemy", RpcTarget.All);
    }

    // 모든 플레이어가 코인을 얻게 하는 RPC 메서드
    [PunRPC]
    public void DistributeCoins()
    {
        if (coinManager != null)
        {
            coinManager.AddCoins(coinReward);
        }
        else
        {
            Debug.LogError("CoinManager not found!");
        }
    }

    // 아이템 드롭 동기화
    [PunRPC]
    protected void DropItem()
    {
        float dropChance = UnityEngine.Random.Range(0f, 1f);
        if (dropChance <= 0.05f)
        {
            int randomIndex = UnityEngine.Random.Range(0, itemPrefabs.Length);
            PhotonNetwork.Instantiate(itemPrefabs[randomIndex].name, transform.position, Quaternion.identity);
        }
    }

    // 모든 클라이언트에서 적을 비활성화하는 RPC
    [PunRPC]
    public void DeactivateEnemy()
    {
        gameObject.SetActive(false);
    }

    protected abstract void Move();
}
