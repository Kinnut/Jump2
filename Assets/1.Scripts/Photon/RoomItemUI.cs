using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomItemUI : MonoBehaviour
{
    public TMP_Text roomNameText;        // 방 이름
    public TMP_Text playerCountText;     // 현재 플레이어 수
    public TMP_Text pingText;            // 핑 정보 텍스트
    public Image lockImage;              // 자물쇠 이미지
    public Sprite lockedSprite;          // 잠긴 자물쇠 이미지
    public Sprite unlockedSprite;        // 열린 자물쇠 이미지

    // 방 정보를 설정하는 함수
    public void SetRoomInfo(string roomName, int currentPlayers, int maxPlayers, bool isPrivate, int ping)
    {
        roomNameText.text = roomName;
        playerCountText.text = $"{currentPlayers} / {maxPlayers}";
        lockImage.sprite = isPrivate ? lockedSprite : unlockedSprite;

        // 핑 정보 설정
        pingText.text = $"{ping} ms";
    }
}
