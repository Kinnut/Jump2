using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource sfxSource;          // ȿ������ ����� AudioSource
    public Slider bgmVolumeSlider;         // ������� ���� �����̴�
    public Slider sfxVolumeSlider;         // ȿ���� ���� �����̴�
    public BGMChanger bgmChanger;          // BGMChanger ����

    private float defaultBgmVolume = 1f;   // �⺻ ������� ����
    private float defaultSfxVolume = 1f;   // �⺻ ȿ���� ����

    void Start()
    {
        // ������ ������ ������ �ҷ����ų� �⺻�� ���
        defaultBgmVolume = PlayerPrefs.GetFloat("BGMVolume", defaultBgmVolume);
        defaultSfxVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSfxVolume);

        // �����̴� �ʱ�ȭ �� ���� ������ ���� ����
        bgmVolumeSlider.value = defaultBgmVolume;
        sfxVolumeSlider.value = defaultSfxVolume;

        // BGMChanger�� �ʱ� ���� ����
        bgmChanger.UpdateUserVolume(defaultBgmVolume);

        // ȿ���� �ʱ� ���� ����
        sfxSource.volume = defaultSfxVolume;

        // �����̴� ���� ����� �� ȣ��
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    // ������� ������ ����Ǿ��� ��
    public void OnBGMVolumeChanged(float newVolume)
    {
        // BGMChanger�� ������� ������ ������Ʈ
        bgmChanger.UpdateUserVolume(newVolume);

        // ���� �������� ����
        PlayerPrefs.SetFloat("BGMVolume", newVolume);
        PlayerPrefs.Save();
    }

    // ȿ���� ������ ����Ǿ��� ��
    public void OnSFXVolumeChanged(float newVolume)
    {
        sfxSource.volume = newVolume;

        // ���� �������� ����
        PlayerPrefs.SetFloat("SFXVolume", newVolume);
        PlayerPrefs.Save();
    }

    // ��ư Ŭ�� �� ���� ���
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

}
