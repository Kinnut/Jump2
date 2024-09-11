using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public TMP_Text timerText;  // TextMeshPro UI 컴포넌트
    private float startTime;    // 타이머 시작 시간
    private bool isTimerRunning = true;  // 타이머 실행 여부

    void Start()
    {
        startTime = Time.time;  // 게임이 시작된 시점을 기록
    }

    void Update()
    {
        if (isTimerRunning)
        {
            UpdateTimer();  // 타이머 업데이트
        }
    }

    void UpdateTimer()
    {
        // 게임이 시작된 이후의 시간 계산
        float timeElapsed = Time.time - startTime;

        // 분과 초 계산
        int minutes = Mathf.FloorToInt(timeElapsed / 60f);
        int seconds = Mathf.FloorToInt(timeElapsed % 60f);

        // 00:00 형식으로 타이머 표시
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // 타이머를 중지하고 싶을 때 사용할 함수 (원하는 조건에 맞춰 호출 가능)
    public void StopTimer()
    {
        isTimerRunning = false;
    }

    // 타이머를 다시 시작하고 싶을 때 사용할 함수
    public void StartTimer()
    {
        isTimerRunning = true;
        startTime = Time.time;  // 타이머 다시 시작할 때 시간 초기화
    }
}
