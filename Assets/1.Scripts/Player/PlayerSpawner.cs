using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;  // �÷��̾� ĳ���� ������

    void Start()
    {
        // ��Ʈ��ũ �󿡼� �� �÷��̾ �ڽ��� ĳ���͸� ����
        if (PhotonNetwork.IsConnected && playerPrefab != null)
        {
            // ������ ��ġ�� �÷��̾� ĳ���� ����
            Vector3 spawnPosition = new Vector3(-4, 0, 4);

            // PhotonNetwork.Instantiate�� ĳ���͸� �����ϰ� ��� Ŭ���̾�Ʈ�� ����ȭ
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        }
    }
}
