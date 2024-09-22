using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public CrystalAttack crystalAttack;
    public CoinManager coinManager;  // ���� �Ŵ��� ����
    public Player player;            // �÷��̾� ���� (�� �� �ӵ� ����)

    // ������ ����
    public int bulletUpgradeCost = 300;
    public int healCost = 150;
    public int speedBoostCost = 200;
    public int crystalAttackCost = 500;

    // �Ѿ� ��ȭ
    public void BuyBulletUpgrade()
    {
        if (coinManager.SpendCoins(bulletUpgradeCost))
        {
            player.StrengthenBasicAttack(); // �Ѿ� ��ȭ
        }
    }

    // ü�� ȸ��
    public void BuyHeal()
    {
        if (coinManager.SpendCoins(healCost))
        {
            player.Heal(40f);  // ü�� 40% ȸ��
        }
    }

    // �ӵ� ����
    public void BuySpeedBoost()
    {
        if (coinManager.SpendCoins(speedBoostCost))
        {
            player.IncreaseSpeed(0.1f * player.speed, 30f); // 30�� ���� �̵� �ӵ� 10% ����
        }
    }

    public void BuyCrystalAttack()
    {
        if (coinManager.SpendCoins(crystalAttackCost))
        {
            crystalAttack.ActivateCrystalAttack(30f);  // ũ����Ż 30�� ���� ����
        }
    }
}
