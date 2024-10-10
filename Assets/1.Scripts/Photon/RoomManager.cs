using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class RoomManager : MonoBehaviourPunCallbacks
{
    // UI 요소들
    public TMP_InputField roomNameInputField;  // 방 이름 입력 필드
    public TMP_InputField passwordInputField;  // 비밀번호 입력 필드
    public Toggle privateRoomToggle;           // 비공개 방 여부 토글 버튼
    public Transform roomListContent;          // 방 목록을 표시할 Content 영역
    public GameObject roomListItemPrefab;      // 방 목록 항목 프리팹
    public GameObject passwordPanel;           // 비밀번호 입력 패널
    public TMP_InputField passwordConfirmInputField;  // 비밀번호 확인 필드
    public Button passwordConfirmButton;       // 비밀번호 확인 버튼
    public GameObject roomUIPanel;             // 방 UI 패널
    public GameObject warningPanel;            // 경고 메시지 패널
    public TMP_Text warningText;               // 경고 메시지 텍스트

    private const int maxPlayers = 3;          // 방의 최대 플레이어 수
    private const int passwordLength = 4;      // 비밀번호 길이 제한
    private string selectedRoomName;           // 선택된 방 이름
    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();  // 방 목록 캐시

    void Start()
    {
        // 비공개 방 여부에 따른 비밀번호 입력 필드 활성화
        privateRoomToggle.onValueChanged.AddListener(delegate { TogglePasswordField(privateRoomToggle.isOn); });

        // 시작 시 비밀번호 필드 비활성화
        TogglePasswordField(false);

        // 비밀번호 확인 버튼 이벤트 설정
        passwordConfirmButton.onClick.AddListener(OnPasswordConfirmButtonClicked);

        // 기본적으로 경고 패널 비활성화
        warningPanel.SetActive(false);
    }

    // 경고 메시지 표시 함수
    private void ShowWarning(string message)
    {
        warningText.text = message;
        warningPanel.SetActive(true);  // 경고 패널 활성화
    }

    // 방 목록 새로고침
    public void OnRefreshButtonClicked()
    {
        PhotonNetwork.JoinLobby();  // 로비에 다시 접속하여 방 목록 갱신
    }

    // 비공개 방 여부에 따른 비밀번호 필드 활성화/비활성화
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

        // 방 이름이 없는 경우 경고 메시지 출력
        if (string.IsNullOrEmpty(roomName))
        {
            ShowWarning("방 이름을 입력하세요.");
            return;
        }

        // 비공개 방 비밀번호 유효성 검사
        if (isPrivate)
        {
            if (password.Length != passwordLength || !int.TryParse(password, out _))
            {
                ShowWarning("비밀번호는 4자리 숫자여야 합니다.");
                return;
            }
        }

        // Photon RoomOptions 설정
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;  // 최대 플레이어 수 설정
        roomOptions.IsVisible = true;  // 방 목록에 표시 (공개/비공개 무관)
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add("password", isPrivate ? password : "");  // 비밀번호 저장
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };  // 로비에서 비밀번호 정보 노출

        PhotonNetwork.CreateRoom(roomName, roomOptions);  // 방 생성
    }

    // 방 생성 성공 시 호출되는 콜백
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공: " + PhotonNetwork.CurrentRoom.Name);
    }

    // 방 생성 실패 시 호출되는 콜백
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ShowWarning("방 생성 실패: " + message);  // 경고 메시지 출력
    }

    // 방 목록 갱신
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        cachedRoomList = roomList;  // 방 목록 캐싱

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

            // 비공개 방 여부 확인
            bool isPrivate = room.CustomProperties != null &&
                             room.CustomProperties.ContainsKey("password") &&
                             !string.IsNullOrEmpty((string)room.CustomProperties["password"]);

            int ping = PhotonNetwork.GetPing();  // 핑 정보

            roomItemUI.SetRoomInfo(room.Name, room.PlayerCount, room.MaxPlayers, isPrivate, ping);  // 방 정보 설정

            // 비공개 방 클릭 시 비밀번호 입력 UI 표시
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

    // 비공개 방 클릭 시 비밀번호 입력 패널 표시
    public void OnPrivateRoomClicked(string roomName)
    {
        selectedRoomName = roomName;
        passwordPanel.SetActive(true);  // 비밀번호 입력 패널 활성화
    }

    // 비밀번호 확인 버튼 클릭 시 호출
    public void OnPasswordConfirmButtonClicked()
    {
        string enteredPassword = passwordConfirmInputField.text;

        // 선택된 방의 비밀번호 확인
        RoomInfo roomInfo = cachedRoomList.FirstOrDefault(r => r.Name == selectedRoomName);
        if (roomInfo != null && roomInfo.CustomProperties.ContainsKey("password"))
        {
            string correctPassword = (string)roomInfo.CustomProperties["password"];
            if (enteredPassword == correctPassword)
            {
                PhotonNetwork.JoinRoom(selectedRoomName);  // 비밀번호 일치 시 방 입장
            }
            else
            {
                ShowWarning("비밀번호가 틀렸습니다.");  // 경고 메시지 출력
            }
        }
        else
        {
            ShowWarning("방 정보를 찾을 수 없습니다.");  // 방 정보가 없는 경우
        }
    }

    // 방 입장 성공 시 호출
    public override void OnJoinedRoom()
    {
        Debug.Log("방에 성공적으로 입장했습니다: " + PhotonNetwork.CurrentRoom.Name);
        roomUIPanel.SetActive(true);  // 방 UI 패널 활성화
    }

    // 방 나가기 버튼 클릭 시 호출
    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
}
