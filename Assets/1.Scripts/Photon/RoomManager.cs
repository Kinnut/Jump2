using UnityEngine;
using TMPro;  // TextMeshPro ���
using Photon.Pun;  // Photon ���
using Photon.Realtime;  // Photon�� Room ���� ��� ���
using UnityEngine.UI;  // ScrollView �� Toggle ���
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInputField;  // �� �̸� �Է� �ʵ�
    public TMP_InputField passwordInputField;  // ��й�ȣ �Է� �ʵ�
    public Toggle privateRoomToggle;           // ����� �� ���� ���
    public Transform roomListContent;          // �� ����� ǥ���� ScrollView�� Content ����
    public GameObject roomListItemPrefab;      // �� ��� �׸� ������
    public GameObject passwordPanel;           // ��й�ȣ �Է� �г�
    public TMP_InputField passwordConfirmInputField;  // ��й�ȣ Ȯ�� �ʵ�
    public Button passwordConfirmButton;       // ��й�ȣ Ȯ�� ��ư
    public GameObject roomUIPanel; // �� UI�� ������ Panel

    private const int maxPlayers = 3;          // ���� �ִ� �÷��̾� ��
    private const int passwordLength = 4;      // ��й�ȣ �ִ� ����
    private string selectedRoomName;           // ���õ� �� �̸�
    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();  // �� ����� ������ ����Ʈ

    void Start()
    {
        // ����� �� ��ۿ� ���� ��й�ȣ �ʵ� Ȱ��ȭ
        privateRoomToggle.onValueChanged.AddListener(delegate { TogglePasswordField(privateRoomToggle.isOn); });

        // ������ �� ��й�ȣ �Է� �ʵ� ��Ȱ��ȭ
        TogglePasswordField(false);

        // ��й�ȣ Ȯ�� ��ư Ŭ�� �̺�Ʈ ����
        passwordConfirmButton.onClick.AddListener(OnPasswordConfirmButtonClicked);
    }

    // �� ��� ���ΰ�ħ ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void OnRefreshButtonClicked()
    {
        Debug.Log("�� ��� ���ΰ�ħ...");
        PhotonNetwork.JoinLobby();  // �κ� �ٽ� �����Ͽ� �� ����� ����
    }

    // ����� �� ��ۿ� ���� ��й�ȣ �Է� �ʵ� Ȱ��ȭ/��Ȱ��ȭ
    private void TogglePasswordField(bool isPrivate)
    {
        passwordInputField.interactable = isPrivate;
        if (!isPrivate)
        {
            passwordInputField.text = "";  // ���� ���̸� ��й�ȣ �ʱ�ȭ
        }
    }

    // �� ���� ��ư Ŭ�� �� ȣ��
    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;
        string password = passwordInputField.text;
        bool isPrivate = privateRoomToggle.isOn;

        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("�� �̸��� �Է��ϼ���.");
            return;
        }

        if (isPrivate)
        {
            // ����� ���� ��� ��й�ȣ �Է� ��ȿ�� �˻�
            if (password.Length != passwordLength || !int.TryParse(password, out _))
            {
                Debug.LogError("��й�ȣ�� 4�ڸ� ���ڿ��� �մϴ�.");
                return;
            }
        }

        // Photon�� RoomOptions ����
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.IsVisible = true;  // ���� ��Ͽ� ǥ�� (����/����� �������)
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add("password", isPrivate ? password : "");  // ��й�ȣ ����
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };  // �κ񿡼� ��й�ȣ ���� ����
        roomOptions.EmptyRoomTtl = 0;  // ���� ��� ������ ��� ����

        // �� ����
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }


    // �� ���� ���� �� ȣ��Ǵ� �ݹ�
    public override void OnCreatedRoom()
    {
        Debug.Log("�� ���� ����: " + PhotonNetwork.CurrentRoom.Name);
    }

    // �� ���� ���� �� ȣ��Ǵ� �ݹ�
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("�� ���� ����: " + message);
    }

    // �� ��� ���� �� ȣ��Ǵ� �ݹ� (Scroll View �ȿ� �� ��� ǥ��)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        cachedRoomList = roomList;  // �� ����� ����

        // ���� �� ��� ����
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        // ���ο� �� ��� �߰�
        foreach (RoomInfo room in roomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);
            RoomItemUI roomItemUI = roomItem.GetComponent<RoomItemUI>();

            // ����� �� ���θ� CustomProperties���� Ȯ��
            bool isPrivate = room.CustomProperties != null &&
                             room.CustomProperties.ContainsKey("password") &&
                             !string.IsNullOrEmpty((string)room.CustomProperties["password"]);

            // ���� �� ������ ������ (���� Ŭ���̾�Ʈ�� ��)
            int ping = PhotonNetwork.GetPing();

            // UI�� �� ���� ���� (�� �̸�, ���� �÷��̾� ��, �ִ� �÷��̾� ��, ����� ����, �� ����)
            roomItemUI.SetRoomInfo(room.Name, room.PlayerCount, room.MaxPlayers, isPrivate, ping);

            // ����� ���̸� Ŭ�� �� ��й�ȣ �Է� UI ȣ��
            if (isPrivate)
            {
                roomItemUI.GetComponent<Button>().onClick.AddListener(() => OnPrivateRoomClicked(room.Name));
            }
            else
            {
                // ���� ���� �ٷ� ����
                roomItemUI.GetComponent<Button>().onClick.AddListener(() => PhotonNetwork.JoinRoom(room.Name));
            }
        }
    }

    // ����� �� Ŭ�� �� ȣ��Ǵ� �Լ� (��й�ȣ �Է�â ǥ��)
    public void OnPrivateRoomClicked(string roomName)
    {
        selectedRoomName = roomName;  // ���õ� �� �̸� ����
        passwordPanel.SetActive(true);  // ��й�ȣ �Է� �г� Ȱ��ȭ
    }

    // ��й�ȣ Ȯ�� ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void OnPasswordConfirmButtonClicked()
    {
        string enteredPassword = passwordConfirmInputField.text;

        // ����� �� ��Ͽ��� ���õ� ���� ������ ã��
        RoomInfo roomInfo = cachedRoomList.FirstOrDefault(r => r.Name == selectedRoomName);
        if (roomInfo != null && roomInfo.CustomProperties.ContainsKey("password"))
        {
            string correctPassword = (string)roomInfo.CustomProperties["password"];
            if (enteredPassword == correctPassword)
            {
                // ��й�ȣ�� ������ �濡 ����
                PhotonNetwork.JoinRoom(selectedRoomName);
            }
            else
            {
                Debug.LogError("��й�ȣ�� Ʋ�Ƚ��ϴ�.");
            }
        }
        else
        {
            Debug.LogError("�� ������ ã�� �� �����ϴ�.");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("�濡 ���������� �����߽��ϴ�: " + PhotonNetwork.CurrentRoom.Name);

        // �濡 �����ϸ� Room UI �г��� Ȱ��ȭ
        roomUIPanel.SetActive(true);

        // �� �̸��� UI�� ǥ�� (RoomUIManager�� �����ϴ� UI�� �� �̸� ����)
        RoomUIManager roomUI = roomUIPanel.GetComponent<RoomUIManager>();
        roomUI.InitializeRoomUI(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnLeftRoom()
    {
        // ���� ������ �κ�� ���ư�
        roomUIPanel.SetActive(false);
    }

    // �� ������ ��ư Ŭ�� �� ȣ��
    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
}
