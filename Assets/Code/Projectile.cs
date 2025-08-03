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

    [Header ("#Enemy Info")]
    private float damage; // 데미지 값
    private bool isEnemyProjectile = false; // 적의 투사체인지 여부

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
                moveDirection = target.transform.position.x < transform.position.x ? Vector3.left : Vector3.right;
                float angle = target.transform.position.x < transform.position.x ? 0f : 180f;
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

        switch(tower.projectileIndex)
        {
            case 1: 
                AudioManager.instance.PlaySFX("P_Fire");
                break;
            case 2:
                AudioManager.instance.PlaySFX("P_Thunder");
                break;
            case 3:
                AudioManager.instance.PlaySFX("P_Dark");
                break;
            case 4:
                AudioManager.instance.PlaySFX("P_Lightning");   
                break;
            case 10:
                AudioManager.instance.PlaySFX("P_Slash");
                break;
        }
    }

    public void Init(float damage, GameObject target, float speed) // 몬스터 전용
    {
        this.damage = damage;
        this.target = target;
        this.speed = speed;
        isEnemyProjectile = true;
        isActive = true;
        if (Enemy.instance.projectileIndex == 14)
        {
            this.damage = damage / 2;
            Vector3 randomOffset = Random.insideUnitCircle * 15f;
            Vector3 spawnPosition = target.transform.position + randomOffset;
            transform.position = spawnPosition;
            CameraShakeComponent cameraShake = Camera.main.GetComponent<CameraShakeComponent>();
            if (cameraShake != null)
                StartCoroutine(cameraShake.Shake(0.5f, 0.5f));
            Invoke("DeactivateProjectile", 0.433f);
        }
        else StartCoroutine(MoveToTarget());
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
    private void OnCollisionEnter2D(Collision2D collision) // 트리거가 적용되지 않는 투사체 (닿았을 때)
    {
        if (isEnemyProjectile) return;

        if (collision.collider.CompareTag("Enemy"))
        {
                HitTarget(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) // 트리거(광역 공격)가 적용되는 투사체 (겹쳤을 때)
    {
        if (isEnemyProjectile)
        {
            if (collision.CompareTag("Tower"))
            {
                Tower tower = collision.GetComponent<Tower>();
                if (tower != null)
                {
                    tower.TakeDamage(damage);
                    GameObject effectInstance = GameManager.instance.pool.Get(effectIndex);
                    effectInstance.transform.position = transform.position;
                    effectInstance.SetActive(true);
                    if (Enemy.instance.projectileIndex == 14)
                    {
                        Invoke("DeactivateProjectile", 0.433f);
                    } 
                    else DeactivateProjectile();
                }
            }
        }
        else
        {
            if (collision.CompareTag("Enemy"))
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(tower.damage); // 적에게 데미지 적용
                }

                if (tower.projectileIndex == 4 || tower.projectileIndex == 10) // S급 투사체
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

    }

    // 목표에 도달했을 때 처리
    private void HitTarget(GameObject enemyObject)
    {
        if (isEnemyProjectile)
        {
            Tower tower = enemyObject.GetComponent<Tower>();
            if (tower != null)
            {
                tower.TakeDamage(tower.damage);
            }
        }
        else
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(this.damage); // 적에게 데미지 적용
            }
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
