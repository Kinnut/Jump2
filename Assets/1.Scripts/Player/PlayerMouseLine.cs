using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerMouseLine : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;  // 두 점(플레이어와 마우스) 사이에 선을 그림
        lineRenderer.startWidth = 0.05f; // 선의 시작 두께
        lineRenderer.endWidth = 0.05f;   // 선의 끝 두께
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 기본 머티리얼
        lineRenderer.startColor = Color.blue;  // 선의 시작 색상
        lineRenderer.endColor = Color.blue;    // 선의 끝 색상
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        lineRenderer.SetPosition(0, transform.position);  // 플레이어 위치
        lineRenderer.SetPosition(1, mousePosition);       // 마우스 위치
    }
}
