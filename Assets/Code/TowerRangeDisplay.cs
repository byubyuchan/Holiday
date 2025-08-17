using UnityEngine;

public class TowerRangeDisplay : MonoBehaviour
{
    public Tower tower;
    public LineRenderer lineRenderer;

    [Header("Line Settings")]
    public int segments = 100;           // 원을 부드럽게 만들 점 개수
    public float lineWidth = 0.05f;      // 선 두께

    private void Awake()
    {
        // Tower와 LineRenderer 가져오기
        tower = GetComponent<Tower>();
        lineRenderer = GetComponent<LineRenderer>();

        // LineRenderer 기본 설정
        lineRenderer.useWorldSpace = false; // 타워 기준
        lineRenderer.loop = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    public void UpdateRange()
    {
        if (tower == null) return;
        lineRenderer.enabled = true;
        float range = tower.range;
        DrawCircle(range);
    }

    public void DrawCircle(float radius)
    {
        if (lineRenderer == null) return;

        int segments = 100;
        lineRenderer.positionCount = segments + 1;

        // 타워 스케일 보정
        float correctedRadius = radius / transform.localScale.x; // x, y, z가 동일하다고 가정

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * 360f / segments;
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * correctedRadius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * correctedRadius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    public void HideCircle()
    {
        lineRenderer.enabled = false;
    }
}