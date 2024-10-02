using UnityEngine;
using TMPro;
using Photon.Pun;

public class ShopManager : MonoBehaviour
{
    public CrystalAttack crystalAttack;
    public CoinManager coinManager;  // 코인 매니저 참조
    private MyPlayer player;          // 플레이어 참조 (힐 및 속도 증가)

    // 아이템 가격
    public int bulletUpgradeCost = 300;
    public int healCost = 150;
    public int speedBoostCost = 200;
    public int crystalAttackCost = 500;

    // MyPlayer.cs (플레이어 스크립트)
    public void SetPlayer(MyPlayer localPlayer)
    {
        player = localPlayer;
    }

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

    // 크리스탈 공격 구매
    public void BuyCrystalAttack()
    {
        if (coinManager.SpendCoins(crystalAttackCost))
        {
            crystalAttack.ActivateCrystalAttack(30f);  // 크리스탈 30초 동안 공격
        }
    }
}
