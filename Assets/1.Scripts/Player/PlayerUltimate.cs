using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class PlayerUltimate : MonoBehaviour
{
    public float ultimateCharge = 0f;  // 궁극기 게이지 (0% ~ 100%)
    public float chargePerHit = 0.2f;    // 상대에게 데미지를 입힐 때 충전량
    public float chargePerKill = 1f;   // 상대를 처치했을 때 충전량
    public float chargePerTime = 1f;   // 시간에 따른 충전량 (3초당 1%)
    private float timeSinceLastCharge = 0f; // 시간 충전을 위한 타이머

    public Image ultimateBar;  // 궁극기 게이지 UI (필어마운트)
    public TextMeshProUGUI ultimatePercentageText; // 궁극기 퍼센트 표시 텍스트 (TMP)

    private MyPlayer player;

    private bool canUseUltimate = false;  // 궁극기 사용 가능 여부

    private void Start()
    {
        player = GetComponent<MyPlayer>();
    }

    void Update()
    {
        if (player != null && player.isDead) return;

        // 3초마다 1% 충전
        timeSinceLastCharge += Time.deltaTime;
        if (timeSinceLastCharge >= 1.5f)
        {
            ChargeUltimate(chargePerTime);
            timeSinceLastCharge = 0f;
        }

        // 궁극기 사용 (Q키) - 100% 충전된 경우
        if (Input.GetKeyDown(KeyCode.Q) && canUseUltimate)
        {
            UseUltimate();
        }

        // 궁극기 게이지가 100% 이상이 될 수 없도록 처리
        ultimateCharge = Mathf.Clamp(ultimateCharge, 0, 100);

        // UI 업데이트 (궁극기 게이지)
        if (ultimateBar != null)
        {
            ultimateBar.fillAmount = ultimateCharge / 100f;
        }

        // 궁극기 퍼센트 텍스트 업데이트
        if (ultimatePercentageText != null)
        {
            ultimatePercentageText.text = Mathf.RoundToInt(ultimateCharge) + "%";  // 예: 58%
        }

        // 궁극기 사용 가능 여부 확인
        canUseUltimate = (ultimateCharge >= 100);
    }

    // 상대에게 데미지를 입히면 5% 충전
    public void OnHitEnemy()
    {
        ChargeUltimate(chargePerHit);
    }

    // 상대를 처치하면 3% 충전
    public void OnKillEnemy()
    {
        ChargeUltimate(chargePerKill);
    }

    // 궁극기 게이지를 충전하는 함수
    void ChargeUltimate(float amount)
    {
        ultimateCharge += amount;
        ultimateCharge = Mathf.Clamp(ultimateCharge, 0, 100); // 100% 이상 충전 방지
        Debug.Log("궁극기 게이지 충전: " + ultimateCharge + "%");
    }

    // 궁극기 사용
    void UseUltimate()
    {
        Debug.Log("궁극기 발동!");
        ultimateCharge = 0;  // 궁극기 사용 후 게이지 리셋
        if (ultimateBar != null)
        {
            ultimateBar.fillAmount = 0f;  // UI 리셋
        }

        // 궁극기 퍼센트 텍스트도 리셋
        if (ultimatePercentageText != null)
        {
            ultimatePercentageText.text = "0%";
        }
    }
}
