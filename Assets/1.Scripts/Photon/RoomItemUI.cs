using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomItemUI : MonoBehaviour
{
    public TMP_Text roomNameText;        // �� �̸�
    public TMP_Text playerCountText;     // ���� �÷��̾� ��
    public TMP_Text pingText;            // �� ���� �ؽ�Ʈ
    public Image lockImage;              // �ڹ��� �̹���
    public Sprite lockedSprite;          // ��� �ڹ��� �̹���
    public Sprite unlockedSprite;        // ���� �ڹ��� �̹���

    // �� ������ �����ϴ� �Լ�
    public void SetRoomInfo(string roomName, int currentPlayers, int maxPlayers, bool isPrivate, int ping)
    {
        roomNameText.text = roomName;
        playerCountText.text = $"{currentPlayers} / {maxPlayers}";
        lockImage.sprite = isPrivate ? lockedSprite : unlockedSprite;

        // �� ���� ����
        pingText.text = $"{ping} ms";
    }
}
