using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CrystalAttack : MonoBehaviourPun
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
        if (isAttacking)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // 마스터 클라이언트에서만 시간을 감소
                timeRemaining -= Time.deltaTime;

                // 모든 클라이언트에게 시간을 동기화
                photonView.RPC("SyncTimeRemaining", RpcTarget.All, timeRemaining);
            }

            // 시간이 0이 되면 공격 종료
            if (timeRemaining <= 0)
            {
                isAttacking = false;
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine); // 코루틴 중지
                }
                photonView.RPC("StopTimeUI", RpcTarget.All); // 모든 클라이언트에서 UI 숨기기
            }
        }
    }

    public void ActivateCrystalAttack(float additionalTime)
    {
        // 모든 클라이언트가 시간을 추가하고 마스터 클라이언트에 전송
        photonView.RPC("RequestAddAttackTime", RpcTarget.MasterClient, additionalTime); // 시간을 마스터 클라이언트로 전달
    }

    [PunRPC]
    void RequestAddAttackTime(float additionalTime)
    {
        // 마스터 클라이언트에서만 시간을 관리
        if (PhotonNetwork.IsMasterClient)
        {
            timeRemaining += additionalTime;  // 추가된 시간을 누적
            isAttacking = true;

            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackRoutine());
            }

            // 모든 클라이언트에게 시간을 동기화
            photonView.RPC("SyncTimeRemaining", RpcTarget.AllBuffered, timeRemaining);
            photonView.RPC("ShowTimeUI", RpcTarget.AllBuffered); // 모든 클라이언트에서 UI 표시
        }
    }

    IEnumerator AttackRoutine()
    {
        while (isAttacking)
        {
            // 마스터 클라이언트에서만 적 공격
            if (PhotonNetwork.IsMasterClient)
            {
                AttackClosestEnemy();
            }
            yield return new WaitForSeconds(attackCooldown); // 공격 간격 유지
        }

        attackCoroutine = null;
    }

    void AttackClosestEnemy()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange);
        Transform closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

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

        if (closestEnemy != null)
        {
            // 마스터 클라이언트에서만 총알을 생성
            FireAtEnemy(closestEnemy.GetComponent<PhotonView>().ViewID);
        }
    }

    void FireAtEnemy(int enemyViewID)
    {
        PhotonView targetView = PhotonView.Find(enemyViewID); // 적의 PhotonView를 찾음
        if (targetView != null)
        {
            Transform enemyTransform = targetView.transform;
            if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트에서만 총알 생성
            {
                GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);
                bullet.GetComponent<CrystalBullet>().SetTarget(enemyTransform); // 총알에 적의 위치 설정
            }
        }
    }

    // 모든 클라이언트에서 남은 시간을 00:00 형식으로 업데이트
    [PunRPC]
    void SyncTimeRemaining(float newTimeRemaining)
    {
        timeRemaining = newTimeRemaining;
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timeRemainingText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // UI 표시
    [PunRPC]
    void ShowTimeUI()
    {
        timeRemainingText.gameObject.SetActive(true);
    }

    // UI 숨기기
    [PunRPC]
    void StopTimeUI()
    {
        timeRemainingText.gameObject.SetActive(false);
    }
}
