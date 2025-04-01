using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 3f;
    private GameObject target; // 목표 몬스터
    public int effectIndex;

    public Tower tower;

    private bool isActive; // 투사체 활성 상태 확인

    // 초기화 메서드: 타워와 목표 설정
    public void Init(Tower _tower, GameObject _target)
    {
        tower = _tower;
        target = _target;
        isActive = true; // 활성화 상태로 설정
        StartCoroutine(MoveToTarget());
    }

    // 목표를 향해 이동하는 코루틴
    IEnumerator MoveToTarget()
    {
        while (target != null && isActive)
        {
            if (target == null || !target.activeSelf) // 목표가 비활성화된 경우
            {
                DeactivateProjectile(); // 투사체 비활성화
                yield break;
            }
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            yield return null;
        }

        // 목표가 사라졌거나 비활성화되었다면 투사체 비활성화
        DeactivateProjectile();
    }

    // 충돌 처리 (OnCollisionEnter2D)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            HitTarget(collision.gameObject);
        }
    }

    // 목표에 도달했을 때 처리
    private void HitTarget(GameObject enemyObject)
    {

        Enemy enemy = enemyObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(tower.damage); // 적에게 데미지 적용
        }

        // 충돌 이펙트 생성
        GameObject effectInstance = GameManager.instance.pool.Get(effectIndex);
        effectInstance.transform.position = transform.position;
        effectInstance.SetActive(true);

        DeactivateProjectile(); // 투사체 비활성화
    }

    private void DeactivateProjectile()
    {
        StopAllCoroutines();
        isActive = false; // 활성 상태 변경
        gameObject.SetActive(false); // 투사체 비활성화
    }
}
