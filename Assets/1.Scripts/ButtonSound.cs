using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip buttonClickSound;   // ��ư Ŭ�� �� ����� ����
    private SoundManager soundManager;   // SoundManager ����

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();  // SoundManager ��ũ��Ʈ�� ã�Ƽ� ����
    }

    // ��ư Ŭ�� �� ȣ��
    public void OnButtonClick()
    {
        soundManager.PlaySFX(buttonClickSound);  // ��ư Ŭ�� ���� ���
    }
}
