using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public TMP_Text timerText;  // TextMeshPro UI ������Ʈ
    private float startTime;    // Ÿ�̸� ���� �ð�
    private bool isTimerRunning = true;  // Ÿ�̸� ���� ����

    void Start()
    {
        startTime = Time.time;  // ������ ���۵� ������ ���
    }

    void Update()
    {
        if (isTimerRunning)
        {
            UpdateTimer();  // Ÿ�̸� ������Ʈ
        }
    }

    void UpdateTimer()
    {
        // ������ ���۵� ������ �ð� ���
        float timeElapsed = Time.time - startTime;

        // �а� �� ���
        int minutes = Mathf.FloorToInt(timeElapsed / 60f);
        int seconds = Mathf.FloorToInt(timeElapsed % 60f);

        // 00:00 �������� Ÿ�̸� ǥ��
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Ÿ�̸Ӹ� �����ϰ� ���� �� ����� �Լ� (���ϴ� ���ǿ� ���� ȣ�� ����)
    public void StopTimer()
    {
        isTimerRunning = false;
    }

    // Ÿ�̸Ӹ� �ٽ� �����ϰ� ���� �� ����� �Լ�
    public void StartTimer()
    {
        isTimerRunning = true;
        startTime = Time.time;  // Ÿ�̸� �ٽ� ������ �� �ð� �ʱ�ȭ
    }
}
