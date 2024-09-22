using System.Collections;
using UnityEngine;
using TMPro;

public class CrystalAttack : MonoBehaviour
{
    public GameObject bulletPrefab;        // ũ����Ż�� �߻��ϴ� �Ѿ� ������
    public Transform firePoint;            // ũ����Ż�� �߻� ��ġ
    public float attackRange = 10f;        // ũ����Ż�� ���� ����
    public float attackCooldown = 2f;      // ���� ����
    public TextMeshProUGUI timeRemainingText; // ���� �ð� ǥ�� TMP

    private bool isAttacking = false;      // ���� ������ ����
    private float timeRemaining = 0f;      // ���� ���� �ð�
    private Coroutine attackCoroutine;     // ���� �ڷ�ƾ

    void Update()
    {
        // ũ����Ż�� ���� ���� ���� ���� ã�� ����
        if (isAttacking)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeUI();

            if (timeRemaining <= 0)
            {
                isAttacking = false;  // ���� �ð��� ������ ���� �ߴ�
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine); // �ڷ�ƾ ����
                }
                timeRemainingText.gameObject.SetActive(false); // �ð��� 0�� �Ǹ� TMP ����
            }
        }
    }

    // �������� ũ����Ż ���� Ȱ��ȭ �� ȣ��Ǵ� �޼���
    public void ActivateCrystalAttack(float additionalTime)
    {
        timeRemaining += additionalTime;   // �߰��� �ð��� ����
        isAttacking = true;

        if (!timeRemainingText.gameObject.activeSelf)
        {
            timeRemainingText.gameObject.SetActive(true);  // TMP Ȱ��ȭ
        }

        // �ڷ�ƾ�� �̹� ���� ���̸� �ٽ� �������� ����
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }

        Debug.Log("ũ����Ż�� " + timeRemaining + "�� ���� ���� �����մϴ�.");
    }

    // ũ����Ż�� ���� ����� ���� ã�� �����ϴ� ��ƾ
    IEnumerator AttackRoutine()
    {
        while (isAttacking)
        {
            AttackClosestEnemy(); // ���� ����� ���� ã�� ����
            yield return new WaitForSeconds(attackCooldown); // ���� ���� ����
        }

        attackCoroutine = null; // �ڷ�ƾ ���� �� null�� ����
    }

    // ũ����Ż�� ���� ����� ���� ã�� �����ϴ� �޼���
    void AttackClosestEnemy()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange);
        Transform closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        // ���� �� ���� ã��
        foreach (Collider2D enemy in enemiesInRange)
        {
            if (enemy.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    closestEnemy = enemy.transform;
                }
            }
        }

        // ���� ����� ���� ���� �Ѿ� �߻�
        if (closestEnemy != null)
        {
            FireAtEnemy(closestEnemy);
        }
    }

    // ���� ���� �Ѿ��� �߻��ϴ� �Լ�
    void FireAtEnemy(Transform enemy)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<CrystalBullet>().SetTarget(enemy); // �Ѿ˿� ���� ��ġ ����
    }

    // ���� �ð��� 00:00 �������� ������Ʈ
    void UpdateTimeUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timeRemainingText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
