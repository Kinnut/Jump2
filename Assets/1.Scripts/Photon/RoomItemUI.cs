using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomItemUI : MonoBehaviour
{
    // �� ���� UI ��ҵ�
    public TMP_Text roomNameText;        // �� �̸� �ؽ�Ʈ
    public TMP_Text playerCountText;     // ���� �÷��̾� �� �ؽ�Ʈ
    public TMP_Text pingText;            // �� ���� �ؽ�Ʈ
    public Image lockImage;              // �� ��� ���� �̹���
    public Sprite lockedSprite;          // ��� �� ��������Ʈ
    public Sprite unlockedSprite;        // ���� �� ��������Ʈ

    // �� ������ �����ϴ� �Լ�
    public void SetRoomInfo(string roomName, int currentPlayers, int maxPlayers, bool isPrivate, int ping)
    {
        roomNameText.text = roomName;  // �� �̸� ����
        playerCountText.text = $"{currentPlayers} / {maxPlayers}";  // �÷��̾� �� ǥ��
        lockImage.sprite = isPrivate ? lockedSprite : unlockedSprite;  // �� ��� ���� �̹��� ����
        pingText.text = $"{ping} ms";  // �� ���� ǥ��
    }
}
