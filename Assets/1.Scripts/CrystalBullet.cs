using UnityEngine;
using Photon.Pun;

public class CrystalBullet : MonoBehaviourPun
{
    public float speed = 10f;         // �Ѿ� �ӵ�
    public float damage = 20f;        // �Ѿ� ������
    private Transform target;         // ��ǥ ���� Transform

    // ��ǥ ���� �޼���
    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;         // Ÿ�� ����
        if (target != null)
        {
            photonView.RPC("SyncTarget", RpcTarget.All, target.GetComponent<PhotonView>().ViewID); // Ÿ�� ����ȭ
            UpdateDirection(); // ��ǥ ���� ����
        }
    }

    void Update()
    {
        if (target == null)
        {
            DestroySelf(); // ��ǥ�� ������ �Ѿ��� �ı�
            return;
        }

        // �Ѿ��� ��ǥ�� ���� �������� �̵�
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // ��ǥ�� �����ϸ� �������� ������ �Ѿ� �ı�
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    // Ÿ�� ID�� �޾� Ÿ���� �����ϴ� �޼���
    [PunRPC]
    void SyncTarget(int targetViewId)
    {
        PhotonView targetView = PhotonView.Find(targetViewId); // PhotonView ID�� ���� Ÿ���� ã��
        if (targetView != null)
        {
            target = targetView.transform; // Ÿ���� Transform ����
            UpdateDirection(); // Ÿ���� ���� ����
        }
    }

    // Ÿ�ٿ� �������� �� ����Ǵ� �޼���
    void HitTarget()
    {
        // ������ �������� ����
        EnemyBase enemy = target.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, false);  // ũ����Ż�� ������ ������ ó��
        }

        DestroySelf(); // �Ѿ� �ı�
    }

    // ��ǥ ������ ����ϰ� �Ѿ� ȸ��
    void UpdateDirection()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    // �ٸ� ��ü�� �浹���� ���� �Ѿ� �ı�
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.transform == target)
        {
            HitTarget(); // ��ǥ�� �����ϸ� Ÿ�� ��Ʈ
        }
    }

    // �����ϰ� ��Ʈ��ũ �󿡼� �Ѿ��� �����ϴ� �Լ�
    void DestroySelf()
    {
        // �����ڰ� �ƴϰų� ������ Ŭ���̾�Ʈ�� �ƴ� ���, ������ Ŭ���̾�Ʈ���� �ı� ��û
        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject); // �Ѿ� �ı�
        }
        else
        {
            // ������ Ŭ���̾�Ʈ���� �Ѿ��� �ı��ϵ��� ��û
            photonView.RPC("RequestDestroy", RpcTarget.MasterClient, photonView.ViewID);
        }
    }

    // ������ Ŭ���̾�Ʈ�� �ı��� ó���ϴ� RPC
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
