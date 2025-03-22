using UnityEngine;

public class TowerRangeDebugger : MonoBehaviour
{
    public float attackRange = 5f; // 공격 사거리
    public Color gizmoColor = Color.blue; // Gizmo 색상

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, attackRange); // 공격 범위 표시
    }
}
