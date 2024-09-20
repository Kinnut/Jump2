using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public int currentCoins = 0;  // ���� ���� ��
    public TextMeshProUGUI coin;

    private void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        Debug.Log("���� ȹ��: " + amount + " ���� ����: " + currentCoins);
        UpdateCoinUI();
    }

    // ���� ���� �޼��� �߰�
    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            UpdateCoinUI();
            Debug.Log("���� ���: " + amount + " ���� ����: " + currentCoins);
            return true;
        }
        else
        {
            Debug.LogWarning("������ �����մϴ�!");
            return false;
        }
    }

    private void UpdateCoinUI()
    {
        coin.text = currentCoins.ToString();
    }
}
