using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 3f;
    private GameObject target; // 목표 몬스터
    public GameObject hitEffectPrefab;

    public Tower tower;

    // 목표와 데미지를 설정하는 함수

    // 초기화 메서드: 타워와 목표 설정
    public void Init(Tower _tower, GameObject _target)
    {
        tower = _tower;
        target = _target;
        StartCoroutine(MoveToTarget());
    }

    // 목표를 향해 이동하는 코루틴
    IEnumerator MoveToTarget()
    {
        while (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

            // 목표에 도달했는지 확인
            if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
            {
                HitTarget();
                yield break; // 코루틴 종료
            }

            yield return null;
        }

        // 목표가 사라졌다면 투사체 제거
        Destroy(gameObject);
    }

// 충돌 처리 (OnCollisionEnter2D)
private void OnCollisionEnter2D(Collision2D collision)
    {
        // "Enemy" 태그를 가진 적과 충돌했는지 확인
        if (collision.collider.CompareTag("Enemy") && collision.gameObject == target)
        {
            HitTarget();
        }
    }

    // 목표에 도달했을 때 처리
    private void HitTarget()
    {
        if (target != null)
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(tower.damage);
            }
        }

        // 충돌 이펙트 생성
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
