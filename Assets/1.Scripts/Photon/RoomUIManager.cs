using UnityEngine;
using TMPro;
using Photon.Pun;

public class RoomUIManager : MonoBehaviour
{
    public TMP_Text roomNameText;  // �� �̸� �ؽ�Ʈ

    // �� �̸��� �÷��̾� ����� �ʱ�ȭ�ϴ� �Լ�
    public void InitializeRoomUI(string roomName)
    {
        // �� �̸��� ����
        roomNameText.text = roomName;
        Debug.Log("�� �̸�: " + roomName);

        // �÷��̾� ��� ������Ʈ
        UpdatePlayerList();
    }

    // �÷��̾� ��� ������Ʈ �Լ� (���� ����)
    private void UpdatePlayerList()
    {
        // ��: PhotonNetwork.PlayerList�� �÷��̾� ����� ������ ǥ��
        Debug.Log("�÷��̾� ����� ������Ʈ�մϴ�.");
    }
}
