using System;
using UnityEngine;
using Photon.Pun;

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

    public event Action OnEnemyDestroyed;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        playerUltimate = FindObjectOfType<PlayerUltimate>();
        coinManager = FindObjectOfType<CoinManager>();  // ���� �Ŵ����� ã��
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        lastHitByPlayer = false;  // �ʱ�ȭ
    }

    protected virtual void Update()
    {
        // �⺻���� ���� ������Ʈ ������ �ۼ��ϰų�, �ڽ� Ŭ�������� ������
    }

    [PunRPC]
    public void TakeDamage(float damage, bool isPlayer)
    {
        currentHealth -= damage;

        if (isPlayer)
        {
            lastHitByPlayer = true;  // �÷��̾ ���������� ���������� ���
            playerUltimate.OnHitEnemy();
        }

        if (currentHealth <= 0)
        {
            // ������ Ŭ���̾�Ʈ������ Die RPC�� ȣ��
            photonView.RPC("Die", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    protected virtual void Die()
    {
        if (lastHitByPlayer)
        {
            playerUltimate.OnKillEnemy();

            if (coinManager != null)
            {
                coinManager.AddCoins(coinReward);
            }
        }

        DropItem();
        OnEnemyDestroyed?.Invoke();  // �̺�Ʈ ȣ��
        gameObject.SetActive(false);
    }


    [PunRPC]
    protected void DropItem()
    {
        // ������ ��� ����ȭ
        float dropChance = UnityEngine.Random.Range(0f, 1f);
        if (dropChance <= 0.05f)
        {
            int randomIndex = UnityEngine.Random.Range(0, itemPrefabs.Length);
            PhotonNetwork.Instantiate(itemPrefabs[randomIndex].name, transform.position, Quaternion.identity);
        }
    }

    protected abstract void Move();
}
