using Cinemachine;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyPlayer : MonoBehaviourPun
{
    public float health = 100f;
    public Image healthBarImage;
    public float maxHealth;

    private CinemachineImpulseSource impulseSource;

    private PlayerShooting playerShooting;
    private PlayerMovement playerMovement;  // PlayerMovement ��ũ��Ʈ ����

    private float playTime = 0f;

    public GameObject deathUI;
    public TextMeshProUGUI countdownText;
    private float respawnTime = 10f;
    public bool isDead = false;

    private Rigidbody2D rb;
    private Collider2D playerCollider;

    private Canvas playerCanvas;  // �÷��̾ ���ӵ� ĵ����
    private Camera mainCamera;    // ���� ī�޶� ����

    void Start()
    {
        ShopManager shopManager = FindObjectOfType<ShopManager>();
        if (shopManager != null && photonView.IsMine)
        {
            shopManager.SetPlayer(this);  // ������ ����
        }

        // �÷��̾��� �ڽ� ������Ʈ�� ���Ե� Canvas�� ã��
        playerCanvas = GetComponentInChildren<Canvas>();

        // �� ��ũ��Ʈ�� ���� �÷��̾��� ������ Ȯ���ϰ�, ���� �÷��̾��� ĵ������ Ȱ��ȭ
        if (photonView.IsMine)
        {
            // ���� �÷��̾��� ���� ī�޶� ã��
            mainCamera = Camera.main;

            // �ڽ��� ĵ������ Ȱ��ȭ
            playerCanvas.enabled = true;

            // ĵ���� ȸ���� �����Ϸ���, ���� ī�޶� ĵ������ �����ϴ� �͵� ����
            playerCanvas.transform.SetParent(mainCamera.transform, false);
            playerCanvas.transform.localPosition = new Vector3(0, 0, 2);  // ������ ��ġ ����
        }
        else
        {
            // �ٸ� �÷��̾���� ĵ���� ��Ȱ��ȭ
            playerCanvas.enabled = false;
        }

        maxHealth = health;
        UpdateHealthBar();

        impulseSource = GetComponent<CinemachineImpulseSource>();
        playerShooting = GetComponent<PlayerShooting>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        // ��� UI ��Ȱ��ȭ
        deathUI.SetActive(false);
    }

    [PunRPC]
    public void SyncPlayerColor(int actorNumber, float r, float g, float b)
    {
        foreach (var player in FindObjectsOfType<PhotonView>())
        {
            if (player.Owner != null && player.Owner.ActorNumber == actorNumber && player.CompareTag("Player"))
            {
                SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

                if (playerSpriteRenderer != null)
                {
                    playerSpriteRenderer.color = new Color(r, g, b);
                }
                else
                {
                    Debug.LogWarning("Player SpriteRenderer not found.");
                }
            }
        }
    }

    void LateUpdate()
    {
        if (photonView.IsMine)
        {
            // UI ĵ������ ȸ���� ������Ű�� (ĵ������ �÷��̾�� �Բ� ȸ������ �ʰ� ��)
            if (playerCanvas != null)
            {
                playerCanvas.transform.rotation = Quaternion.identity;  // ȸ���� ����
            }
        }
    }

    void Update()
    {
        playTime += Time.deltaTime;

        // ��� ���� Ȯ�� �� ��Ȱ ó��
        if (isDead && respawnTime > 0)
        {
            respawnTime -= Time.deltaTime;
            countdownText.text = "RESPAWN : " + Mathf.Ceil(respawnTime).ToString();

            if (respawnTime <= 0)
            {
                Respawn();
            }
        }
    }

    // �������� �޴� �޼���
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        if (health < 0)
        {
            health = 0;
        }

        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        UpdateHealthBar();

        if (health <= 0)
        {
            Die();
        }
    }

    // ü�� ȸ�� �޼���
    public void Heal(float amount)
    {
        if (isDead) return;

        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthBar();
        Debug.Log("�÷��̾� ü�� ȸ��: " + health);
    }

    // �⺻ ���� ��ȭ �޼���
    public void StrengthenBasicAttack()
    {
        playerShooting.ChangeBasicBulletPrefab();
        Debug.Log("�⺻ ������ ��ȭ�Ǿ����ϴ�.");
    }

    // ü�� �� ������Ʈ �޼���
    void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = health / maxHealth;
        }
    }

    // �÷��̾� ��� ó��
    void Die()
    {
        Debug.Log("�÷��̾� ���");
        isDead = true;

        deathUI.SetActive(true);
        respawnTime = 10f;

        DisablePlayer();
    }

    // �÷��̾� ��Ȱ ó��
    void Respawn()
    {
        Debug.Log("�÷��̾� ��Ȱ");
        isDead = false;
        health = maxHealth;
        UpdateHealthBar();

        EnablePlayer();

        deathUI.SetActive(false);
    }

    // �÷��̾� ��Ȱ��ȭ �޼���
    void DisablePlayer()
    {
        playerShooting.enabled = false;
        playerMovement.enabled = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        playerCollider.enabled = false;
    }

    // �÷��̾� Ȱ��ȭ �޼���
    void EnablePlayer()
    {
        playerShooting.enabled = true;
        playerMovement.enabled = true;
        rb.isKinematic = false;
        playerCollider.enabled = true;
    }
}
