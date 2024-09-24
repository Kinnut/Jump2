using UnityEngine;
using TMPro;  // TextMeshPro 사용
using Photon.Pun;  // Photon 사용
using Photon.Realtime;  // Photon의 Room 관련 기능 사용
using System.Collections.Generic;  // List 사용

public class RoomUIManager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomNameText;           // 방 이름 텍스트
    public GameObject[] playerCards;        // 플레이어 카드 (유저 정보를 표시할 슬롯)
    public TMP_Text[] playerNameTexts;      // 각 카드에 들어가는 플레이어 닉네임 텍스트
    public GameObject[] cardFrontPanels;    // 각 카드의 앞부분을 가리는 패널 (새 유저가 들어오기 전 가려지는 패널)

    void Start()
    {
        // 방 이름 설정
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        // 플레이어 목록 업데이트
        UpdatePlayerList();
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

        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].NickName;  // 해당 슬롯에 플레이어 닉네임 설정
            cardFrontPanels[i].SetActive(false);  // 패널을 비활성화하여 정보가 보이도록 설정
        }
    }

    // 새로운 플레이어가 방에 들어왔을 때 호출되는 콜백
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "이(가) 방에 입장했습니다.");
        UpdatePlayerList();  // 플레이어 목록 갱신
    }

    // 플레이어가 방을 나갔을 때 호출되는 콜백
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + "이(가) 방을 떠났습니다.");
        UpdatePlayerList();  // 플레이어 목록 갱신
    }
}
