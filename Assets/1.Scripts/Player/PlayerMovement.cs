using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;          // �⺻ �̵� �ӵ�
    public float dashSpeed = 15f;         // ��� �ӵ�
    public float dashDuration = 0.2f;     // ��� ���� �ð�
    public float dashCooldown = 2f;       // ��� ��Ÿ��

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 mousePos;

    public Camera cam;
    private bool isDashing = false;       // ��� ������ ����
    private float dashTime;               // ��� ���� �ð�
    private float nextDashTime = 0f;      // ���� ��� ���� �ð�

    public Image dashCooldownImage;       // ��� ��Ÿ�� UI �̹���

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dashCooldownImage.gameObject.SetActive(false);
    }

    void Update()
    {
        // �̵� �Է�
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // ��� �Է� ó�� (Shift Ű)
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= nextDashTime)
        {
            dashCooldownImage.gameObject.SetActive(true);
            StartDash();
        }

        // ��Ÿ�� UI ������Ʈ
        if (dashCooldownImage != null)
        {
            if (Time.time >= nextDashTime)
            {
                dashCooldownImage.fillAmount = 1f;  // ��Ÿ�� �Ϸ� �� UI ������
                dashCooldownImage.gameObject.SetActive(false);
            }
            else
            {
                dashCooldownImage.fillAmount = (nextDashTime - Time.time) / dashCooldown;  // ��Ÿ�� ������
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            // ��� �� �̵�
            rb.MovePosition(rb.position + movement * dashSpeed * Time.fixedDeltaTime);

            // ��� �ð� ����
            dashTime -= Time.fixedDeltaTime;
            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }
        else
        {
            // �Ϲ� �̵�
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        // ĳ���Ͱ� ���콺�� ���� ȸ��
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    // ��� ����
    void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;
        nextDashTime = Time.time + dashCooldown;  // ��Ÿ�� ����
    }
}
