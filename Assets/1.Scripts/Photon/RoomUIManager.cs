using UnityEngine;
using TMPro;  // TextMeshPro ���
using Photon.Pun;  // Photon ���
using Photon.Realtime;  // Photon�� Room ���� ��� ���
using System.Collections.Generic;  // List ���

public class RoomUIManager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomNameText;           // �� �̸� �ؽ�Ʈ
    public GameObject[] playerCards;        // �÷��̾� ī�� (���� ������ ǥ���� ����)
    public TMP_Text[] playerNameTexts;      // �� ī�忡 ���� �÷��̾� �г��� �ؽ�Ʈ
    public GameObject[] cardFrontPanels;    // �� ī���� �պκ��� ������ �г� (�� ������ ������ �� �������� �г�)

    void Start()
    {
        // �� �̸� ����
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        // �÷��̾� ��� ������Ʈ
        UpdatePlayerList();
    }

    public void InitializeRoomUI(string roomName)
    {
        // �� �̸��� ����
        roomNameText.text = roomName;
        Debug.Log("�� �̸�: " + roomName);

        // �÷��̾� ��� ������Ʈ
        UpdatePlayerList();
    }

    // �÷��̾� ��� ������Ʈ �Լ�
    private void UpdatePlayerList()
    {
        // ��� ���� �ʱ�ȭ (�г��� �ٽ� ����)
        for (int i = 0; i < playerCards.Length; i++)
        {
            playerNameTexts[i].text = "???";  // ���� �г��� ����α�
            cardFrontPanels[i].SetActive(true);  // �г� Ȱ��ȭ
        }

        // ���� �濡 �ִ� ��� �÷��̾� ����� ������
        List<Player> players = new List<Player>(PhotonNetwork.CurrentRoom.Players.Values);

        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].NickName;  // �ش� ���Կ� �÷��̾� �г��� ����
            cardFrontPanels[i].SetActive(false);  // �г��� ��Ȱ��ȭ�Ͽ� ������ ���̵��� ����
        }
    }

    // ���ο� �÷��̾ �濡 ������ �� ȣ��Ǵ� �ݹ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "��(��) �濡 �����߽��ϴ�.");
        UpdatePlayerList();  // �÷��̾� ��� ����
    }

    // �÷��̾ ���� ������ �� ȣ��Ǵ� �ݹ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + "��(��) ���� �������ϴ�.");
        UpdatePlayerList();  // �÷��̾� ��� ����
    }
}
