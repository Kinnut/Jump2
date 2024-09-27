using UnityEngine;
using Photon.Pun;
using System.Linq;

public class PlayerChaser : EnemyBase
{
    public float attackRange = 5f; // 플레이어 공격 범위
    public float attackDamage = 10f; // 플레이어에 주는 피해
    public float attackCooldown = 1f; // 공격 쿨타임
    public GameObject bulletPrefab; // 발사할 총알 프리팹
    public Transform firePoint; // 총알이 발사될 위치

    private Transform targetPlayer; // 추적할 플레이어
    private float nextAttackTime = 0f;

    protected override void Start()
    {
        base.Start();
        FindClosestPlayer(); // 가장 가까운 플레이어 찾기
    }

    protected override void Update()
    {
        if (targetPlayer == null)
        {
            FindClosestPlayer(); // 추적 중인 플레이어가 없으면 다시 찾기
        }
        else
        {
            Move();
            TryAttackPlayer();
        }
    }

    protected override void Move()
    {
        if (targetPlayer != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);
            if (distanceToPlayer > attackRange)
            {
                Vector2 direction = (targetPlayer.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    private void TryAttackPlayer()
    {
        if (targetPlayer != null && Time.time >= nextAttackTime)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);
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
                Vector2 direction = (targetPlayer.position - firePoint.position).normalized; // 플레이어 쪽으로 방향 계산
                bulletScript.SetDirection(direction); // 방향을 총알에 설정
                bulletScript.SetDamage(attackDamage); // 총알에 데미지 설정
            }
        }
    }

    // 가장 가까운 플레이어 찾기
    private void FindClosestPlayer()
    {
        // 네트워크 상에서 모든 플레이어를 가져옴
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            targetPlayer = null; // 플레이어가 없으면 null로 설정
            return;
        }

        // 가장 가까운 플레이어를 찾음
        targetPlayer = players
            .Select(p => p.transform)
            .OrderBy(t => Vector2.Distance(transform.position, t.position)) // 거리에 따라 정렬
            .FirstOrDefault();
    }
}
