using UnityEngine;
using TMPro;

public class EndingSceneManager : MonoBehaviour
{
    public TMP_Text scoreText;   // ���� ������ ǥ���� TMP
    public TMP_Text playtimeText; // �÷��� Ÿ���� ǥ���� TMP

    void Start()
    {
        // PlayerPrefs���� ���� ������ �÷��� Ÿ�� �ҷ�����
        float finalScore = PlayerPrefs.GetFloat("FinalScore", 0);
        float playTime = PlayerPrefs.GetFloat("PlayTime", 0);

        // ���� ������ �÷��� Ÿ���� TMP�� ǥ��
        scoreText.text = "���� ����: " + finalScore.ToString();
        playtimeText.text = "�÷��� Ÿ��: " + playTime.ToString("F2") + "��";

        // ���⼭ ��ŷ �ý����� ������Ʈ�� ���� �ֽ��ϴ�.
    }
}
