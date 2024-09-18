using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;                     // ������ ǥ���� TMP
    public int timeScoreInterval = 10;             // ���� �ð����� ��� ���� ����
    public int timeScore = 100;                    // ���� �ð����� ��� ����
    public int killScore = 100;                    // ���� óġ�� �� ��� ����

    private float currentScore = 0f;               // ���� ����
    private float elapsedTime = 0f;                // ��� �ð�

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

    // ���� óġ���� �� ȣ��
    public void AddKillScore()
    {
        currentScore += killScore;
        UpdateScoreUI();
    }

    // ���� ���� ��ȯ
    public float GetCurrentScore()
    {
        return currentScore;
    }

    // ���� UI ������Ʈ
    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + Mathf.FloorToInt(currentScore).ToString();
    }
}
