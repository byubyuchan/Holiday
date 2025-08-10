using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private HashSet<GameObject> Enemies = new HashSet<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GameManager.instance.Life --;
            // GameObject enemy = collision.gameObject;

            // 이미 처리된 적은 무시
            //if (Enemies.Contains(enemy)) return;

            //Enemies.Add(enemy);
            //enemy.GetComponent<Enemy>().Dead();

            // 적이 비활성화되면 HashSet에서 제거
            //StartCoroutine(RemoveEnemyWhenDeactivated(enemy));
        }

        if(GameManager.instance.Life == 0)
        {
            CutsceneManager.instance.PlayDeathCutscene(this.transform, "패배하였습니다...",0.5f);
            GameManager.instance.ShowRetryButton(4f, false);
            AudioManager.instance.PlaySFX("P_Hit4");
            GameManager.instance.isLive = false;
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
