using Photon.Pun;
using UnityEngine;
using ExitGames.Client.Photon;

public class PlayerSpawner : MonoBehaviourPun
{
    public GameObject playerPrefab;  // �÷��̾� ĳ���� ������

    // �ñر� ���ÿ� ���� ���� ����
    public Color iceColor;
    public Color fireColor;
    public Color healColor;

    void Start()
    {
        if (PhotonNetwork.IsConnected && playerPrefab != null)
        {
            // ������ ��ġ�� �÷��̾� ĳ���� ����
            Vector3 spawnPosition = new Vector3(-4, 0, 4);
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

            // �÷��̾ ����� �����Ǿ����� Ȯ��
            if (player == null)
            {
                Debug.LogError("Player instantiation failed!");
                return;
            }

            // PhotonView Ȯ�� (�÷��̾� ������Ʈ�� PhotonView)
            PhotonView playerPhotonView = player.GetComponent<PhotonView>();
            if (playerPhotonView == null)
            {
                Debug.LogError("PhotonView not found on player prefab.");
                return;
            }

            // SpriteRenderer Ȯ�� �� ���� ����
            SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
            if (playerSpriteRenderer != null)
            {
                // CustomProperties���� 'SelectedUltimate' Ű�� �ִ��� Ȯ��
                if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("SelectedUltimate"))
                {
                    int selectedUltimate = (int)PhotonNetwork.LocalPlayer.CustomProperties["SelectedUltimate"];
                    Color playerColor = GetColorByUltimate(selectedUltimate);

                    // ���ÿ��� ���� ����
                    playerSpriteRenderer.color = playerColor;

                    // �÷��̾� ������Ʈ�� PhotonView�� RPC ȣ�� (MyPlayer ��ũ��Ʈ�� SyncPlayerColor �޼���)
                    playerPhotonView.RPC("SyncPlayerColor", RpcTarget.OthersBuffered, PhotonNetwork.LocalPlayer.ActorNumber, playerColor.r, playerColor.g, playerColor.b);
                }
                else
                {
                    Debug.LogError("SelectedUltimate not found in CustomProperties.");
                }
            }
            else
            {
                Debug.LogError("SpriteRenderer not found on player prefab.");
            }
        }
        else
        {
            Debug.LogError("PhotonNetwork is not connected or playerPrefab is null");
        }
    }

    // �ñر⿡ ���� ���� ����
    private Color GetColorByUltimate(int ultimateIndex)
    {
        switch (ultimateIndex)
        {
            case 0: return iceColor;
            case 1: return fireColor;
            case 2: return healColor;
            default: return Color.white;
        }
    }
}
