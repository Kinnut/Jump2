using UnityEngine;
using Photon.Pun;
using System.Linq;

public class PlayerChaser : EnemyBase
{
    public float attackRange = 5f; // �÷��̾� ���� ����
    public float attackDamage = 10f; // �÷��̾ �ִ� ����
    public float attackCooldown = 1f; // ���� ��Ÿ��
    public GameObject bulletPrefab; // �߻��� �Ѿ� ������
    public Transform firePoint; // �Ѿ��� �߻�� ��ġ

    private Transform targetPlayer; // ������ �÷��̾�
    private float nextAttackTime = 0f;

    protected override void Start()
    {
        base.Start();
        FindClosestPlayer(); // ���� ����� �÷��̾� ã��
    }

    protected override void Update()
    {
        if (targetPlayer == null)
        {
            FindClosestPlayer(); // ���� ���� �÷��̾ ������ �ٽ� ã��
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
                Vector2 direction = (targetPlayer.position - firePoint.position).normalized; // �÷��̾� ������ ���� ���
                bulletScript.SetDirection(direction); // ������ �Ѿ˿� ����
                bulletScript.SetDamage(attackDamage); // �Ѿ˿� ������ ����
            }
        }
    }

    // ���� ����� �÷��̾� ã��
    private void FindClosestPlayer()
    {
        // ��Ʈ��ũ �󿡼� ��� �÷��̾ ������
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            targetPlayer = null; // �÷��̾ ������ null�� ����
            return;
        }

        // ���� ����� �÷��̾ ã��
        targetPlayer = players
            .Select(p => p.transform)
            .OrderBy(t => Vector2.Distance(transform.position, t.position)) // �Ÿ��� ���� ����
            .FirstOrDefault();
    }
}
