using UnityEngine;
using System;  // �̺�Ʈ ����� ���� System ���ӽ����̽� �߰�

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;           // ���� �ִ� ü��
    private float currentHealth;             // ���� ���� ü��
    public float moveSpeed = 2f;             // ���� �̵� �ӵ�
    public float stoppingDistance = 5f;      // �÷��̾���� �ּ� �Ÿ� (�� �Ÿ� �ȿ� ������ ����)
    public float fireRate = 2f;              // �Ѿ� �߻� ���� (�� ����)
    public float bulletSpeed = 10f;          // �Ѿ� �ӵ�
    public GameObject bulletPrefab;          // �߻��� �Ѿ� ������
    public Transform firePoint;              // �Ѿ��� �߻�� ��ġ
    public GameObject[] itemPrefabs;         // ��ӵ� ������ ������ �迭

    private ScoreManager scoreManager;       // ���� ���� ��ũ��Ʈ
    private Transform player;                // �÷��̾��� ��ġ
    private float nextFireTime = 0f;         // ���� �߻� ���� �ð�

    // ���� ����� �� �߻��� �̺�Ʈ
    public event Action OnEnemyDestroyed;    // ���� �ı��Ǿ��� �� �߻��ϴ� �̺�Ʈ

    void Start()
    {
        // ���� ���� �� �÷��̾� ��ġ�� ScoreManager�� ã�Ƽ� ����
        player = GameObject.FindGameObjectWithTag("Player").transform;
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void OnEnable()
    {
        currentHealth = maxHealth;  // ���� Ȱ��ȭ�� ������ ü���� �ִ� ü������ ����
    }

    void Update()
    {
        // �÷��̾ �����ϴ� ������ �߻� ���� ����
        FollowPlayer();

        // �߻� �ֱ�(fireRate)���� �÷��̾�� ���� �߻�
        if (Time.time >= nextFireTime)
        {
            ShootPlayer();
            nextFireTime = Time.time + fireRate;
        }
    }

    // �÷��̾ �����ϴ� �Լ�
    void FollowPlayer()
    {
        if (player != null)
        {
            // �÷��̾�� �� ������ �Ÿ� ���
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // �÷��̾�� ���� �Ÿ� �̻��� ���� ����
            if (distanceToPlayer > stoppingDistance)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    // �÷��̾ ���� ���� �߻��ϴ� �Լ�
    void ShootPlayer()
    {
        if (player != null)
        {
            // �Ѿ� ����
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // �Ѿ� �߻� ���� ���� (�÷��̾ ����)
            Vector2 direction = (player.position - firePoint.position).normalized;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;  // �Ѿ˿� �ӵ� ����
            }
        }
    }

    // ���� �������� �޾��� �� ȣ��Ǵ� �Լ�
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // ü���� 0 ������ �� ���� ���
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ���� ������� �� ȣ��Ǵ� �Լ�
    void Die()
    {
        scoreManager.AddKillScore();  // ���� ���� �� ���� �߰�
        DropItem();                   // ������ ��� �õ�

        // �� ��� �̺�Ʈ ȣ��
        if (OnEnemyDestroyed != null)
        {
            Debug.Log("���� ���������, OnEnemyDestroyed �̺�Ʈ ȣ��");
            OnEnemyDestroyed.Invoke();  // �̺�Ʈ ȣ��
        }
        else
        {
            Debug.LogWarning("OnEnemyDestroyed �̺�Ʈ�� ������� �ʾҽ��ϴ�.");
        }

        gameObject.SetActive(false);  // �� ��Ȱ��ȭ (������Ʈ Ǯ�� ��� ��)
    }


    // 20% Ȯ���� ������ ���
    void DropItem()
    {
        Debug.Log("������ ��� �õ�");
        float dropChance = UnityEngine.Random.Range(0f, 1f);  // 0���� 1 ������ ���� �� ����

        if (dropChance <= 0.2f)  // 20% Ȯ���� ������ ���
        {
            int randomIndex = UnityEngine.Random.Range(0, itemPrefabs.Length);  // ������ ������ �� �ϳ��� ���� ����
            Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);
            Debug.Log("������ ��ӵ�: " + itemPrefabs[randomIndex].name);
        }
    }
}
