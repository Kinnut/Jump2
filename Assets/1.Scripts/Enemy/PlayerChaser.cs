using UnityEngine;

public class PlayerChaser : EnemyBase
{
    public float attackRange = 2f; // 플레이어 공격 범위
    public float attackDamage = 10f; // 플레이어에 주는 피해
    public float attackCooldown = 1f;
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
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void AttackPlayer()
    {
        Debug.Log("플레이어에게 피해!");
        // 플레이어에게 피해를 주는 로직 구현 필요
    }
}
