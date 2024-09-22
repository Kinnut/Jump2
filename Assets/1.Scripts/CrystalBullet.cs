using UnityEngine;

public class CrystalBullet : MonoBehaviour
{
    public float speed = 10f;         // �Ѿ� �ӵ�
    public float damage = 20f;        // �Ѿ� ������
    private Transform target;         // ��ǥ ���� Transform

    // ��ǥ ���� �޼���
    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;         // Ÿ�� ����
        if (target != null)
        {
            // �Ѿ��� ������ ��ǥ�� ȸ��
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    void Update()
    {
        // ��ǥ�� ������ �Ѿ��� �ı�
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // �Ѿ��� ��ǥ�� ���� �������� �̵�
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // ��ǥ�� �����ϸ� �������� ������ �Ѿ� �ı�
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    // Ÿ�ٿ� �������� �� ����Ǵ� �޼���
    void HitTarget()
    {
        // ������ �������� ����
        EnemyBase enemy = target.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, false);  // ũ����Ż�� ������ ������ ó��
        }

        // �Ѿ� �ı�
        Destroy(gameObject);
    }

    // �ٸ� ��ü�� �浹���� ���� �Ѿ� �ı�
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.transform == target)
        {
            HitTarget(); // ��ǥ�� �����ϸ� Ÿ�� ��Ʈ
        }
    }
}
