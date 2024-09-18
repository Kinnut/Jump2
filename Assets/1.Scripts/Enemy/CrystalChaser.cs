using UnityEngine;

public class CrystalChaser : EnemyBase
{
    public float explosionRange = 3f; // ũ����Ż���� ���� ����
    public float damage = 50f; // ũ����Ż�� �ִ� ����
    private Transform crystal;
    private Crystal crystalScript; // Crystal ��ũ��Ʈ ����
    private bool hasExploded = false; // ���� ���� üũ

    protected override void Start()
    {
        base.Start();
        crystal = GameObject.FindGameObjectWithTag("Crystal").transform;
        if (crystal != null)
        {
            crystalScript = crystal.GetComponent<Crystal>(); // Crystal ��ũ��Ʈ ��������
        }
    }

    protected override void Update()
    {
        if (!hasExploded) // �������� �ʾҴٸ� �����Ӱ� ���� üũ
        {
            Move();
            CheckForExplosion();
        }
    }

    protected override void Move()
    {
        if (crystal != null)
        {
            float distanceToCrystal = Vector2.Distance(transform.position, crystal.position);
            if (distanceToCrystal > explosionRange)
            {
                Vector2 direction = (crystal.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, crystal.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    private void CheckForExplosion()
    {
        float distanceToCrystal = Vector2.Distance(transform.position, crystal.position);
        if (distanceToCrystal <= explosionRange)
        {
            Explode();
        }
    }

    private void Explode()
    {
        // ũ����Ż�� ����
        if (crystalScript != null)
        {
            crystalScript.TakeDamage(damage); // ũ����Ż ��ũ��Ʈ�� TakeDamage ȣ��
        }

        hasExploded = true; // ���� ó��
        Die(); // �⺻ ��� ó���� ����
    }
}
