using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public static EnemyDetector instance;
    public List<GameObject> enemiesInRange = new List<GameObject>();
    private Coroutine attackCoroutine;

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
            Debug.Log("적이 사정거리 안에 들어왔습니다!");

            // 공격 코루틴 시작 (이미 실행 중이면 중복 실행 방지)
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackRoutine());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);

            // 적이 모두 나가면 코루틴 중지
            if (enemiesInRange.Count == 0 && attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Debug.Log("적과 충돌! 데미지 입음");

        }
    }

    private IEnumerator AttackRoutine()
    {
        while (enemiesInRange.Count > 0)
        {
            TowerAttack.instance.Attack();
            yield return new WaitForSeconds(1f); // TODO : 공격 속도 조절
        }

        attackCoroutine = null; // 모든 적이 사라지면 코루틴 종료
    }
}
