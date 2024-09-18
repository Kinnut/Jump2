using UnityEngine;
using UnityEngine.UI;

public class PlayerUltimate : MonoBehaviour
{
    public float ultimateCharge = 0f;  // �ñر� ������ (0% ~ 100%)
    public float chargePerHit = 0.2f;    // ��뿡�� �������� ���� �� ������
    public float chargePerKill = 1f;   // ��븦 óġ���� �� ������
    public float chargePerTime = 1f;   // �ð��� ���� ������ (3�ʴ� 1%)
    private float timeSinceLastCharge = 0f; // �ð� ������ ���� Ÿ�̸�

    public Image ultimateBar;  // �ñر� ������ UI (�ʾ��Ʈ)

    private bool canUseUltimate = false;  // �ñر� ��� ���� ����

    void Update()
    {
        // 3�ʸ��� 1% ����
        timeSinceLastCharge += Time.deltaTime;
        if (timeSinceLastCharge >= 1.5f)
        {
            ChargeUltimate(chargePerTime);
            timeSinceLastCharge = 0f;
        }

        // �ñر� ��� (QŰ) - 100% ������ ���
        if (Input.GetKeyDown(KeyCode.Q) && canUseUltimate)
        {
            UseUltimate();
        }

        // �ñر� �������� 100% �̻��� �� �� ������ ó��
        ultimateCharge = Mathf.Clamp(ultimateCharge, 0, 100);

        // UI ������Ʈ (�ñر� ������)
        if (ultimateBar != null)
        {
            ultimateBar.fillAmount = ultimateCharge / 100f;
        }

        // �ñر� ��� ���� ���� Ȯ��
        canUseUltimate = (ultimateCharge >= 100);
    }

    // ��뿡�� �������� ������ 5% ����
    public void OnHitEnemy()
    {
        ChargeUltimate(chargePerHit);
    }

    // ��븦 óġ�ϸ� 3% ����
    public void OnKillEnemy()
    {
        ChargeUltimate(chargePerKill);
    }

    // �ñر� �������� �����ϴ� �Լ�
    void ChargeUltimate(float amount)
    {
        ultimateCharge += amount;
        ultimateCharge = Mathf.Clamp(ultimateCharge, 0, 100); // 100% �̻� ���� ����
        Debug.Log("�ñر� ������ ����: " + ultimateCharge + "%");
    }

    // �ñر� ���
    void UseUltimate()
    {
        Debug.Log("�ñر� �ߵ�!");
        ultimateCharge = 0;  // �ñر� ��� �� ������ ����
        if (ultimateBar != null)
        {
            ultimateBar.fillAmount = 0f;  // UI ����
        }
    }
}
