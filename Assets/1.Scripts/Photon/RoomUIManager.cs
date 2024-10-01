using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomUIManager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomNameText;           // 방 이름 텍스트
    public GameObject[] playerCards;        // 플레이어 카드 (유저 정보를 표시할 슬롯)
    public TMP_Text[] playerNameTexts;      // 각 카드에 들어가는 플레이어 닉네임 텍스트
    public GameObject[] cardFrontPanels;    // 각 카드의 앞부분을 가리는 패널 (새 유저가 들어오기 전 가려지는 패널)

    public Button[] ultimateButtons;        // 1차원 배열로 궁극기 버튼들 (총 플레이어 수 * 각 플레이어의 궁극기 수)
    public Button[] selectionButtons;       // 각 유저의 궁극기 선택 버튼 (하얀 테두리)
    public Button startGameButton;          // 게임 시작 버튼 (방장만 누를 수 있음)

    public Image[] ultimateSelectionImages; // 각 플레이어 중앙의 궁극기 이미지 (중앙에 표시될 이미지)
    public Sprite iceSprite, fireSprite, healSprite; // 궁극기 스프라이트 (얼음, 불, 치유)

    private Dictionary<int, int> selectedUltimates = new Dictionary<int, int>(); // 플레이어별 선택된 궁극기 정보
    private HashSet<int> globallySelectedUltimates = new HashSet<int>(); // 전역적으로 선택된 궁극기 인덱스 저장

    private const int ultimatesPerPlayer = 3;  // 플레이어마다 3개의 궁극기 버튼이 있다고 가정

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        // 방 이름 설정
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        // 플레이어 목록 업데이트
        UpdatePlayerList();

        // 궁극기 선택 버튼 초기화
        for (int i = 0; i < ultimateButtons.Length; i++)
        {
            int buttonIndex = i;
            ultimateButtons[i].onClick.RemoveAllListeners(); // 이전 리스너 삭제
            ultimateButtons[i].onClick.AddListener(() => OnUltimateSelected(buttonIndex));
        }

        // 자신의 궁극기 선택 버튼만 활성화
        for (int i = 0; i < selectionButtons.Length; i++)
        {
            int playerIndex = i;

            // 자신의 버튼만 활성화
            if (PhotonNetwork.LocalPlayer.ActorNumber - 1 == playerIndex)
            {
                selectionButtons[i].interactable = true;  // 자신의 버튼 활성화
            }
            else
            {
                selectionButtons[i].interactable = false; // 다른 유저의 버튼 비활성화
            }
        }

        // 방장 여부에 따라 게임 시작 버튼 활성화
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.interactable = true;
            startGameButton.onClick.AddListener(StartGame);  // 방장이 게임 시작 버튼을 누를 수 있음
        }
        else
        {
            startGameButton.interactable = false;  // 방장이 아닌 경우 게임 시작 버튼 비활성화
        }
    }

    public void InitializeRoomUI(string roomName)
    {
        // 방 이름을 설정
        roomNameText.text = roomName;
        Debug.Log("방 이름: " + roomName);

        // 플레이어 목록 업데이트
        UpdatePlayerList();
    }

    // 플레이어 목록 업데이트 함수
    private void UpdatePlayerList()
    {
        // 모든 슬롯 초기화 (패널을 다시 덮음)
        for (int i = 0; i < playerCards.Length; i++)
        {
            playerNameTexts[i].text = "???";  // 유저 닉네임 비워두기
            cardFrontPanels[i].SetActive(true);  // 패널 활성화
        }

        // 현재 방에 있는 모든 플레이어 목록을 가져옴
        List<Player> players = new List<Player>(PhotonNetwork.CurrentRoom.Players.Values);

        // ActorNumber 순서로 정렬된 플레이어 목록을 가져옴
        players.Sort((x, y) => x.ActorNumber.CompareTo(y.ActorNumber));

        // 각 플레이어를 올바른 슬롯에 배치
        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].NickName;  // 해당 슬롯에 플레이어 닉네임 설정
            cardFrontPanels[i].SetActive(false);  // 패널을 비활성화하여 정보가 보이도록 설정
        }
    }

    // 궁극기 선택 시 호출되는 함수
    public void OnUltimateSelected(int buttonIndex)
    {
        Debug.Log("궁 선택 눌럿음");

        int playerIndex = buttonIndex / ultimatesPerPlayer;  // 플레이어 인덱스 계산
        int ultimateIndex = buttonIndex % ultimatesPerPlayer;  // 해당 플레이어가 선택한 궁극기 인덱스

        // 자신이 속한 슬롯에서만 궁극기 선택 가능
        if (PhotonNetwork.LocalPlayer.ActorNumber - 1 != playerIndex) return;

        // 이미 다른 유저가 해당 궁극기를 선택했으면 선택 불가
        if (globallySelectedUltimates.Contains(ultimateIndex))
        {
            Debug.Log($"Ultimate {ultimateIndex} is already selected by another player.");
            return;
        }

        // 이미 다른 궁극기를 선택한 경우 해제 후 새로운 궁극기 선택
        if (selectedUltimates.ContainsKey(playerIndex))
        {
            int previousUltimate = selectedUltimates[playerIndex];
            photonView.RPC("DeselectUltimate", RpcTarget.AllBuffered, playerIndex, previousUltimate);
        }

        // 새로운 궁극기 선택
        photonView.RPC("SelectUltimate", RpcTarget.AllBuffered, playerIndex, ultimateIndex);
    }

    [PunRPC]
    public void SelectUltimate(int playerIndex, int ultimateIndex)
    {
        // 중복 선택 방지 (로컬 상태 확인 후 처리)
        if (selectedUltimates.ContainsKey(playerIndex) && selectedUltimates[playerIndex] == ultimateIndex)
        {
            Debug.Log($"Player {playerIndex} already selected this ultimate: {ultimateIndex}");
            return;
        }

        Debug.Log($"Player {playerIndex} selects ultimate {ultimateIndex}");

        // 궁극기 선택 상태 저장
        selectedUltimates[playerIndex] = ultimateIndex;
        globallySelectedUltimates.Add(ultimateIndex); // 전역 선택에 추가

        // 로컬 플레이어라면 궁극기 선택 정보를 커스텀 프로퍼티에 저장
        if (PhotonNetwork.LocalPlayer.ActorNumber - 1 == playerIndex)
        {
            // 궁극기 정보를 커스텀 프로퍼티로 저장하여 네트워크 동기화
            Hashtable playerProperties = new Hashtable();
            playerProperties["SelectedUltimate"] = ultimateIndex;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }

        // 궁극기 선택에 따라 중앙 이미지를 변경
        switch (ultimateIndex)
        {
            case 0:
                ultimateSelectionImages[playerIndex].sprite = iceSprite;   // 얼음
                break;
            case 1:
                ultimateSelectionImages[playerIndex].sprite = fireSprite;  // 불
                break;
            case 2:
                ultimateSelectionImages[playerIndex].sprite = healSprite;  // 치유
                break;
        }

        // UI 상태 갱신
        UpdateUltimateUI();
    }


    [PunRPC]
    public void DeselectUltimate(int playerIndex, int ultimateIndex)
    {
        // 궁극기 선택 해제
        selectedUltimates.Remove(playerIndex);
        globallySelectedUltimates.Remove(ultimateIndex); // 전역 선택에서 제거

        Debug.Log($"Player {playerIndex} deselects ultimate {ultimateIndex}");

        // 선택된 궁극기 이미지 초기화
        ultimateSelectionImages[playerIndex].sprite = null;

        // UI 상태 갱신
        UpdateUltimateUI();
    }

    // 궁극기 선택 UI 업데이트
    private void UpdateUltimateUI()
    {
        // 각 플레이어의 궁극기 선택 상태에 따라 UI 업데이트
        foreach (var selected in selectedUltimates)
        {
            int playerIndex = selected.Key;
            int ultimateIndex = selected.Value;

            // 해당 플레이어의 궁극기 버튼을 선택된 상태로 표시
            for (int i = 0; i < ultimatesPerPlayer; i++)
            {
                int buttonIndex = playerIndex * ultimatesPerPlayer + i;
                ultimateButtons[buttonIndex].interactable = (i != ultimateIndex);
            }
        }

        // 전역적으로 선택된 궁극기 처리
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount * ultimatesPerPlayer; i++)
        {
            int ultimateIndex = i % ultimatesPerPlayer;
            if (globallySelectedUltimates.Contains(ultimateIndex))
            {
                ultimateButtons[i].interactable = false;
            }
        }
    }

    // 모든 유저가 궁극기를 선택했는지 확인하는 함수
    private bool AllPlayersSelectedUltimate()
    {
        // 방의 모든 플레이어가 궁극기를 선택했는지 확인
        return PhotonNetwork.CurrentRoom.PlayerCount == selectedUltimates.Count;
    }

    // 게임 시작 버튼 클릭 시 호출되는 함수 (방장만)
    public void StartGame()
    {
        if (AllPlayersSelectedUltimate())
        {
            Debug.Log("모든 플레이어가 궁극기를 선택했으므로 게임을 시작합니다.");

            // 이 메서드를 사용하면 방에 있는 모든 클라이언트가 동기화된 상태로 씬을 로드합니다.
            PhotonNetwork.LoadLevel("1.PlayScene");  // 모든 플레이어가 동기화된 씬 로드
        }
        else
        {
            Debug.Log("모든 플레이어가 궁극기를 선택하지 않았습니다.");
        }
    }


    // 새로운 플레이어가 방에 들어왔을 때 호출되는 콜백
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "이(가) 방에 입장했습니다.");
        UpdatePlayerList();  // 플레이어 목록 갱신
        UpdateUltimateUI();   // 플레이어가 입장하면 궁극기 상태 갱신
    }

    // 플레이어가 방을 나갔을 때 호출되는 콜백
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + "이(가) 방을 떠났습니다.");

        // 플레이어가 선택한 궁극기를 해제
        if (selectedUltimates.ContainsKey(otherPlayer.ActorNumber - 1))
        {
            int ultimateIndex = selectedUltimates[otherPlayer.ActorNumber - 1];
            photonView.RPC("DeselectUltimate", RpcTarget.AllBuffered, otherPlayer.ActorNumber - 1, ultimateIndex);
        }

        UpdatePlayerList();  // 플레이어 목록 갱신
        UpdateUltimateUI();   // 플레이어가 나가면 궁극기 상태 갱신
    }
}
