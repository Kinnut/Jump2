using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public int timeScoreInterval = 10;
    public int timeScore = 100;
    public int killScore = 100;

    private float currentScore = 0f;
    private float elapsedTime = 0f;

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

    public void AddKillScore()
    {
        currentScore += killScore;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + Mathf.FloorToInt(currentScore).ToString();
    }
}
