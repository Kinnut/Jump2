using UnityEngine;

public class PlayerChaser : EnemyBase
{
    public float attackRange = 5f; // �÷��̾� ���� ����
    public float attackDamage = 10f; // �÷��̾ �ִ� ����
    public float attackCooldown = 1f; // ���� ��Ÿ��
    public GameObject bulletPrefab; // �߻��� �Ѿ� ������
    public Transform firePoint; // �Ѿ��� �߻�� ��ġ

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
                Vector2 direction = (player.position - firePoint.position).normalized; // �÷��̾� ������ ���� ���
                bulletScript.SetDirection(direction); // ������ �Ѿ˿� ����
                bulletScript.SetDamage(attackDamage); // �Ѿ˿� ������ ����
            }
        }
    }
}
