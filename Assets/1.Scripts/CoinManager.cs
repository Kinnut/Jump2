using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int currentCoins = 0;  // ���� ���� ��

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        Debug.Log("���� ȹ��: " + amount + " ���� ����: " + currentCoins);
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        // ���� ���� UI�� ������Ʈ�ϴ� �ڵ�
        // ��: TextMeshProUGUI ������Ʈ�� ����Ͽ� UI ������Ʈ
    }
}
