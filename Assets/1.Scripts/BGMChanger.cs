using UnityEngine;
using System.Collections;

public class BGMChanger : MonoBehaviour
{
    public AudioSource audioSource1;    // 첫 번째 AudioSource
    public AudioSource audioSource2;    // 두 번째 AudioSource
    public AudioClip[] backgroundMusic; // 배경음악 클립 배열
    public float changeInterval = 60f;  // 음악이 바뀌는 시간 간격 (초 단위)
    public float fadeDuration = 2f;     // 페이드 시간 (초 단위)

    private int currentMusicIndex = 0;
    private bool isPlayingSource1 = true; // 현재 어느 오디오 소스가 재생 중인지
    private float elapsedTime = 0f;       // 경과 시간 (Time.timeScale에 따른)

    [Range(0f, 1f)]
    public float userVolume = 1f;  // 유저가 설정한 볼륨 (0 ~ 1 사이)

    void Start()
    {
        // AudioSource 초기 설정
        audioSource1.clip = backgroundMusic[currentMusicIndex];
        audioSource1.volume = userVolume;
        audioSource2.volume = userVolume;  // 두 번째 소스에도 초기값 적용
        audioSource1.Play();
        StartCoroutine(ChangeMusicRoutine());
    }

    IEnumerator ChangeMusicRoutine()
    {
        while (true)
        {
            // Time.timeScale에 영향을 받지 않고 경과 시간을 계산 (Realtime으로 계산)
            while (elapsedTime < changeInterval)
            {
                if (Time.timeScale > 0)
                {
                    elapsedTime += Time.unscaledDeltaTime;  // Time.timeScale의 영향을 받지 않음
                }
                yield return null;  // 매 프레임 대기
            }

            // 음악을 변경하는 코드
            ChangeMusic();

            // 경과 시간 초기화
            elapsedTime = 0f;
        }
    }

    void ChangeMusic()
    {
        // 다음 음악으로 전환
        currentMusicIndex = (currentMusicIndex + 1) % backgroundMusic.Length;
        AudioSource nextSource = isPlayingSource1 ? audioSource2 : audioSource1;

        nextSource.clip = backgroundMusic[currentMusicIndex];
        nextSource.volume = 0f;  // 페이드 인을 위해 초기 볼륨 0으로 설정
        nextSource.Play();

        // 페이드 전환 시작
        StartCoroutine(FadeMusic(isPlayingSource1 ? audioSource1 : audioSource2, nextSource));

        // 다음 소스로 전환
        isPlayingSource1 = !isPlayingSource1;
    }

    IEnumerator FadeMusic(AudioSource fromSource, AudioSource toSource)
    {
        float timer = 0f;

        // 서서히 볼륨을 줄이고 올리기
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;  // Time.timeScale에 영향 받지 않음
            float progress = timer / fadeDuration;

            fromSource.volume = Mathf.Lerp(userVolume, 0f, progress);  // 기존 음악의 볼륨 감소
            toSource.volume = Mathf.Lerp(0f, userVolume, progress);    // 새로운 음악의 볼륨 증가

            yield return null;
        }

        fromSource.volume = 0f;
        fromSource.Stop();  // 기존 음악 정지
        toSource.volume = userVolume;  // 새로운 음악의 볼륨을 유저 설정 값으로 유지
    }

    // 유저 볼륨 업데이트 함수
    public void UpdateUserVolume(float newVolume)
    {
        userVolume = newVolume;

        // 두 AudioSource에 모두 새로운 볼륨 적용
        audioSource1.volume = userVolume;
        audioSource2.volume = userVolume;
    }
}
