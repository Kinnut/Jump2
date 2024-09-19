using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int currentCoins = 0;  // 현재 코인 수

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        Debug.Log("코인 획득: " + amount + " 현재 코인: " + currentCoins);
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        // 코인 수를 UI에 업데이트하는 코드
        // 예: TextMeshProUGUI 컴포넌트를 사용하여 UI 업데이트
    }
}
