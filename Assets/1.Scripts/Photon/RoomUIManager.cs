using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomUIManager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomNameText;           // �� �̸� �ؽ�Ʈ
    public GameObject[] playerCards;        // �÷��̾� ī�� (���� ������ ǥ���� ����)
    public TMP_Text[] playerNameTexts;      // �� ī�忡 ���� �÷��̾� �г��� �ؽ�Ʈ
    public GameObject[] cardFrontPanels;    // �� ī���� �պκ��� ������ �г� (�� ������ ������ �� �������� �г�)

    public Button[] ultimateButtons;        // 1���� �迭�� �ñر� ��ư�� (�� �÷��̾� �� * �� �÷��̾��� �ñر� ��)
    public Button[] selectionButtons;       // �� ������ �ñر� ���� ��ư (�Ͼ� �׵θ�)
    public Button startGameButton;          // ���� ���� ��ư (���常 ���� �� ����)

    public Image[] ultimateSelectionImages; // �� �÷��̾� �߾��� �ñر� �̹��� (�߾ӿ� ǥ�õ� �̹���)
    public Sprite iceSprite, fireSprite, healSprite; // �ñر� ��������Ʈ (����, ��, ġ��)

    private Dictionary<int, int> selectedUltimates = new Dictionary<int, int>(); // �÷��̾ ���õ� �ñر� ����
    private HashSet<int> globallySelectedUltimates = new HashSet<int>(); // ���������� ���õ� �ñر� �ε��� ����

    private const int ultimatesPerPlayer = 3;  // �÷��̾�� 3���� �ñر� ��ư�� �ִٰ� ����

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        // �� �̸� ����
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        // �÷��̾� ��� ������Ʈ
        UpdatePlayerList();

        // �ñر� ���� ��ư �ʱ�ȭ
        for (int i = 0; i < ultimateButtons.Length; i++)
        {
            int buttonIndex = i;
            ultimateButtons[i].onClick.RemoveAllListeners(); // ���� ������ ����
            ultimateButtons[i].onClick.AddListener(() => OnUltimateSelected(buttonIndex));
        }

        // �ڽ��� �ñر� ���� ��ư�� Ȱ��ȭ
        for (int i = 0; i < selectionButtons.Length; i++)
        {
            int playerIndex = i;

            // �ڽ��� ��ư�� Ȱ��ȭ
            if (PhotonNetwork.LocalPlayer.ActorNumber - 1 == playerIndex)
            {
                selectionButtons[i].interactable = true;  // �ڽ��� ��ư Ȱ��ȭ
            }
            else
            {
                selectionButtons[i].interactable = false; // �ٸ� ������ ��ư ��Ȱ��ȭ
            }
        }

        // ���� ���ο� ���� ���� ���� ��ư Ȱ��ȭ
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.interactable = true;
            startGameButton.onClick.AddListener(StartGame);  // ������ ���� ���� ��ư�� ���� �� ����
        }
        else
        {
            startGameButton.interactable = false;  // ������ �ƴ� ��� ���� ���� ��ư ��Ȱ��ȭ
        }
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

        // ActorNumber ������ ���ĵ� �÷��̾� ����� ������
        players.Sort((x, y) => x.ActorNumber.CompareTo(y.ActorNumber));

        // �� �÷��̾ �ùٸ� ���Կ� ��ġ
        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].NickName;  // �ش� ���Կ� �÷��̾� �г��� ����
            cardFrontPanels[i].SetActive(false);  // �г��� ��Ȱ��ȭ�Ͽ� ������ ���̵��� ����
        }
    }

    // �ñر� ���� �� ȣ��Ǵ� �Լ�
    public void OnUltimateSelected(int buttonIndex)
    {
        Debug.Log("�� ���� ������");

        int playerIndex = buttonIndex / ultimatesPerPlayer;  // �÷��̾� �ε��� ���
        int ultimateIndex = buttonIndex % ultimatesPerPlayer;  // �ش� �÷��̾ ������ �ñر� �ε���

        // �ڽ��� ���� ���Կ����� �ñر� ���� ����
        if (PhotonNetwork.LocalPlayer.ActorNumber - 1 != playerIndex) return;

        // �̹� �ٸ� ������ �ش� �ñر⸦ ���������� ���� �Ұ�
        if (globallySelectedUltimates.Contains(ultimateIndex))
        {
            Debug.Log($"Ultimate {ultimateIndex} is already selected by another player.");
            return;
        }

        // �̹� �ٸ� �ñر⸦ ������ ��� ���� �� ���ο� �ñر� ����
        if (selectedUltimates.ContainsKey(playerIndex))
        {
            int previousUltimate = selectedUltimates[playerIndex];
            photonView.RPC("DeselectUltimate", RpcTarget.AllBuffered, playerIndex, previousUltimate);
        }

        // ���ο� �ñر� ����
        photonView.RPC("SelectUltimate", RpcTarget.AllBuffered, playerIndex, ultimateIndex);
    }

    [PunRPC]
    public void SelectUltimate(int playerIndex, int ultimateIndex)
    {
        // �ߺ� ���� ���� (���� ���� Ȯ�� �� ó��)
        if (selectedUltimates.ContainsKey(playerIndex) && selectedUltimates[playerIndex] == ultimateIndex)
        {
            Debug.Log($"Player {playerIndex} already selected this ultimate: {ultimateIndex}");
            return;
        }

        Debug.Log($"Player {playerIndex} selects ultimate {ultimateIndex}");

        // �ñر� ���� ���� ����
        selectedUltimates[playerIndex] = ultimateIndex;
        globallySelectedUltimates.Add(ultimateIndex); // ���� ���ÿ� �߰�

        // ���� �÷��̾��� �ñر� ���� ������ Ŀ���� ������Ƽ�� ����
        if (PhotonNetwork.LocalPlayer.ActorNumber - 1 == playerIndex)
        {
            // �ñر� ������ Ŀ���� ������Ƽ�� �����Ͽ� ��Ʈ��ũ ����ȭ
            Hashtable playerProperties = new Hashtable();
            playerProperties["SelectedUltimate"] = ultimateIndex;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }

        // �ñر� ���ÿ� ���� �߾� �̹����� ����
        switch (ultimateIndex)
        {
            case 0:
                ultimateSelectionImages[playerIndex].sprite = iceSprite;   // ����
                break;
            case 1:
                ultimateSelectionImages[playerIndex].sprite = fireSprite;  // ��
                break;
            case 2:
                ultimateSelectionImages[playerIndex].sprite = healSprite;  // ġ��
                break;
        }

        // UI ���� ����
        UpdateUltimateUI();
    }


    [PunRPC]
    public void DeselectUltimate(int playerIndex, int ultimateIndex)
    {
        // �ñر� ���� ����
        selectedUltimates.Remove(playerIndex);
        globallySelectedUltimates.Remove(ultimateIndex); // ���� ���ÿ��� ����

        Debug.Log($"Player {playerIndex} deselects ultimate {ultimateIndex}");

        // ���õ� �ñر� �̹��� �ʱ�ȭ
        ultimateSelectionImages[playerIndex].sprite = null;

        // UI ���� ����
        UpdateUltimateUI();
    }

    // �ñر� ���� UI ������Ʈ
    private void UpdateUltimateUI()
    {
        // �� �÷��̾��� �ñر� ���� ���¿� ���� UI ������Ʈ
        foreach (var selected in selectedUltimates)
        {
            int playerIndex = selected.Key;
            int ultimateIndex = selected.Value;

            // �ش� �÷��̾��� �ñر� ��ư�� ���õ� ���·� ǥ��
            for (int i = 0; i < ultimatesPerPlayer; i++)
            {
                int buttonIndex = playerIndex * ultimatesPerPlayer + i;
                ultimateButtons[buttonIndex].interactable = (i != ultimateIndex);
            }
        }

        // ���������� ���õ� �ñر� ó��
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount * ultimatesPerPlayer; i++)
        {
            int ultimateIndex = i % ultimatesPerPlayer;
            if (globallySelectedUltimates.Contains(ultimateIndex))
            {
                ultimateButtons[i].interactable = false;
            }
        }
    }

    // ��� ������ �ñر⸦ �����ߴ��� Ȯ���ϴ� �Լ�
    private bool AllPlayersSelectedUltimate()
    {
        // ���� ��� �÷��̾ �ñر⸦ �����ߴ��� Ȯ��
        return PhotonNetwork.CurrentRoom.PlayerCount == selectedUltimates.Count;
    }

    // ���� ���� ��ư Ŭ�� �� ȣ��Ǵ� �Լ� (���常)
    public void StartGame()
    {
        if (AllPlayersSelectedUltimate())
        {
            Debug.Log("��� �÷��̾ �ñر⸦ ���������Ƿ� ������ �����մϴ�.");

            // �� �޼��带 ����ϸ� �濡 �ִ� ��� Ŭ���̾�Ʈ�� ����ȭ�� ���·� ���� �ε��մϴ�.
            PhotonNetwork.LoadLevel("1.PlayScene");  // ��� �÷��̾ ����ȭ�� �� �ε�
        }
        else
        {
            Debug.Log("��� �÷��̾ �ñر⸦ �������� �ʾҽ��ϴ�.");
        }
    }


    // ���ο� �÷��̾ �濡 ������ �� ȣ��Ǵ� �ݹ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "��(��) �濡 �����߽��ϴ�.");
        UpdatePlayerList();  // �÷��̾� ��� ����
        UpdateUltimateUI();   // �÷��̾ �����ϸ� �ñر� ���� ����
    }

    // �÷��̾ ���� ������ �� ȣ��Ǵ� �ݹ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + "��(��) ���� �������ϴ�.");

        // �÷��̾ ������ �ñر⸦ ����
        if (selectedUltimates.ContainsKey(otherPlayer.ActorNumber - 1))
        {
            int ultimateIndex = selectedUltimates[otherPlayer.ActorNumber - 1];
            photonView.RPC("DeselectUltimate", RpcTarget.AllBuffered, otherPlayer.ActorNumber - 1, ultimateIndex);
        }

        UpdatePlayerList();  // �÷��̾� ��� ����
        UpdateUltimateUI();   // �÷��̾ ������ �ñر� ���� ����
    }
}
