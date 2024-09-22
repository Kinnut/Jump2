using UnityEngine;

public class CrystalBullet : MonoBehaviour
{
    public float speed = 10f;         // 총알 속도
    public float damage = 20f;        // 총알 데미지
    private Transform target;         // 목표 적의 Transform

    // 목표 설정 메서드
    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;         // 타겟 설정
        if (target != null)
        {
            // 총알의 방향을 목표로 회전
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    void Update()
    {
        // 목표가 없으면 총알을 파괴
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 총알이 목표를 향해 직선으로 이동
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // 목표에 도착하면 데미지를 입히고 총알 파괴
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    // 타겟에 도착했을 때 실행되는 메서드
    void HitTarget()
    {
        // 적에게 데미지를 입힘
        EnemyBase enemy = target.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, false);  // 크리스탈이 공격한 것으로 처리
        }

        // 총알 파괴
        Destroy(gameObject);
    }

    // 다른 물체와 충돌했을 때도 총알 파괴
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.transform == target)
        {
            HitTarget(); // 목표에 도착하면 타겟 히트
        }
    }
}
