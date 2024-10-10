using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro�� ����ϱ� ���� �߰�
using Photon.Pun;

public class PlayerUltimate : MonoBehaviourPun
{
    public float ultimateCharge = 0f;  // �ñر� ������ (0% ~ 100%)
    public float chargePerHit = 0.2f;    // ��뿡�� �������� ���� �� ������
    public float chargePerKill = 1f;   // ��븦 óġ���� �� ������
    public float chargePerTime = 1f;   // �ð��� ���� ������ (3�ʴ� 1%)
    private float timeSinceLastCharge = 0f; // �ð� ������ ���� Ÿ�̸�

    public Image ultimateBar;  // �ñر� ������ UI (�ʾ��Ʈ)
    public TextMeshProUGUI ultimatePercentageText; // �ñر� �ۼ�Ʈ ǥ�� �ؽ�Ʈ (TMP)

    private MyPlayer player;

    private bool canUseUltimate = false;  // �ñر� ��� ���� ����
    public int selectedUltimate = 0;

    private void Start()
    {
        player = GetComponent<MyPlayer>();
    }

    void Update()
    {
        if (player != null && player.isDead) return;

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

        // �ñر� �ۼ�Ʈ �ؽ�Ʈ ������Ʈ
        if (ultimatePercentageText != null)
        {
            ultimatePercentageText.text = Mathf.RoundToInt(ultimateCharge) + "%";  // ��: 58%
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

        // ���õ� �ñر⿡ ���� �ٸ� ȿ�� �ߵ�
        if (selectedUltimate == 0) // ���� �ñر�
        {
            photonView.RPC("FreezeEnemies", RpcTarget.All);
        }
        else if (selectedUltimate == 1) // �� �ñر�
        {
            photonView.RPC("BoostAllies", RpcTarget.All);
        }
        else if (selectedUltimate == 2) // ȸ�� �ñر�
        {
            photonView.RPC("HealAllAllies", RpcTarget.All);
        }

        ultimateCharge = 0;  // �ñر� ��� �� ������ ����
        ultimateBar.fillAmount = 0f;
        ultimatePercentageText.text = "0%";
    }

    // ��� ���� 10�ʰ� ���ߴ� �Լ�
    [PunRPC]
    void FreezeEnemies()
    {
        var enemies = FindObjectsOfType<CrystalChaser>();
        foreach (var enemy in enemies)
        {
            enemy.FreezeForDuration(10f);
        }
    }

    // ��� �Ʊ��� �ӵ��� ���ݼӵ��� 20�ʰ� ������Ű�� �Լ�
    [PunRPC]
    void BoostAllies()
    {
        var players = FindObjectsOfType<PlayerMovement>();
        foreach (var player in players)
        {
            player.IncreaseSpeed(2f, 20f);
        }
    }

    // ��� �Ʊ��� ü���� ȸ����Ű�� �Լ�
    [PunRPC]
    void HealAllAllies()
    {
        var players = FindObjectsOfType<MyPlayer>();
        foreach (var player in players)
        {
            player.Heal(player.maxHealth);
        }
    }
}
