using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class RoomManager : MonoBehaviourPunCallbacks
{
    // UI ��ҵ�
    public TMP_InputField roomNameInputField;  // �� �̸� �Է� �ʵ�
    public TMP_InputField passwordInputField;  // ��й�ȣ �Է� �ʵ�
    public Toggle privateRoomToggle;           // ����� �� ���� ��� ��ư
    public Transform roomListContent;          // �� ����� ǥ���� Content ����
    public GameObject roomListItemPrefab;      // �� ��� �׸� ������
    public GameObject passwordPanel;           // ��й�ȣ �Է� �г�
    public TMP_InputField passwordConfirmInputField;  // ��й�ȣ Ȯ�� �ʵ�
    public Button passwordConfirmButton;       // ��й�ȣ Ȯ�� ��ư
    public GameObject roomUIPanel;             // �� UI �г�
    public GameObject warningPanel;            // ��� �޽��� �г�
    public TMP_Text warningText;               // ��� �޽��� �ؽ�Ʈ

    private const int maxPlayers = 3;          // ���� �ִ� �÷��̾� ��
    private const int passwordLength = 4;      // ��й�ȣ ���� ����
    private string selectedRoomName;           // ���õ� �� �̸�
    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();  // �� ��� ĳ��

    void Start()
    {
        // ����� �� ���ο� ���� ��й�ȣ �Է� �ʵ� Ȱ��ȭ
        privateRoomToggle.onValueChanged.AddListener(delegate { TogglePasswordField(privateRoomToggle.isOn); });

        // ���� �� ��й�ȣ �ʵ� ��Ȱ��ȭ
        TogglePasswordField(false);

        // ��й�ȣ Ȯ�� ��ư �̺�Ʈ ����
        passwordConfirmButton.onClick.AddListener(OnPasswordConfirmButtonClicked);

        // �⺻������ ��� �г� ��Ȱ��ȭ
        warningPanel.SetActive(false);
    }

    // ��� �޽��� ǥ�� �Լ�
    private void ShowWarning(string message)
    {
        warningText.text = message;
        warningPanel.SetActive(true);  // ��� �г� Ȱ��ȭ
    }

    // �� ��� ���ΰ�ħ
    public void OnRefreshButtonClicked()
    {
        PhotonNetwork.JoinLobby();  // �κ� �ٽ� �����Ͽ� �� ��� ����
    }

    // ����� �� ���ο� ���� ��й�ȣ �ʵ� Ȱ��ȭ/��Ȱ��ȭ
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

        // �� �̸��� ���� ��� ��� �޽��� ���
        if (string.IsNullOrEmpty(roomName))
        {
            ShowWarning("�� �̸��� �Է��ϼ���.");
            return;
        }

        // ����� �� ��й�ȣ ��ȿ�� �˻�
        if (isPrivate)
        {
            if (password.Length != passwordLength || !int.TryParse(password, out _))
            {
                ShowWarning("��й�ȣ�� 4�ڸ� ���ڿ��� �մϴ�.");
                return;
            }
        }

        // Photon RoomOptions ����
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;  // �ִ� �÷��̾� �� ����
        roomOptions.IsVisible = true;  // �� ��Ͽ� ǥ�� (����/����� ����)
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add("password", isPrivate ? password : "");  // ��й�ȣ ����
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };  // �κ񿡼� ��й�ȣ ���� ����

        PhotonNetwork.CreateRoom(roomName, roomOptions);  // �� ����
    }

    // �� ���� ���� �� ȣ��Ǵ� �ݹ�
    public override void OnCreatedRoom()
    {
        Debug.Log("�� ���� ����: " + PhotonNetwork.CurrentRoom.Name);
    }

    // �� ���� ���� �� ȣ��Ǵ� �ݹ�
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ShowWarning("�� ���� ����: " + message);  // ��� �޽��� ���
    }

    // �� ��� ����
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        cachedRoomList = roomList;  // �� ��� ĳ��

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

            // ����� �� ���� Ȯ��
            bool isPrivate = room.CustomProperties != null &&
                             room.CustomProperties.ContainsKey("password") &&
                             !string.IsNullOrEmpty((string)room.CustomProperties["password"]);

            int ping = PhotonNetwork.GetPing();  // �� ����

            roomItemUI.SetRoomInfo(room.Name, room.PlayerCount, room.MaxPlayers, isPrivate, ping);  // �� ���� ����

            // ����� �� Ŭ�� �� ��й�ȣ �Է� UI ǥ��
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

    // ����� �� Ŭ�� �� ��й�ȣ �Է� �г� ǥ��
    public void OnPrivateRoomClicked(string roomName)
    {
        selectedRoomName = roomName;
        passwordPanel.SetActive(true);  // ��й�ȣ �Է� �г� Ȱ��ȭ
    }

    // ��й�ȣ Ȯ�� ��ư Ŭ�� �� ȣ��
    public void OnPasswordConfirmButtonClicked()
    {
        string enteredPassword = passwordConfirmInputField.text;

        // ���õ� ���� ��й�ȣ Ȯ��
        RoomInfo roomInfo = cachedRoomList.FirstOrDefault(r => r.Name == selectedRoomName);
        if (roomInfo != null && roomInfo.CustomProperties.ContainsKey("password"))
        {
            string correctPassword = (string)roomInfo.CustomProperties["password"];
            if (enteredPassword == correctPassword)
            {
                PhotonNetwork.JoinRoom(selectedRoomName);  // ��й�ȣ ��ġ �� �� ����
            }
            else
            {
                ShowWarning("��й�ȣ�� Ʋ�Ƚ��ϴ�.");  // ��� �޽��� ���
            }
        }
        else
        {
            ShowWarning("�� ������ ã�� �� �����ϴ�.");  // �� ������ ���� ���
        }
    }

    // �� ���� ���� �� ȣ��
    public override void OnJoinedRoom()
    {
        Debug.Log("�濡 ���������� �����߽��ϴ�: " + PhotonNetwork.CurrentRoom.Name);
        roomUIPanel.SetActive(true);  // �� UI �г� Ȱ��ȭ
    }

    // �� ������ ��ư Ŭ�� �� ȣ��
    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
}
