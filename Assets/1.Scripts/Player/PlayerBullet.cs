using Photon.Pun;
using UnityEngine;

public class PlayerBullet : MonoBehaviourPun
{
    public float speed = 10f;
    private float damage;
    private Rigidbody2D rb;
    private Vector2 moveDirection;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    public void SetDamage(float damageValue)
    {
        damage = damageValue;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        EnemyBase enemy = hitInfo.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, true);  // true를 전달하여 플레이어가 공격했음을 알림
            DestroyBullet();  // 총알 파괴
        }
        else
        {
            // 총알이 다른 오브젝트에 부딪혀도 파괴
            DestroyBullet();
        }
    }

    // 총알을 네트워크 상에서 파괴
    private void DestroyBullet()
    {
        if (gameObject != null && photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
