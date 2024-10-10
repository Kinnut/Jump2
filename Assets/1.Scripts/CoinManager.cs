using Photon.Pun;
using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviourPun
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

    // 모든 플레이어에게 코인을 분배하는 RPC 메서드 추가
    [PunRPC]
    public void DistributeCoins(int amount)
    {
        AddCoins(amount);
    }

    private void UpdateCoinUI()
    {
        coin.text = currentCoins.ToString();
    }
}
