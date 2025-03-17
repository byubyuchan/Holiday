using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    public static TowerAttack instance;
    public EnemyDetector EnemyDetector;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
    }

    public void Attack()
    {
        if (EnemyDetector.enemiesInRange.Count > 0)
        {
            switch (Tower.instance.towerType)
            {
                case ("Melee"):
                    for (int i = 0; i < EnemyDetector.enemiesInRange.Count; i++)
                    {
                        GameObject target = EnemyDetector.enemiesInRange[i]; // 첫 번째 적 공격

                        // 실제 데미지를 주는 코드 (적 스크립트 필요)
                        Enemy enemy = target.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.hp -= Tower.instance.damage;
                        }
                    }
                    break;

                case ("Range"):
                    // TODO : RangeAttack
                    break;
            }
        }
        else return;
    }
}
