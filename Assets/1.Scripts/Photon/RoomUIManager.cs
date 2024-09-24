using UnityEngine;
using TMPro;
using Photon.Pun;

public class RoomUIManager : MonoBehaviour
{
    public TMP_Text roomNameText;  // 방 이름 텍스트

    // 방 이름과 플레이어 목록을 초기화하는 함수
    public void InitializeRoomUI(string roomName)
    {
        // 방 이름을 설정
        roomNameText.text = roomName;
        Debug.Log("방 이름: " + roomName);

        // 플레이어 목록 업데이트
        UpdatePlayerList();
    }

    // 플레이어 목록 업데이트 함수 (추후 구현)
    private void UpdatePlayerList()
    {
        // 예: PhotonNetwork.PlayerList로 플레이어 목록을 가져와 표시
        Debug.Log("플레이어 목록을 업데이트합니다.");
    }
}
