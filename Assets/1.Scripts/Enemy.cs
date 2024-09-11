using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;  // �ִ� ü��
    private float currentHealth;    // ���� ü��
    public float moveSpeed = 2f;           // ���� �̵� �ӵ�
    public float stoppingDistance = 5f;    // �÷��̾�� ������ �Ÿ�
    public float fireRate = 2f;            // ���� ��� �ֱ� (2�ʿ� �� ��)
    public float bulletSpeed = 10f;        // �Ѿ� �ӵ�
    public string poolTag; // ObjectPooler�� ��ϵ� ������ �±�
    private ObjectPooler objectPooler;
    public GameObject bulletPrefab;        // �߻��� �Ѿ� ������
    public Transform firePoint;            // �Ѿ��� �߻�� ��ġ

    private Transform player;              // �÷��̾��� Transform
    private float nextFireTime = 0f;       // ���� �Ѿ� �߻� �ð�

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // �÷��̾ ã�� (Player �±� ���)
    }

    void OnEnable()
    {
        currentHealth = maxHealth;  // ���� Ȱ��ȭ�� ������ ü�� �ʱ�ȭ
    }

    void Update()
    {
        // �÷��̾ �����ϰ� ���� �Ÿ��� ������
        FollowPlayer();

        // 2�ʸ��� ���� �߻�
        if (Time.time >= nextFireTime)
        {
            ShootPlayer();
            nextFireTime = Time.time + fireRate; // ���� �߻� �ð� ����
        }
    }

    void FollowPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position); // �÷��̾���� �Ÿ� ���

            // �÷��̾���� �Ÿ��� ������ �Ÿ����� ũ�� ��� ����
            if (distanceToPlayer > stoppingDistance)
            {
                Vector2 direction = (player.position - transform.position).normalized; // �÷��̾� ���� ���
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime); // �÷��̾� ������ �̵�
            }
        }
    }

    void ShootPlayer()
    {
        if (player != null)
        {
            // �Ѿ� ����
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // �÷��̾� �������� �Ѿ� �߻�
            Vector2 direction = (player.position - firePoint.position).normalized;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed; // �÷��̾ ���� �Ѿ� �߻�
            }

            Debug.Log("���� �÷��̾�� �Ѿ� �߻�");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // ������ ����
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
