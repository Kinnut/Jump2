using UnityEngine;
using TMPro;  // TextMeshPro 사용
using Photon.Pun;  // Photon 사용
using Photon.Realtime;  // Photon의 Room 관련 기능 사용
using UnityEngine.UI;  // ScrollView 및 Toggle 사용
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInputField;  // 방 이름 입력 필드
    public TMP_InputField passwordInputField;  // 비밀번호 입력 필드
    public Toggle privateRoomToggle;           // 비공개 방 여부 토글
    public Transform roomListContent;          // 방 목록을 표시할 ScrollView의 Content 영역
    public GameObject roomListItemPrefab;      // 방 목록 항목 프리팹
    public GameObject passwordPanel;           // 비밀번호 입력 패널
    public TMP_InputField passwordConfirmInputField;  // 비밀번호 확인 필드
    public Button passwordConfirmButton;       // 비밀번호 확인 버튼
    public GameObject roomUIPanel; // 방 UI를 관리할 Panel

    private const int maxPlayers = 3;          // 방의 최대 플레이어 수
    private const int passwordLength = 4;      // 비밀번호 최대 길이
    private string selectedRoomName;           // 선택된 방 이름
    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();  // 방 목록을 저장할 리스트

    void Start()
    {
        // 비공개 방 토글에 따라 비밀번호 필드 활성화
        privateRoomToggle.onValueChanged.AddListener(delegate { TogglePasswordField(privateRoomToggle.isOn); });

        // 시작할 때 비밀번호 입력 필드 비활성화
        TogglePasswordField(false);

        // 비밀번호 확인 버튼 클릭 이벤트 설정
        passwordConfirmButton.onClick.AddListener(OnPasswordConfirmButtonClicked);
    }

    // 방 목록 새로고침 버튼 클릭 시 호출되는 함수
    public void OnRefreshButtonClicked()
    {
        Debug.Log("방 목록 새로고침...");
        PhotonNetwork.JoinLobby();  // 로비에 다시 접속하여 방 목록을 갱신
    }

    // 비공개 방 토글에 따른 비밀번호 입력 필드 활성화/비활성화
    private void TogglePasswordField(bool isPrivate)
    {
        passwordInputField.interactable = isPrivate;
        if (!isPrivate)
        {
            passwordInputField.text = "";  // 공개 방이면 비밀번호 초기화
        }
    }

    // 방 생성 버튼 클릭 시 호출
    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;
        string password = passwordInputField.text;
        bool isPrivate = privateRoomToggle.isOn;

        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("방 이름을 입력하세요.");
            return;
        }

        if (isPrivate)
        {
            // 비공개 방의 경우 비밀번호 입력 유효성 검사
            if (password.Length != passwordLength || !int.TryParse(password, out _))
            {
                Debug.LogError("비밀번호는 4자리 숫자여야 합니다.");
                return;
            }
        }

        // Photon의 RoomOptions 설정
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.IsVisible = true;  // 방을 목록에 표시 (공개/비공개 상관없이)
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add("password", isPrivate ? password : "");  // 비밀번호 저장
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };  // 로비에서 비밀번호 정보 노출
        roomOptions.EmptyRoomTtl = 0;  // 방이 비어 있으면 즉시 삭제

        // 방 생성
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }


    // 방 생성 성공 시 호출되는 콜백
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공: " + PhotonNetwork.CurrentRoom.Name);
    }

    // 방 생성 실패 시 호출되는 콜백
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("방 생성 실패: " + message);
    }

    // 방 목록 갱신 시 호출되는 콜백 (Scroll View 안에 방 목록 표시)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        cachedRoomList = roomList;  // 방 목록을 저장

        // 기존 방 목록 삭제
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 방 목록 추가
        foreach (RoomInfo room in roomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);
            RoomItemUI roomItemUI = roomItem.GetComponent<RoomItemUI>();

            // 비공개 방 여부를 CustomProperties에서 확인
            bool isPrivate = room.CustomProperties != null &&
                             room.CustomProperties.ContainsKey("password") &&
                             !string.IsNullOrEmpty((string)room.CustomProperties["password"]);

            // 방의 핑 정보를 가져옴 (현재 클라이언트의 핑)
            int ping = PhotonNetwork.GetPing();

            // UI에 방 정보 설정 (방 이름, 현재 플레이어 수, 최대 플레이어 수, 비공개 여부, 핑 정보)
            roomItemUI.SetRoomInfo(room.Name, room.PlayerCount, room.MaxPlayers, isPrivate, ping);

            // 비공개 방이면 클릭 시 비밀번호 입력 UI 호출
            if (isPrivate)
            {
                roomItemUI.GetComponent<Button>().onClick.AddListener(() => OnPrivateRoomClicked(room.Name));
            }
            else
            {
                // 공개 방은 바로 입장
                roomItemUI.GetComponent<Button>().onClick.AddListener(() => PhotonNetwork.JoinRoom(room.Name));
            }
        }
    }

    // 비공개 방 클릭 시 호출되는 함수 (비밀번호 입력창 표시)
    public void OnPrivateRoomClicked(string roomName)
    {
        selectedRoomName = roomName;  // 선택된 방 이름 저장
        passwordPanel.SetActive(true);  // 비밀번호 입력 패널 활성화
    }

    // 비밀번호 확인 버튼 클릭 시 호출되는 함수
    public void OnPasswordConfirmButtonClicked()
    {
        string enteredPassword = passwordConfirmInputField.text;

        // 저장된 방 목록에서 선택된 방의 정보를 찾음
        RoomInfo roomInfo = cachedRoomList.FirstOrDefault(r => r.Name == selectedRoomName);
        if (roomInfo != null && roomInfo.CustomProperties.ContainsKey("password"))
        {
            string correctPassword = (string)roomInfo.CustomProperties["password"];
            if (enteredPassword == correctPassword)
            {
                // 비밀번호가 맞으면 방에 입장
                PhotonNetwork.JoinRoom(selectedRoomName);
            }
            else
            {
                Debug.LogError("비밀번호가 틀렸습니다.");
            }
        }
        else
        {
            Debug.LogError("방 정보를 찾을 수 없습니다.");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방에 성공적으로 입장했습니다: " + PhotonNetwork.CurrentRoom.Name);

        // 방에 입장하면 Room UI 패널을 활성화
        roomUIPanel.SetActive(true);

        // 방 이름을 UI에 표시 (RoomUIManager가 관리하는 UI에 방 이름 전달)
        RoomUIManager roomUI = roomUIPanel.GetComponent<RoomUIManager>();
        roomUI.InitializeRoomUI(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnLeftRoom()
    {
        // 방을 떠나면 로비로 돌아감
        roomUIPanel.SetActive(false);
    }

    // 방 나가기 버튼 클릭 시 호출
    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
}
