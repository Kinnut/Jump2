using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 10f; // �Ѿ� ������
    public float lifeTime = 5f; // �Ѿ��� ������������ �ð�

    void Start()
    {
        Destroy(gameObject, lifeTime); // �Ѿ��� ���� �ð��� ������ �����
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Player player = hitInfo.GetComponent<Player>(); // �÷��̾ ������
        if (player != null)
        {
            player.TakeDamage(damage); // �÷��̾�� �������� ��
            Destroy(gameObject); // �Ѿ��� �ı�
        }
    }
}
