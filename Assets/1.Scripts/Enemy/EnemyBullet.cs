using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f; // �Ѿ� �ӵ�
    private float damage; // �Ѿ� ������
    private Vector2 moveDirection; // �Ѿ��� �̵� ����

    public float lifeTime = 5f; // �Ѿ��� ������������ �ð�

    void Start()
    {
        Destroy(gameObject, lifeTime); // ���� �ð� �� �Ѿ��� �����
    }

    void FixedUpdate()
    {
        // �Ѿ��� ������ �������� �̵�
        transform.Translate(moveDirection * speed * Time.fixedDeltaTime, Space.World);
    }

    public void SetDamage(float damageValue)
    {
        damage = damageValue; // ���� ������ ������ ����
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized; // �߻� ���� ����

        // ȸ�� ������ �߻� ���⿡ �°� ���
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

        // �Ѿ��� �߻� �������� ȸ��
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Player player = hitInfo.GetComponent<Player>(); // �÷��̾ ������
        if (player != null)
        {
            player.TakeDamage(damage); // �÷��̾�� �������� ��
            Destroy(gameObject); // �Ѿ� �ı�
        }
    }
}
