using System.Collections;
using UnityEngine;
using TMPro;

public class CrystalAttack : MonoBehaviour
{
    public GameObject bulletPrefab;        // 크리스탈이 발사하는 총알 프리팹
    public Transform firePoint;            // 크리스탈의 발사 위치
    public float attackRange = 10f;        // 크리스탈의 공격 범위
    public float attackCooldown = 2f;      // 공격 간격
    public TextMeshProUGUI timeRemainingText; // 남은 시간 표시 TMP

    private bool isAttacking = false;      // 공격 중인지 여부
    private float timeRemaining = 0f;      // 남은 공격 시간
    private Coroutine attackCoroutine;     // 공격 코루틴

    void Update()
    {
        // 크리스탈이 공격 중일 때만 적을 찾고 공격
        if (isAttacking)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeUI();

            if (timeRemaining <= 0)
            {
                isAttacking = false;  // 공격 시간이 끝나면 공격 중단
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine); // 코루틴 중지
                }
                timeRemainingText.gameObject.SetActive(false); // 시간이 0이 되면 TMP 숨김
            }
        }
    }

    // 상점에서 크리스탈 공격 활성화 시 호출되는 메서드
    public void ActivateCrystalAttack(float additionalTime)
    {
        timeRemaining += additionalTime;   // 추가된 시간을 누적
        isAttacking = true;

        if (!timeRemainingText.gameObject.activeSelf)
        {
            timeRemainingText.gameObject.SetActive(true);  // TMP 활성화
        }

        // 코루틴이 이미 실행 중이면 다시 시작하지 않음
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }

        Debug.Log("크리스탈이 " + timeRemaining + "초 동안 적을 공격합니다.");
    }

    // 크리스탈이 가장 가까운 적을 찾아 공격하는 루틴
    IEnumerator AttackRoutine()
    {
        while (isAttacking)
        {
            AttackClosestEnemy(); // 가장 가까운 적을 찾아 공격
            yield return new WaitForSeconds(attackCooldown); // 공격 간격 유지
        }

        attackCoroutine = null; // 코루틴 종료 시 null로 설정
    }

    // 크리스탈이 가장 가까운 적을 찾아 공격하는 메서드
    void AttackClosestEnemy()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange);
        Transform closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        // 범위 내 적을 찾음
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

        // 가장 가까운 적을 향해 총알 발사
        if (closestEnemy != null)
        {
            FireAtEnemy(closestEnemy);
        }
    }

    // 적을 향해 총알을 발사하는 함수
    void FireAtEnemy(Transform enemy)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<CrystalBullet>().SetTarget(enemy); // 총알에 적의 위치 설정
    }

    // 남은 시간을 00:00 형식으로 업데이트
    void UpdateTimeUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timeRemainingText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
