using UnityEngine;
using System;  // 이벤트 사용을 위해 System 네임스페이스 추가

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;           // 적의 최대 체력
    private float currentHealth;             // 적의 현재 체력
    public float moveSpeed = 2f;             // 적의 이동 속도
    public float stoppingDistance = 5f;      // 플레이어와의 최소 거리 (이 거리 안에 있으면 멈춤)
    public float fireRate = 2f;              // 총알 발사 간격 (초 단위)
    public float bulletSpeed = 10f;          // 총알 속도
    public GameObject bulletPrefab;          // 발사할 총알 프리팹
    public Transform firePoint;              // 총알이 발사될 위치
    public GameObject[] itemPrefabs;         // 드롭될 아이템 프리팹 배열

    private ScoreManager scoreManager;       // 점수 관리 스크립트
    private Transform player;                // 플레이어의 위치
    private float nextFireTime = 0f;         // 다음 발사 가능 시간

    // 적이 사망할 때 발생할 이벤트
    public event Action OnEnemyDestroyed;    // 적이 파괴되었을 때 발생하는 이벤트

    void Start()
    {
        // 게임 시작 시 플레이어 위치와 ScoreManager를 찾아서 설정
        player = GameObject.FindGameObjectWithTag("Player").transform;
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void OnEnable()
    {
        currentHealth = maxHealth;  // 적이 활성화될 때마다 체력을 최대 체력으로 설정
    }

    void Update()
    {
        // 플레이어를 추적하는 로직과 발사 로직 실행
        FollowPlayer();

        // 발사 주기(fireRate)마다 플레이어에게 총을 발사
        if (Time.time >= nextFireTime)
        {
            ShootPlayer();
            nextFireTime = Time.time + fireRate;
        }
    }

    // 플레이어를 추적하는 함수
    void FollowPlayer()
    {
        if (player != null)
        {
            // 플레이어와 적 사이의 거리 계산
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // 플레이어와 일정 거리 이상일 때만 추적
            if (distanceToPlayer > stoppingDistance)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    // 플레이어를 향해 총을 발사하는 함수
    void ShootPlayer()
    {
        if (player != null)
        {
            // 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // 총알 발사 방향 설정 (플레이어를 향함)
            Vector2 direction = (player.position - firePoint.position).normalized;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;  // 총알에 속도 적용
            }
        }
    }

    // 적이 데미지를 받았을 때 호출되는 함수
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // 체력이 0 이하일 때 적이 사망
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 적이 사망했을 때 호출되는 함수
    void Die()
    {
        scoreManager.AddKillScore();  // 적이 죽을 때 점수 추가
        DropItem();                   // 아이템 드롭 시도

        // 적 사망 이벤트 호출
        if (OnEnemyDestroyed != null)
        {
            Debug.Log("적이 사망했으며, OnEnemyDestroyed 이벤트 호출");
            OnEnemyDestroyed.Invoke();  // 이벤트 호출
        }
        else
        {
            Debug.LogWarning("OnEnemyDestroyed 이벤트가 연결되지 않았습니다.");
        }

        gameObject.SetActive(false);  // 적 비활성화 (오브젝트 풀링 사용 시)
    }


    // 20% 확률로 아이템 드롭
    void DropItem()
    {
        Debug.Log("아이템 드롭 시도");
        float dropChance = UnityEngine.Random.Range(0f, 1f);  // 0에서 1 사이의 랜덤 값 생성

        if (dropChance <= 0.2f)  // 20% 확률로 아이템 드롭
        {
            int randomIndex = UnityEngine.Random.Range(0, itemPrefabs.Length);  // 아이템 프리팹 중 하나를 랜덤 선택
            Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);
            Debug.Log("아이템 드롭됨: " + itemPrefabs[randomIndex].name);
        }
    }
}
