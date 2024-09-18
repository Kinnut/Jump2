using UnityEngine;
using TMPro;

public class EndingSceneManager : MonoBehaviour
{
    public TMP_Text scoreText;   // 최종 점수를 표시할 TMP
    public TMP_Text playtimeText; // 플레이 타임을 표시할 TMP

    void Start()
    {
        // PlayerPrefs에서 최종 점수와 플레이 타임 불러오기
        float finalScore = PlayerPrefs.GetFloat("FinalScore", 0);
        float playTime = PlayerPrefs.GetFloat("PlayTime", 0);

        // 최종 점수와 플레이 타임을 TMP에 표시
        scoreText.text = "최종 점수: " + finalScore.ToString();
        playtimeText.text = "플레이 타임: " + playTime.ToString("F2") + "초";

        // 여기서 랭킹 시스템을 업데이트할 수도 있습니다.
    }
}
