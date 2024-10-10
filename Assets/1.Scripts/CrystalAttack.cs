using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CrystalAttack : MonoBehaviourPun
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
        if (isAttacking)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // ������ Ŭ���̾�Ʈ������ �ð��� ����
                timeRemaining -= Time.deltaTime;

                // ��� Ŭ���̾�Ʈ���� �ð��� ����ȭ
                photonView.RPC("SyncTimeRemaining", RpcTarget.All, timeRemaining);
            }

            // �ð��� 0�� �Ǹ� ���� ����
            if (timeRemaining <= 0)
            {
                isAttacking = false;
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine); // �ڷ�ƾ ����
                }
                photonView.RPC("StopTimeUI", RpcTarget.All); // ��� Ŭ���̾�Ʈ���� UI �����
            }
        }
    }

    public void ActivateCrystalAttack(float additionalTime)
    {
        // ��� Ŭ���̾�Ʈ�� �ð��� �߰��ϰ� ������ Ŭ���̾�Ʈ�� ����
        photonView.RPC("RequestAddAttackTime", RpcTarget.MasterClient, additionalTime); // �ð��� ������ Ŭ���̾�Ʈ�� ����
    }

    [PunRPC]
    void RequestAddAttackTime(float additionalTime)
    {
        // ������ Ŭ���̾�Ʈ������ �ð��� ����
        if (PhotonNetwork.IsMasterClient)
        {
            timeRemaining += additionalTime;  // �߰��� �ð��� ����
            isAttacking = true;

            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackRoutine());
            }

            // ��� Ŭ���̾�Ʈ���� �ð��� ����ȭ
            photonView.RPC("SyncTimeRemaining", RpcTarget.AllBuffered, timeRemaining);
            photonView.RPC("ShowTimeUI", RpcTarget.AllBuffered); // ��� Ŭ���̾�Ʈ���� UI ǥ��
        }
    }

    IEnumerator AttackRoutine()
    {
        while (isAttacking)
        {
            // ������ Ŭ���̾�Ʈ������ �� ����
            if (PhotonNetwork.IsMasterClient)
            {
                AttackClosestEnemy();
            }
            yield return new WaitForSeconds(attackCooldown); // ���� ���� ����
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
            // ������ Ŭ���̾�Ʈ������ �Ѿ��� ����
            FireAtEnemy(closestEnemy.GetComponent<PhotonView>().ViewID);
        }
    }

    void FireAtEnemy(int enemyViewID)
    {
        PhotonView targetView = PhotonView.Find(enemyViewID); // ���� PhotonView�� ã��
        if (targetView != null)
        {
            Transform enemyTransform = targetView.transform;
            if (PhotonNetwork.IsMasterClient) // ������ Ŭ���̾�Ʈ������ �Ѿ� ����
            {
                GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);
                bullet.GetComponent<CrystalBullet>().SetTarget(enemyTransform); // �Ѿ˿� ���� ��ġ ����
            }
        }
    }

    // ��� Ŭ���̾�Ʈ���� ���� �ð��� 00:00 �������� ������Ʈ
    [PunRPC]
    void SyncTimeRemaining(float newTimeRemaining)
    {
        timeRemaining = newTimeRemaining;
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timeRemainingText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // UI ǥ��
    [PunRPC]
    void ShowTimeUI()
    {
        timeRemainingText.gameObject.SetActive(true);
    }

    // UI �����
    [PunRPC]
    void StopTimeUI()
    {
        timeRemainingText.gameObject.SetActive(false);
    }
}
