using UnityEngine;
using TMPro;
using Photon.Pun;

public class ShopManager : MonoBehaviour
{
    public CrystalAttack crystalAttack;
    public CoinManager coinManager;  // ���� �Ŵ��� ����
    private MyPlayer player;          // �÷��̾� ���� (�� �� �ӵ� ����)
    private PlayerMovement playerMovement; // PlayerMovement ��ũ��Ʈ ����

    // ������ ����
    public int bulletUpgradeCost = 300;
    public int healCost = 150;
    public int speedBoostCost = 200;
    public int crystalAttackCost = 500;
    public int questionCost = 300;   // Question �������� ����

    // TMP ������Ʈ �߰�
    public TextMeshProUGUI bulletCostText; // �Ѿ� ��ȭ ������ ǥ���� TMP �ؽ�Ʈ
    public TextMeshProUGUI feedbackText;   // ���� ����/���� �޽����� ǥ���� TMP �ؽ�Ʈ

    // MyPlayer.cs (�÷��̾� ��ũ��Ʈ)
    public void SetPlayer(MyPlayer localPlayer)
    {
        player = localPlayer;
        playerMovement = localPlayer.GetComponent<PlayerMovement>();

        UpdateBulletCostText();  // ó�� ���� ���� �� TMP �ؽ�Ʈ ������Ʈ
    }

    // �Ѿ� ��ȭ
    public void BuyBulletUpgrade()
    {
        if (coinManager.SpendCoins(bulletUpgradeCost))
        {
            player.StrengthenBasicAttack(); // �Ѿ� ��ȭ
            bulletUpgradeCost += 500;  // ���� ���� �� 500�� ����
            UpdateBulletCostText();    // TMP �ؽ�Ʈ ������Ʈ
            ShowFeedbackMessage("���� ����!", 1f); // ���� ���� �޽��� ǥ��
        }
        else
        {
            ShowFeedbackMessage("���� ����!", 1f); // ���� ���� �޽��� ǥ��
        }
    }

    // ü�� ȸ��
    public void BuyHeal()
    {
        if (coinManager.SpendCoins(healCost))
        {
            player.Heal(40f);  // ü�� 40% ȸ��
            ShowFeedbackMessage("���� ����!", 1f);
        }
        else
        {
            ShowFeedbackMessage("���� ����!", 1f);
        }
    }

    // �ӵ� ����
    public void BuySpeedBoost()
    {
        if (coinManager.SpendCoins(speedBoostCost))
        {
            // PlayerMovement���� �ӵ� ���� ó��
            playerMovement.IncreaseSpeed(0.1f * playerMovement.moveSpeed, 30f); // 30�� ���� �̵� �ӵ� 10% ����
            ShowFeedbackMessage("���� ����!", 1f);
        }
        else
        {
            ShowFeedbackMessage("���� ����!", 1f);
        }
    }

    // ũ����Ż ���� ����
    public void BuyCrystalAttack()
    {
        if (coinManager.SpendCoins(crystalAttackCost))
        {
            crystalAttack.ActivateCrystalAttack(30f);  // ũ����Ż 30�� ���� ����
            ShowFeedbackMessage("���� ����!", 1f);
        }
        else
        {
            ShowFeedbackMessage("���� ����!", 1f);
        }
    }

    // Question ������ ����
    public void BuyQuestion()
    {
        if (coinManager.SpendCoins(questionCost))
        {
            ApplyRandomEffect();  // ������ ȿ�� ����
            ShowFeedbackMessage("���� ����!", 1f);
        }
        else
        {
            ShowFeedbackMessage("���� ����!", 1f);
        }
    }

    // �� ���� ȿ�� �� �ϳ��� �������� ����
    private void ApplyRandomEffect()
    {
        int randomEffect = Random.Range(0, 4);  // 0~3 ������ ��

        switch (randomEffect)
        {
            case 0:
                player.StrengthenBasicAttack();
                Debug.Log("���� ȿ��: �Ѿ� ��ȭ");
                break;
            case 1:
                player.Heal(40f);  // 40% ȸ��
                Debug.Log("���� ȿ��: ü�� ȸ��");
                break;
            case 2:
                playerMovement.IncreaseSpeed(0.1f * playerMovement.moveSpeed, 30f); // 30�� ���� �̵� �ӵ� 10% ����
                Debug.Log("���� ȿ��: �ӵ� ����");
                break;
            case 3:
                crystalAttack.ActivateCrystalAttack(30f);  // ũ����Ż 30�� ���� ����
                Debug.Log("���� ȿ��: ũ����Ż ����");
                break;
        }
    }

    // TMP �ؽ�Ʈ�� ������Ʈ�ϴ� �޼���
    private void UpdateBulletCostText()
    {
        if (bulletCostText != null)
        {
            bulletCostText.text = bulletUpgradeCost.ToString(); // ��: "300 Coins"
        }
    }

    // ���� ����/���� �޽��� ǥ��
    private void ShowFeedbackMessage(string message, float duration)
    {
        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);  // �޽��� Ȱ��ȭ
        Invoke("HideFeedbackMessage", duration);  // ���� �ð� �� �޽��� ��Ȱ��ȭ
    }

    // ���� ����/���� �޽��� ����
    private void HideFeedbackMessage()
    {
        feedbackText.gameObject.SetActive(false);
    }
}
