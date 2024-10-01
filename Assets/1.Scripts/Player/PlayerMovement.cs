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

    private CinemachineVirtualCamera virtualCam;  // 시네머신 가상 카메라 참조
    private bool isDashing = false;
    private float dashTime;
    private float nextDashTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 로컬 플레이어의 경우에만 카메라를 설정
        if (photonView.IsMine)
        {
            // 씬에 있는 CinemachineVirtualCamera를 찾음
            virtualCam = FindObjectOfType<CinemachineVirtualCamera>();

            if (virtualCam != null)
            {
                // 가상 카메라의 Follow를 현재 플레이어로 설정
                virtualCam.Follow = transform;
            }
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;  // 로컬 플레이어가 아니면 입력 무시

        // 이동 입력 처리
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 마우스 위치를 업데이트 (로컬 플레이어만)
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 대시 입력 처리 (Shift 키)
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime)
        {
            StartDash();
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;  // 로컬 플레이어가 아니면 조작 불가

        if (isDashing)
        {
            // 대시 중 이동
            rb.MovePosition(rb.position + movement * dashSpeed * Time.fixedDeltaTime);
            dashTime -= Time.fixedDeltaTime;

            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }
        else
        {
            // 일반 이동 처리
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        // 캐릭터가 마우스를 향해 회전 (로컬 플레이어만 회전)
        if (photonView.IsMine)  // 로컬 플레이어만 회전 처리
        {
            Vector2 lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }

    // 대시 처리
    void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;
        nextDashTime = Time.time + dashCooldown;
    }
}
