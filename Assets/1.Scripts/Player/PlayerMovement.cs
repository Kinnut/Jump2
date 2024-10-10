using Cinemachine;
using UnityEngine;
using UnityEngine.UI;  // Image 컴포넌트 사용을 위해 추가
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;           // 기본 이동 속도
    public float dashSpeed = 15f;          // 대시 속도
    public float dashDuration = 0.2f;      // 대시 지속 시간
    public float dashCooldown = 2f;        // 대시 쿨타임
    private float defaultSpeed;            // 기본 이동 속도를 저장하는 변수
    private bool isBoosted = false;        // 속도 증가 여부

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 mousePos;

    private CinemachineVirtualCamera virtualCam;  // 시네머신 가상 카메라 참조
    private bool isDashing = false;
    private float dashTime;
    private float nextDashTime = 0f;

    // 대시 쿨타임을 표시할 Image UI 참조
    public Image dashCooldownImage;  // Inspector에서 참조 설정 필요

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultSpeed = moveSpeed;  // 기본 이동 속도 저장

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

        if (dashCooldownImage != null)
        {
            dashCooldownImage.fillAmount = 0f;  // 시작 시 쿨타임 없음
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

        // 대시 쿨타임 UI 업데이트
        if (dashCooldownImage != null)
        {
            // 대시 쿨타임 진행 상황을 이미지에 표시
            if (nextDashTime > Time.time)
            {
                dashCooldownImage.fillAmount = (nextDashTime - Time.time) / dashCooldown;
            }
            else
            {
                dashCooldownImage.fillAmount = 0f;  // 대시 가능 상태일 때 0
            }
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

        // 대시 쿨타임 게이지 시작
        if (dashCooldownImage != null)
        {
            dashCooldownImage.fillAmount = 1f;
        }
    }

    // 속도 증가 메서드
    public void IncreaseSpeed(float amount, float duration)
    {
        if (!isBoosted)
        {
            moveSpeed += amount;
            isBoosted = true;
            Debug.Log("속도 증가: " + amount + " for " + duration + " seconds");
            Invoke("ResetSpeed", duration);
        }
    }

    // 속도 복원 메서드
    private void ResetSpeed()
    {
        moveSpeed = defaultSpeed;
        isBoosted = false;
        Debug.Log("속도 원상복구");
    }
}
