using Cinemachine;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyPlayer : MonoBehaviourPun
{
    // 기존 변수들
    public float health = 100f;
    public Image healthBarImage;
    private float maxHealth;

    private CinemachineImpulseSource impulseSource;
    public float speed = 5f;
    private float defaultSpeed;
    private bool isBoosted = false;

    private PlayerShooting playerShooting;
    private PlayerMovement playerMovement;  // PlayerMovement 스크립트 참조

    private float playTime = 0f;

    public GameObject deathUI;
    public TextMeshProUGUI countdownText;
    private float respawnTime = 10f;
    public bool isDead = false;

    private Rigidbody2D rb;
    private Collider2D playerCollider;

    private Canvas playerCanvas;  // 플레이어에 종속된 캔버스
    private Camera mainCamera;    // 메인 카메라 참조

    void Start()
    {
        ShopManager shopManager = FindObjectOfType<ShopManager>();
        if (shopManager != null && photonView.IsMine)
        {
            shopManager.SetPlayer(this);  // 참조를 전달
        }

        // 플레이어의 자식 오브젝트로 포함된 Canvas를 찾음
        playerCanvas = GetComponentInChildren<Canvas>();

        // 이 스크립트가 로컬 플레이어의 것인지 확인하고, 로컬 플레이어의 캔버스만 활성화
        if (photonView.IsMine)
        {
            // 로컬 플레이어의 메인 카메라 찾기
            mainCamera = Camera.main;

            // 자신의 캔버스만 활성화
            playerCanvas.enabled = true;

            // 캔버스 회전을 고정하려면, 메인 카메라에 캔버스를 고정하는 것도 가능
            playerCanvas.transform.SetParent(mainCamera.transform, false);
            playerCanvas.transform.localPosition = new Vector3(0, 0, 2);  // 적절한 위치 설정
        }
        else
        {
            // 다른 플레이어들의 캔버스 비활성화
            playerCanvas.enabled = false;
        }

        maxHealth = health;
        defaultSpeed = speed;
        UpdateHealthBar();

        impulseSource = GetComponent<CinemachineImpulseSource>();
        playerShooting = GetComponent<PlayerShooting>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        // 사망 UI 비활성화
        deathUI.SetActive(false);
    }

    [PunRPC]
    public void SyncPlayerColor(int actorNumber, float r, float g, float b)
    {
        // 모든 플레이어의 PhotonView를 찾고, 해당 플레이어의 색상을 변경
        foreach (var player in FindObjectsOfType<PhotonView>())
        {
            // PhotonView 소유주가 있는지 확인하고, 그 소유주가 플레이어인지 확인
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
            // UI 캔버스의 회전을 고정시키기 (캔버스가 플레이어와 함께 회전하지 않게 함)
            if (playerCanvas != null)
            {
                playerCanvas.transform.rotation = Quaternion.identity;  // 회전을 고정
            }
        }
    }

    void Update()
    {
        playTime += Time.deltaTime;

        // 사망 상태 확인 및 부활 처리
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

    // 데미지를 받는 메서드
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

    // 체력 회복 메서드
    public void Heal(float amount)
    {
        if (isDead) return;

        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthBar();
        Debug.Log("플레이어 체력 회복: " + health);
    }

    // 기본 공격 강화 메서드
    public void StrengthenBasicAttack()
    {
        playerShooting.ChangeBasicBulletPrefab();
        Debug.Log("기본 공격이 강화되었습니다.");
    }

    // 속도 증가 메서드
    public void IncreaseSpeed(float amount, float duration)
    {
        if (!isBoosted)
        {
            speed += amount;
            isBoosted = true;
            Debug.Log("속도 증가: " + amount + " for " + duration + " seconds");
            Invoke("ResetSpeed", duration);
        }
    }

    // 속도 복원 메서드
    private void ResetSpeed()
    {
        speed = defaultSpeed;
        isBoosted = false;
        Debug.Log("속도 원상복구");
    }

    // 체력 바 업데이트 메서드
    void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = health / maxHealth;
        }
    }

    // 플레이어 사망 처리
    void Die()
    {
        Debug.Log("플레이어 사망");
        isDead = true;

        deathUI.SetActive(true);
        respawnTime = 10f;

        // 플레이어 이동 및 공격 비활성화
        DisablePlayer();
    }

    // 플레이어 부활 처리
    void Respawn()
    {
        Debug.Log("플레이어 부활");
        isDead = false;
        health = maxHealth;
        UpdateHealthBar();

        EnablePlayer();

        deathUI.SetActive(false);
    }

    // 플레이어 비활성화 메서드
    void DisablePlayer()
    {
        playerShooting.enabled = false;
        playerMovement.enabled = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        playerCollider.enabled = false;
    }

    // 플레이어 활성화 메서드
    void EnablePlayer()
    {
        playerShooting.enabled = true;
        playerMovement.enabled = true;
        rb.isKinematic = false;
        playerCollider.enabled = true;
    }
}
