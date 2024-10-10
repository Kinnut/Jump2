using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomItemUI : MonoBehaviour
{
    // 방 정보 UI 요소들
    public TMP_Text roomNameText;        // 방 이름 텍스트
    public TMP_Text playerCountText;     // 현재 플레이어 수 텍스트
    public TMP_Text pingText;            // 핑 정보 텍스트
    public Image lockImage;              // 방 잠김 여부 이미지
    public Sprite lockedSprite;          // 잠긴 방 스프라이트
    public Sprite unlockedSprite;        // 열린 방 스프라이트

    // 방 정보를 설정하는 함수
    public void SetRoomInfo(string roomName, int currentPlayers, int maxPlayers, bool isPrivate, int ping)
    {
        roomNameText.text = roomName;  // 방 이름 설정
        playerCountText.text = $"{currentPlayers} / {maxPlayers}";  // 플레이어 수 표시
        lockImage.sprite = isPrivate ? lockedSprite : unlockedSprite;  // 방 잠김 여부 이미지 설정
        pingText.text = $"{ping} ms";  // 핑 정보 표시
    }
}
