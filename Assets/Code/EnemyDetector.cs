using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    private TowerAttack towerAttack;  // 타워 공격 클래스
    private Tower tower;
    private GameObject currentTarget;

    private void Awake()
    {
        towerAttack = GetComponent<TowerAttack>();
        tower = GetComponent<Tower>();
    }

    private void Update()
    {
        // 범위 내 가장 가까운 적을 찾음
        currentTarget = FindClosestEnemy();

        if (currentTarget != null)
        {
            // 현재 목표가 범위 내에 있는지 확인
            if (IsWithinRange(currentTarget))
            {
                towerAttack.Attack(currentTarget); // 지속적으로 공격 수행
            }
            else
            {
                currentTarget = null; // 목표가 범위를 벗어나면 초기화
            }
        }
    }

    private GameObject FindClosestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, tower.range);
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector3.Distance(transform.position, collider.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = collider.gameObject;
                }
            }
        }

        return closestEnemy;
    }

    private bool IsWithinRange(GameObject enemy)
    {
        float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
        return distanceToEnemy <= tower.range; // 타워의 Range 값과 비교
    }
}
