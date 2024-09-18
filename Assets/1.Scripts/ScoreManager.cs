using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;                     // 점수를 표시할 TMP
    public int timeScoreInterval = 10;             // 일정 시간마다 얻는 점수 간격
    public int timeScore = 100;                    // 일정 시간마다 얻는 점수
    public int killScore = 100;                    // 적을 처치할 때 얻는 점수

    private float currentScore = 0f;               // 현재 점수
    private float elapsedTime = 0f;                // 경과 시간

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeScoreInterval)
        {
            currentScore += timeScore;
            elapsedTime = 0f;
            UpdateScoreUI();
        }
    }

    // 적을 처치했을 때 호출
    public void AddKillScore()
    {
        currentScore += killScore;
        UpdateScoreUI();
    }

    // 현재 점수 반환
    public float GetCurrentScore()
    {
        return currentScore;
    }

    // 점수 UI 업데이트
    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + Mathf.FloorToInt(currentScore).ToString();
    }
}
