using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;          // 기본 이동 속도
    public float dashSpeed = 15f;         // 대시 속도
    public float dashDuration = 0.2f;     // 대시 지속 시간
    public float dashCooldown = 2f;       // 대시 쿨타임

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 mousePos;

    public Camera cam;
    private bool isDashing = false;       // 대시 중인지 여부
    private float dashTime;               // 대시 남은 시간
    private float nextDashTime = 0f;      // 다음 대시 가능 시간

    public Image dashCooldownImage;       // 대시 쿨타임 UI 이미지

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dashCooldownImage.gameObject.SetActive(false);
    }

    void Update()
    {
        // 이동 입력
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // 대시 입력 처리 (Shift 키)
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime)
        {
            dashCooldownImage.gameObject.SetActive(true);
            StartDash();
        }

        // 쿨타임 UI 업데이트
        if (dashCooldownImage != null)
        {
            if (Time.time >= nextDashTime)
            {
                dashCooldownImage.fillAmount = 1f;  // 쿨타임 완료 시 UI 가득참
                dashCooldownImage.gameObject.SetActive(false);
            }
            else
            {
                dashCooldownImage.fillAmount = (nextDashTime - Time.time) / dashCooldown;  // 쿨타임 진행중
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            // 대시 중 이동
            rb.MovePosition(rb.position + movement * dashSpeed * Time.fixedDeltaTime);

            // 대시 시간 종료
            dashTime -= Time.fixedDeltaTime;
            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }
        else
        {
            // 일반 이동
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        // 캐릭터가 마우스를 향해 회전
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    // 대시 시작
    void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;
        nextDashTime = Time.time + dashCooldown;  // 쿨타임 설정
    }
}
