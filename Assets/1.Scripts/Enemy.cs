using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;  // 최대 체력
    private float currentHealth;    // 현재 체력
    public float moveSpeed = 2f;           // 적의 이동 속도
    public float stoppingDistance = 5f;    // 플레이어와 유지할 거리
    public float fireRate = 2f;            // 총을 쏘는 주기 (2초에 한 번)
    public float bulletSpeed = 10f;        // 총알 속도
    public string poolTag; // ObjectPooler에 등록된 몬스터의 태그
    private ObjectPooler objectPooler;
    public GameObject bulletPrefab;        // 발사할 총알 프리팹
    public Transform firePoint;            // 총알이 발사될 위치

    private Transform player;              // 플레이어의 Transform
    private float nextFireTime = 0f;       // 다음 총알 발사 시간

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어를 찾음 (Player 태그 사용)
    }

    void OnEnable()
    {
        currentHealth = maxHealth;  // 적이 활성화될 때마다 체력 초기화
    }

    void Update()
    {
        // 플레이어를 추적하고 일정 거리를 유지함
        FollowPlayer();

        // 2초마다 총을 발사
        if (Time.time >= nextFireTime)
        {
            ShootPlayer();
            nextFireTime = Time.time + fireRate; // 다음 발사 시간 설정
        }
    }

    void FollowPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position); // 플레이어와의 거리 계산

            // 플레이어와의 거리가 유지할 거리보다 크면 계속 따라감
            if (distanceToPlayer > stoppingDistance)
            {
                Vector2 direction = (player.position - transform.position).normalized; // 플레이어 방향 계산
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime); // 플레이어 쪽으로 이동
            }
        }
    }

    void ShootPlayer()
    {
        if (player != null)
        {
            // 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // 플레이어 방향으로 총알 발사
            Vector2 direction = (player.position - firePoint.position).normalized;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed; // 플레이어를 향해 총알 발사
            }

            Debug.Log("적이 플레이어에게 총알 발사");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // 데미지 감소
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}
