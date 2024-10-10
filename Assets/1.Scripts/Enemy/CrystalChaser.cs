using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CrystalChaser : EnemyBase
{
    public float explosionRange = 3f; // ũ����Ż���� ���� ����
    public float damage = 50f; // ũ����Ż�� �ִ� ����
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
        moveSpeedOriginal = moveSpeed;  // �⺻ �̵� �ӵ� ����
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

    // ���� ���߰� �ϴ� �Լ�
    public void FreezeForDuration(float duration)
    {
        StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        moveSpeed = 0f;
        yield return new WaitForSeconds(duration);
        moveSpeed = moveSpeedOriginal;  // ���� �ӵ��� ����
    }
}
