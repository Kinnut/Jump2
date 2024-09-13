using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip buttonClickSound;   // 버튼 클릭 시 재생될 사운드
    private SoundManager soundManager;   // SoundManager 참조

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();  // SoundManager 스크립트를 찾아서 연결
    }

    // 버튼 클릭 시 호출
    public void OnButtonClick()
    {
        soundManager.PlaySFX(buttonClickSound);  // 버튼 클릭 사운드 재생
    }
}
