using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Goal : MonoBehaviour
{
    private HashSet<GameObject> Enemies = new HashSet<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GameObject enemy = collision.gameObject;

            // 이미 처리된 적은 무시
            if (Enemies.Contains(enemy)) return;

            // 처음 들어온 적만 처리
            Enemies.Add(enemy);
            GameManager.instance.Life--;
            GameManager.instance.Gold++;
            enemy.GetComponent<Enemy>().Dead();

            // 적이 비활성화되면 HashSet에서 제거
            StartCoroutine(RemoveEnemyWhenDeactivated(enemy));
        }
    }

    private IEnumerator RemoveEnemyWhenDeactivated(GameObject enemy)
    {
        // 적이 비활성화될 때까지 대기
        while (enemy.activeInHierarchy)
        {
            yield return null;
        }

        // HashSet에서 제거
        Enemies.Remove(enemy);
    }
}
