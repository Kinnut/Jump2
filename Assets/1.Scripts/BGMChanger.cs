using UnityEngine;
using System.Collections;

public class BGMChanger : MonoBehaviour
{
    public AudioSource audioSource1;    // ù ��° AudioSource
    public AudioSource audioSource2;    // �� ��° AudioSource
    public AudioClip[] backgroundMusic; // ������� Ŭ�� �迭
    public float changeInterval = 60f;  // ������ �ٲ�� �ð� ���� (�� ����)
    public float fadeDuration = 2f;     // ���̵� �ð� (�� ����)

    private int currentMusicIndex = 0;
    private bool isPlayingSource1 = true; // ���� ��� ����� �ҽ��� ��� ������
    private float elapsedTime = 0f;       // ��� �ð� (Time.timeScale�� ����)

    [Range(0f, 1f)]
    public float userVolume = 1f;  // ������ ������ ���� (0 ~ 1 ����)

    void Start()
    {
        // AudioSource �ʱ� ����
        audioSource1.clip = backgroundMusic[currentMusicIndex];
        audioSource1.volume = userVolume;
        audioSource2.volume = userVolume;  // �� ��° �ҽ����� �ʱⰪ ����
        audioSource1.Play();
        StartCoroutine(ChangeMusicRoutine());
    }

    IEnumerator ChangeMusicRoutine()
    {
        while (true)
        {
            // Time.timeScale�� ������ ���� �ʰ� ��� �ð��� ��� (Realtime���� ���)
            while (elapsedTime < changeInterval)
            {
                if (Time.timeScale > 0)
                {
                    elapsedTime += Time.unscaledDeltaTime;  // Time.timeScale�� ������ ���� ����
                }
                yield return null;  // �� ������ ���
            }

            // ������ �����ϴ� �ڵ�
            ChangeMusic();

            // ��� �ð� �ʱ�ȭ
            elapsedTime = 0f;
        }
    }

    void ChangeMusic()
    {
        // ���� �������� ��ȯ
        currentMusicIndex = (currentMusicIndex + 1) % backgroundMusic.Length;
        AudioSource nextSource = isPlayingSource1 ? audioSource2 : audioSource1;

        nextSource.clip = backgroundMusic[currentMusicIndex];
        nextSource.volume = 0f;  // ���̵� ���� ���� �ʱ� ���� 0���� ����
        nextSource.Play();

        // ���̵� ��ȯ ����
        StartCoroutine(FadeMusic(isPlayingSource1 ? audioSource1 : audioSource2, nextSource));

        // ���� �ҽ��� ��ȯ
        isPlayingSource1 = !isPlayingSource1;
    }

    IEnumerator FadeMusic(AudioSource fromSource, AudioSource toSource)
    {
        float timer = 0f;

        // ������ ������ ���̰� �ø���
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;  // Time.timeScale�� ���� ���� ����
            float progress = timer / fadeDuration;

            fromSource.volume = Mathf.Lerp(userVolume, 0f, progress);  // ���� ������ ���� ����
            toSource.volume = Mathf.Lerp(0f, userVolume, progress);    // ���ο� ������ ���� ����

            yield return null;
        }

        fromSource.volume = 0f;
        fromSource.Stop();  // ���� ���� ����
        toSource.volume = userVolume;  // ���ο� ������ ������ ���� ���� ������ ����
    }

    // ���� ���� ������Ʈ �Լ�
    public void UpdateUserVolume(float newVolume)
    {
        userVolume = newVolume;

        // �� AudioSource�� ��� ���ο� ���� ����
        audioSource1.volume = userVolume;
        audioSource2.volume = userVolume;
    }
}
