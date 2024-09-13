using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource sfxSource;          // 효과음을 재생할 AudioSource
    public Slider bgmVolumeSlider;         // 배경음악 볼륨 슬라이더
    public Slider sfxVolumeSlider;         // 효과음 볼륨 슬라이더
    public BGMChanger bgmChanger;          // BGMChanger 참조

    private float defaultBgmVolume = 1f;   // 기본 배경음악 볼륨
    private float defaultSfxVolume = 1f;   // 기본 효과음 볼륨

    void Start()
    {
        // 유저가 저장한 볼륨을 불러오거나 기본값 사용
        defaultBgmVolume = PlayerPrefs.GetFloat("BGMVolume", defaultBgmVolume);
        defaultSfxVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSfxVolume);

        // 슬라이더 초기화 및 현재 설정된 볼륨 적용
        bgmVolumeSlider.value = defaultBgmVolume;
        sfxVolumeSlider.value = defaultSfxVolume;

        // BGMChanger에 초기 볼륨 설정
        bgmChanger.UpdateUserVolume(defaultBgmVolume);

        // 효과음 초기 볼륨 설정
        sfxSource.volume = defaultSfxVolume;

        // 슬라이더 값이 변경될 때 호출
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    // 배경음악 볼륨이 변경되었을 때
    public void OnBGMVolumeChanged(float newVolume)
    {
        // BGMChanger에 배경음악 볼륨을 업데이트
        bgmChanger.UpdateUserVolume(newVolume);

        // 유저 설정값을 저장
        PlayerPrefs.SetFloat("BGMVolume", newVolume);
        PlayerPrefs.Save();
    }

    // 효과음 볼륨이 변경되었을 때
    public void OnSFXVolumeChanged(float newVolume)
    {
        sfxSource.volume = newVolume;

        // 유저 설정값을 저장
        PlayerPrefs.SetFloat("SFXVolume", newVolume);
        PlayerPrefs.Save();
    }

    // 버튼 클릭 시 사운드 재생
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

}
