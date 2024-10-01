using Cinemachine;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 mousePos;

    private CinemachineVirtualCamera virtualCam;  // �ó׸ӽ� ���� ī�޶� ����
    private bool isDashing = false;
    private float dashTime;
    private float nextDashTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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
    }
}
