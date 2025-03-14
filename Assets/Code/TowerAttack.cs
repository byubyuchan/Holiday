using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    public static TowerAttack instance;
    private Tower tower;
    private float attackCooldown;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        tower = GetComponent<Tower>();
        attackCooldown = 1f / tower.towerData.Speed;
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);
            Attack();
        }
    }

    public void Attack()
    {
        if (EnemyDetector.instance.enemiesInRange.Count > 0)
        {
            for (int i=0; i<EnemyDetector.instance.enemiesInRange.Count; i++)
            {
                GameObject target = EnemyDetector.instance.enemiesInRange[i]; // 첫 번째 적 공격
                Debug.Log($"타워 {tower.towerData.towerType}가 {target.name}을 공격! 피해량: {tower.towerData.Damage}");

                // 실제 데미지를 주는 코드 (적 스크립트 필요)
                Enemy enemy = target.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.hp -= Tower.instance.damage;
                }
            }

        }
    }
}
