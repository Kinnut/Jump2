using UnityEngine;

public class CrystalChaser : EnemyBase
{
    public float explosionRange = 3f; // 크리스탈과의 폭발 범위
    public float damage = 50f; // 크리스탈에 주는 피해
    private Transform crystal;
    private Crystal crystalScript; // Crystal 스크립트 참조
    private bool hasExploded = false; // 자폭 여부 체크

    protected override void Start()
    {
        base.Start();
        crystal = GameObject.FindGameObjectWithTag("Crystal").transform;
        if (crystal != null)
        {
            crystalScript = crystal.GetComponent<Crystal>(); // Crystal 스크립트 가져오기
        }
    }

    protected override void Update()
    {
        if (!hasExploded) // 자폭하지 않았다면 움직임과 자폭 체크
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
        // 크리스탈에 피해
        if (crystalScript != null)
        {
            crystalScript.TakeDamage(damage); // 크리스탈 스크립트의 TakeDamage 호출
        }

        hasExploded = true; // 자폭 처리
        Die(); // 기본 사망 처리로 변경
    }
}
