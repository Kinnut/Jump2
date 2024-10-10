using UnityEngine;
using Photon.Pun;

public class CrystalBullet : MonoBehaviourPun
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
            photonView.RPC("SyncTarget", RpcTarget.All, target.GetComponent<PhotonView>().ViewID); // 타겟 동기화
            UpdateDirection(); // 목표 방향 갱신
        }
    }

    void Update()
    {
        if (target == null)
        {
            DestroySelf(); // 목표가 없으면 총알을 파괴
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

    // 타겟 ID를 받아 타겟을 설정하는 메서드
    [PunRPC]
    void SyncTarget(int targetViewId)
    {
        PhotonView targetView = PhotonView.Find(targetViewId); // PhotonView ID를 통해 타겟을 찾음
        if (targetView != null)
        {
            target = targetView.transform; // 타겟의 Transform 설정
            UpdateDirection(); // 타겟의 방향 갱신
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

        DestroySelf(); // 총알 파괴
    }

    // 목표 방향을 계산하고 총알 회전
    void UpdateDirection()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    // 다른 물체와 충돌했을 때도 총알 파괴
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.transform == target)
        {
            HitTarget(); // 목표에 도착하면 타겟 히트
        }
    }

    // 안전하게 네트워크 상에서 총알을 제거하는 함수
    void DestroySelf()
    {
        // 소유자가 아니거나 마스터 클라이언트가 아닌 경우, 마스터 클라이언트에게 파괴 요청
        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject); // 총알 파괴
        }
        else
        {
            // 마스터 클라이언트에게 총알을 파괴하도록 요청
            photonView.RPC("RequestDestroy", RpcTarget.MasterClient, photonView.ViewID);
        }
    }

    // 마스터 클라이언트가 파괴를 처리하는 RPC
    [PunRPC]
    void RequestDestroy(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(targetView.gameObject);
        }
    }
}
