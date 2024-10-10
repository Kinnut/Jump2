using System;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public abstract class EnemyBase : MonoBehaviourPun
{
    public float maxHealth = 100f;
    protected float currentHealth;
    public float moveSpeed = 2f;
    public GameObject[] itemPrefabs;
    private PlayerUltimate playerUltimate;
    private CoinManager coinManager;  // ���� �Ŵ��� ����

    public int coinReward = 10;  // ���� óġ �� �����ϴ� ���� ��
    private bool lastHitByPlayer = false;  // ���������� ������ ����� �÷��̾����� ����
    private bool isDead = false;  // ���� ��� ���� üũ

    public event Action OnEnemyDestroyed;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        playerUltimate = FindObjectOfType<PlayerUltimate>();
        coinManager = FindObjectOfType<CoinManager>();
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        lastHitByPlayer = false;
        isDead = false;  // ���� ��� �ִ� ���·� �ʱ�ȭ
    }

    // Ŭ���̾�Ʈ���� ���� ü���� ���̴� TakeDamage
    [PunRPC]
    public void TakeDamage(float damage, bool isPlayer)
    {
        if (isDead) return; // �̹� ���� ���¶�� �� �̻� ���ظ� ���� ����

        currentHealth -= damage;  // ��� Ŭ���̾�Ʈ���� ü�� ����

        if (isPlayer)
        {
            lastHitByPlayer = true;  // �÷��̾ ���������� ���������� ���
            playerUltimate.OnHitEnemy();
        }

        if (currentHealth <= 0)
        {
            Die();  // ��� Ŭ���̾�Ʈ���� ���� �׵��� ó��
        }
    }

    protected virtual void Update()
    {
        // �⺻���� ���� ������Ʈ ������ �ۼ��ϰų�, �ڽ� Ŭ�������� ������
    }

    // ��� Ŭ���̾�Ʈ���� ����Ǵ� Die �޼���
    protected virtual void Die()
    {
        if (isDead) return;  // �̹� ���� �����̸� ����
        isDead = true;  // ���� ���·� ����

        if (lastHitByPlayer)
        {
            playerUltimate.OnKillEnemy();

            // ���� �й�� �� Ŭ���̾�Ʈ���� ó��
            photonView.RPC("DistributeCoins", RpcTarget.All);
        }

        DropItem();
        OnEnemyDestroyed?.Invoke();  // �̺�Ʈ ȣ��

        // ��� Ŭ���̾�Ʈ���� �� ��Ȱ��ȭ
        photonView.RPC("DeactivateEnemy", RpcTarget.All);
    }

    // ��� �÷��̾ ������ ��� �ϴ� RPC �޼���
    [PunRPC]
    public void DistributeCoins()
    {
        if (coinManager != null)
        {
            coinManager.AddCoins(coinReward);
        }
        else
        {
            Debug.LogError("CoinManager not found!");
        }
    }

    // ������ ��� ����ȭ
    [PunRPC]
    protected void DropItem()
    {
        float dropChance = UnityEngine.Random.Range(0f, 1f);
        if (dropChance <= 0.05f)
        {
            int randomIndex = UnityEngine.Random.Range(0, itemPrefabs.Length);
            PhotonNetwork.Instantiate(itemPrefabs[randomIndex].name, transform.position, Quaternion.identity);
        }
    }

    // ��� Ŭ���̾�Ʈ���� ���� ��Ȱ��ȭ�ϴ� RPC
    [PunRPC]
    public void DeactivateEnemy()
    {
        gameObject.SetActive(false);
    }

    protected abstract void Move();
}
