using UnityEngine;

public class PlayerBullet : MonoBehaviour
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

    void OnBecameInvisible()
    {
        Destroy(gameObject);
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
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }

}
