using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerMouseLine : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;  // �� ��(�÷��̾�� ���콺) ���̿� ���� �׸�
        lineRenderer.startWidth = 0.05f; // ���� ���� �β�
        lineRenderer.endWidth = 0.05f;   // ���� �� �β�
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // �⺻ ��Ƽ����
        lineRenderer.startColor = Color.blue;  // ���� ���� ����
        lineRenderer.endColor = Color.blue;    // ���� �� ����
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        lineRenderer.SetPosition(0, transform.position);  // �÷��̾� ��ġ
        lineRenderer.SetPosition(1, mousePosition);       // ���콺 ��ġ
    }
}
