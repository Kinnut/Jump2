using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public CoinManager coinManager;  // 코인 매니저 참조
    public Player player;            // 플레이어 참조 (힐 및 속도 증가)

    // 아이템 가격
    public int bulletUpgradeCost = 300;
    public int healCost = 150;
    public int speedBoostCost = 200;

    // 총알 강화
    public void BuyBulletUpgrade()
    {
        if (coinManager.SpendCoins(bulletUpgradeCost))
        {
            player.StrengthenBasicAttack(); // 총알 강화
        }
    }

    // 체력 회복
    public void BuyHeal()
    {
        if (coinManager.SpendCoins(healCost))
        {
            player.Heal(40f);  // 체력 40% 회복
        }
    }

    // 속도 증가
    public void BuySpeedBoost()
    {
        if (coinManager.SpendCoins(speedBoostCost))
        {
            player.IncreaseSpeed(0.1f * player.speed, 30f); // 30초 동안 이동 속도 10% 증가
        }
    }
}
