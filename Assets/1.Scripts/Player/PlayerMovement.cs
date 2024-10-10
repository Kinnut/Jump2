using Cinemachine;
using UnityEngine;
using UnityEngine.UI;  // Image ������Ʈ ����� ���� �߰�
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;           // �⺻ �̵� �ӵ�
    public float dashSpeed = 15f;          // ��� �ӵ�
    public float dashDuration = 0.2f;      // ��� ���� �ð�
    public float dashCooldown = 2f;        // ��� ��Ÿ��
    private float defaultSpeed;            // �⺻ �̵� �ӵ��� �����ϴ� ����
    private bool isBoosted = false;        // �ӵ� ���� ����

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 mousePos;

    private CinemachineVirtualCamera virtualCam;  // �ó׸ӽ� ���� ī�޶� ����
    private bool isDashing = false;
    private float dashTime;
    private float nextDashTime = 0f;

    // ��� ��Ÿ���� ǥ���� Image UI ����
    public Image dashCooldownImage;  // Inspector���� ���� ���� �ʿ�

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultSpeed = moveSpeed;  // �⺻ �̵� �ӵ� ����

        // ���� �÷��̾��� ��쿡�� ī�޶� ����
        if (photonView.IsMine)
        {
            // ���� �ִ� CinemachineVirtualCamera�� ã��
            virtualCam = FindObjectOfType<CinemachineVirtualCamera>();

            if (virtualCam != null)
            {
                // ���� ī�޶��� Follow�� ���� �÷��̾�� ����
                virtualCam.Follow = transform;
            }
        }

        if (dashCooldownImage != null)
        {
            dashCooldownImage.fillAmount = 0f;  // ���� �� ��Ÿ�� ����
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;  // ���� �÷��̾ �ƴϸ� �Է� ����

        // �̵� �Է� ó��
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // ���콺 ��ġ�� ������Ʈ (���� �÷��̾)
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ��� �Է� ó�� (Shift Ű)
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime)
        {
            StartDash();
        }

        // ��� ��Ÿ�� UI ������Ʈ
        if (dashCooldownImage != null)
        {
            // ��� ��Ÿ�� ���� ��Ȳ�� �̹����� ǥ��
            if (nextDashTime > Time.time)
            {
                dashCooldownImage.fillAmount = (nextDashTime - Time.time) / dashCooldown;
            }
            else
            {
                dashCooldownImage.fillAmount = 0f;  // ��� ���� ������ �� 0
            }
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;  // ���� �÷��̾ �ƴϸ� ���� �Ұ�

        if (isDashing)
        {
            // ��� �� �̵�
            rb.MovePosition(rb.position + movement * dashSpeed * Time.fixedDeltaTime);
            dashTime -= Time.fixedDeltaTime;

            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }
        else
        {
            // �Ϲ� �̵� ó��
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        // ĳ���Ͱ� ���콺�� ���� ȸ�� (���� �÷��̾ ȸ��)
        if (photonView.IsMine)  // ���� �÷��̾ ȸ�� ó��
        {
            Vector2 lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }

    // ��� ó��
    void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;
        nextDashTime = Time.time + dashCooldown;

        // ��� ��Ÿ�� ������ ����
        if (dashCooldownImage != null)
        {
            dashCooldownImage.fillAmount = 1f;
        }
    }

    // �ӵ� ���� �޼���
    public void IncreaseSpeed(float amount, float duration)
    {
        if (!isBoosted)
        {
            moveSpeed += amount;
            isBoosted = true;
            Debug.Log("�ӵ� ����: " + amount + " for " + duration + " seconds");
            Invoke("ResetSpeed", duration);
        }
    }

    // �ӵ� ���� �޼���
    private void ResetSpeed()
    {
        moveSpeed = defaultSpeed;
        isBoosted = false;
        Debug.Log("�ӵ� ���󺹱�");
    }
}
