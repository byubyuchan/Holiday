using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public List<GameObject> enemiesInRange = new List<GameObject>();
    public bool isAttacking = false;

    private Animator towerAnim;  // 타워의 애니메이터
    private TowerAttack towerAttack;  // 타워 공격 클래스

    private void Awake()
    {
        // 타워의 애니메이터와 공격 클래스 초기화
        towerAnim = GetComponent<Animator>();
        towerAttack = GetComponent<TowerAttack>();
    }

    private void Update()
    {
        if (enemiesInRange.Count > 0 && !isAttacking)
        {
            Attack();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
            Debug.Log("적이 사정거리 안에 들어왔습니다!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Debug.Log("적과 충돌! 데미지 입음");
        }
    }

    private void Attack()
    {
        isAttacking = true;  // 공격 시작
        towerAnim.SetTrigger("Attack");
        towerAttack.Attack();
        Invoke("ResetAttack", 1f);  // 공격 후 isAttacking을 false로 설정
    }

    private void ResetAttack()
    {
        isAttacking = false;  // 공격 종료
    }
}
