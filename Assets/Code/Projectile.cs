using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 3f;
    private GameObject target; // 목표 몬스터
    public int effectIndex;

    public Tower tower;

    private bool isActive; // 투사체 활성 상태 확인
    private Vector3 moveDirection;

    // 초기화 메서드: 타워와 목표 설정
    public void Init(Tower _tower, GameObject _target)
    {
        tower = _tower;
        target = _target;
        isActive = true; // 활성화 상태로 설정

        if (tower.projectileIndex == 3 || tower.projectileIndex == 10) // 특수한 프로젝타일인 경우
        {
            if (tower.projectileIndex == 3)
            {
                // 인덱스 3: 타겟을 향해 이동하며 회전
                moveDirection = (target.transform.position - transform.position).normalized;
                transform.up = moveDirection;
            }
            else // 인덱스 10: 캐릭터 기준 수평 이동
            {
                // 타워가 왼쪽을 바라보면 Vector3.left, 오른쪽이면 Vector3.right
                moveDirection = tower.flipX ? Vector3.left : Vector3.right;

                // 스프라이트를 수평(90도)으로 회전
                float angle = tower.flipX ? 0f : 180f;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            StartCoroutine(MoveStraight());
            Invoke("DeactivateProjectile", 10f);
        }
        else if (tower.projectileIndex == 4 )
        {
            transform.position = target.transform.position;
            Invoke("DeactivateProjectile", 0.433f);
        }
        else // 일반적인 프로젝타일
        {
            StartCoroutine(MoveToTarget());
        }
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

    IEnumerator MoveStraight()
    {
        while (isActive)
        {
            transform.position += moveDirection * speed * Time.deltaTime; // 현재 방향으로 이동
            if (tower.projectileIndex == 3) transform.Rotate(Vector3.forward * 1000 * Time.deltaTime);
            yield return null;
        }
    }

    // 충돌 처리 (OnCollisionEnter2D)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            HitTarget(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // 적과 충돌했을 때
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(tower.damage); // 적에게 데미지 적용
            }

            if (tower.projectileIndex == 4 || tower.projectileIndex == 10)
            {
                GameObject effectInstance = GameManager.instance.pool.Get(effectIndex);
                effectInstance.transform.position = enemy.transform.position;
                effectInstance.SetActive(true);
            }
            else
            {
                GameObject effectInstance = GameManager.instance.pool.Get(effectIndex);
                effectInstance.transform.position = transform.position;
                effectInstance.SetActive(true);
            }
                
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
