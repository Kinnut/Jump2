using UnityEngine;

public class PlayerChaser : EnemyBase
{
    public float attackRange = 2f; // �÷��̾� ���� ����
    public float attackDamage = 10f; // �÷��̾ �ִ� ����
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
        Debug.Log("�÷��̾�� ����!");
        // �÷��̾�� ���ظ� �ִ� ���� ���� �ʿ�
    }
}
