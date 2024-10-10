using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CrystalChaser : EnemyBase
{
    public float explosionRange = 3f; // 크리스탈과의 폭발 범위
    public float damage = 50f; // 크리스탈에 주는 피해
    private Transform crystal;
    private Crystal crystalScript;
    private bool hasExploded = false;
    private float moveSpeedOriginal;

    protected override void Start()
    {
        base.Start();
        crystal = GameObject.FindGameObjectWithTag("Crystal").transform;
        if (crystal != null)
        {
            crystalScript = crystal.GetComponent<Crystal>();
        }
        moveSpeedOriginal = moveSpeed;  // 기본 이동 속도 저장
    }

    protected override void Update()
    {
        if (!hasExploded)
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
        if (crystalScript != null)
        {
            crystalScript.TakeDamage(damage);
        }
        hasExploded = true;
        Die();
    }

    // 적을 멈추게 하는 함수
    public void FreezeForDuration(float duration)
    {
        StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        moveSpeed = 0f;
        yield return new WaitForSeconds(duration);
        moveSpeed = moveSpeedOriginal;  // 원래 속도로 복구
    }
}
