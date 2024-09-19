using UnityEngine;

public class PlayerChaser : EnemyBase
{
    public float attackRange = 5f; // 플레이어 공격 범위
    public float attackDamage = 10f; // 플레이어에 주는 피해
    public float attackCooldown = 1f; // 공격 쿨타임
    public GameObject bulletPrefab; // 발사할 총알 프리팹
    public Transform firePoint; // 총알이 발사될 위치

    private Transform player;
    private float nextAttackTime = 0f;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Update()
    {
        Move();
        TryAttackPlayer();
    }

    protected override void Move()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > attackRange)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    private void TryAttackPlayer()
    {
        if (player != null && Time.time >= nextAttackTime)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                ShootAtPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void ShootAtPlayer()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();

            if (bulletScript != null)
            {
                Vector2 direction = (player.position - firePoint.position).normalized; // 플레이어 쪽으로 방향 계산
                bulletScript.SetDirection(direction); // 방향을 총알에 설정
                bulletScript.SetDamage(attackDamage); // 총알에 데미지 설정
            }
        }
    }
}
