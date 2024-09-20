using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public int currentCoins = 0;  // 현재 코인 수
    public TextMeshProUGUI coin;

    private void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        Debug.Log("코인 획득: " + amount + " 현재 코인: " + currentCoins);
        UpdateCoinUI();
    }

    // 코인 차감 메서드 추가
    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            UpdateCoinUI();
            Debug.Log("코인 사용: " + amount + " 남은 코인: " + currentCoins);
            return true;
        }
        else
        {
            Debug.LogWarning("코인이 부족합니다!");
            return false;
        }
    }

    private void UpdateCoinUI()
    {
        coin.text = currentCoins.ToString();
    }
}
