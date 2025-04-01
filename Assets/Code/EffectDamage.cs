using System.Collections.Generic;
using UnityEngine;

public class EffectDamage : MonoBehaviour
{
    public float damage = 10f; // 이펙트가 줄 데미지
    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>(); // 이미 데미지를 준 적을 기록

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // Enemy 태그를 가진 적과 충돌했을 때
        {
            if (!damagedEnemies.Contains(collision.gameObject)) // 이미 데미지를 준 적은 제외
            {
                damagedEnemies.Add(collision.gameObject); // 충돌한 적을 기록

                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage); // 적에게 데미지 적용
                }
            }
        }
    }
    private void OnDisable()
    {
        ResetDamageList();
    }

    public void ResetDamageList()
    {
        damagedEnemies.Clear();
    }
}
