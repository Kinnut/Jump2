using UnityEngine;
using TMPro;
using Photon.Pun;

public class ShopManager : MonoBehaviour
{
    public CrystalAttack crystalAttack;
    public CoinManager coinManager;  // 코인 매니저 참조
    private MyPlayer player;          // 플레이어 참조 (힐 및 속도 증가)
    private PlayerMovement playerMovement; // PlayerMovement 스크립트 참조

    // 아이템 가격
    public int bulletUpgradeCost = 300;
    public int healCost = 150;
    public int speedBoostCost = 200;
    public int crystalAttackCost = 500;
    public int questionCost = 300;   // Question 아이템의 가격

    // TMP 오브젝트 추가
    public TextMeshProUGUI bulletCostText; // 총알 강화 가격을 표시할 TMP 텍스트
    public TextMeshProUGUI feedbackText;   // 구매 성공/실패 메시지를 표시할 TMP 텍스트

    // MyPlayer.cs (플레이어 스크립트)
    public void SetPlayer(MyPlayer localPlayer)
    {
        player = localPlayer;
        playerMovement = localPlayer.GetComponent<PlayerMovement>();

        UpdateBulletCostText();  // 처음 가격 설정 시 TMP 텍스트 업데이트
    }

    // 총알 강화
    public void BuyBulletUpgrade()
    {
        if (coinManager.SpendCoins(bulletUpgradeCost))
        {
            player.StrengthenBasicAttack(); // 총알 강화
            bulletUpgradeCost += 500;  // 다음 구매 시 500원 증가
            UpdateBulletCostText();    // TMP 텍스트 업데이트
            ShowFeedbackMessage("구매 성공!", 1f); // 구매 성공 메시지 표시
        }
        else
        {
            ShowFeedbackMessage("구매 실패!", 1f); // 구매 실패 메시지 표시
        }
    }

    // 체력 회복
    public void BuyHeal()
    {
        if (coinManager.SpendCoins(healCost))
        {
            player.Heal(40f);  // 체력 40% 회복
            ShowFeedbackMessage("구매 성공!", 1f);
        }
        else
        {
            ShowFeedbackMessage("구매 실패!", 1f);
        }
    }

    // 속도 증가
    public void BuySpeedBoost()
    {
        if (coinManager.SpendCoins(speedBoostCost))
        {
            // PlayerMovement에서 속도 증가 처리
            playerMovement.IncreaseSpeed(0.1f * playerMovement.moveSpeed, 30f); // 30초 동안 이동 속도 10% 증가
            ShowFeedbackMessage("구매 성공!", 1f);
        }
        else
        {
            ShowFeedbackMessage("구매 실패!", 1f);
        }
    }

    // 크리스탈 공격 구매
    public void BuyCrystalAttack()
    {
        if (coinManager.SpendCoins(crystalAttackCost))
        {
            crystalAttack.ActivateCrystalAttack(30f);  // 크리스탈 30초 동안 공격
            ShowFeedbackMessage("구매 성공!", 1f);
        }
        else
        {
            ShowFeedbackMessage("구매 실패!", 1f);
        }
    }

    // Question 아이템 구매
    public void BuyQuestion()
    {
        if (coinManager.SpendCoins(questionCost))
        {
            ApplyRandomEffect();  // 무작위 효과 적용
            ShowFeedbackMessage("구매 성공!", 1f);
        }
        else
        {
            ShowFeedbackMessage("구매 실패!", 1f);
        }
    }

    // 네 가지 효과 중 하나를 무작위로 적용
    private void ApplyRandomEffect()
    {
        int randomEffect = Random.Range(0, 4);  // 0~3 사이의 값

        switch (randomEffect)
        {
            case 0:
                player.StrengthenBasicAttack();
                Debug.Log("랜덤 효과: 총알 강화");
                break;
            case 1:
                player.Heal(40f);  // 40% 회복
                Debug.Log("랜덤 효과: 체력 회복");
                break;
            case 2:
                playerMovement.IncreaseSpeed(0.1f * playerMovement.moveSpeed, 30f); // 30초 동안 이동 속도 10% 증가
                Debug.Log("랜덤 효과: 속도 증가");
                break;
            case 3:
                crystalAttack.ActivateCrystalAttack(30f);  // 크리스탈 30초 동안 공격
                Debug.Log("랜덤 효과: 크리스탈 공격");
                break;
        }
    }

    // TMP 텍스트를 업데이트하는 메서드
    private void UpdateBulletCostText()
    {
        if (bulletCostText != null)
        {
            bulletCostText.text = bulletUpgradeCost.ToString(); // 예: "300 Coins"
        }
    }

    // 구매 성공/실패 메시지 표시
    private void ShowFeedbackMessage(string message, float duration)
    {
        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);  // 메시지 활성화
        Invoke("HideFeedbackMessage", duration);  // 일정 시간 후 메시지 비활성화
    }

    // 구매 성공/실패 메시지 숨김
    private void HideFeedbackMessage()
    {
        feedbackText.gameObject.SetActive(false);
    }
}
