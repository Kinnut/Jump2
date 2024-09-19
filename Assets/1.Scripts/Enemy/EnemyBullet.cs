using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f; // 총알 속도
    private float damage; // 총알 데미지
    private Vector2 moveDirection; // 총알의 이동 방향

    public float lifeTime = 5f; // 총알이 사라지기까지의 시간

    void Start()
    {
        Destroy(gameObject, lifeTime); // 일정 시간 후 총알이 사라짐
    }

    void FixedUpdate()
    {
        // 총알을 설정된 방향으로 이동
        transform.Translate(moveDirection * speed * Time.fixedDeltaTime, Space.World);
    }

    public void SetDamage(float damageValue)
    {
        damage = damageValue; // 적이 설정한 데미지 적용
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized; // 발사 방향 설정

        // 회전 각도를 발사 방향에 맞게 계산
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

        // 총알을 발사 방향으로 회전
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Player player = hitInfo.GetComponent<Player>(); // 플레이어에 맞으면
        if (player != null)
        {
            player.TakeDamage(damage); // 플레이어에게 데미지를 줌
            Destroy(gameObject); // 총알 파괴
        }
    }
}
