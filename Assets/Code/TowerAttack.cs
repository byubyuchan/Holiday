using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    public Transform firePoint; // 투사체 발사 위치
    public GameObject[] projectilePrefabs; // 서로 다른 투사체 프리팹

    public Tower tower;

    private Animator towerAnim;  // 타워의 애니메이터
    private SpriteRenderer spriteRenderer;

    public GameObject meleeEffectPrefab;
    private bool isAttacking = false;
    private float attackCooldown = 0f; // 쿨타임 시간

    private void Awake()
    {
        firePoint = this.transform;
        towerAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        tower = GetComponent<Tower>();
    }

    private void Update()
    {
        // 쿨타임 감소
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0)
            {
                isAttacking = false; // 쿨타임 종료
            }
        }
    }

    public void Attack(GameObject target)
    {
        if (target == null || isAttacking)
        {
            return;
        }

        Vector2 direction = (target.transform.position - transform.position).normalized;

        // 타워가 왼쪽을 바라보도록 flipX 설정
        spriteRenderer.flipX = direction.x > 0;

        switch (tower.towerType)
        {
            case "Melee":
                PerformMeleeAttack(target);
                break;

            case "Range":
                PerformRangeAttack(target);
                break;

            default:
                break;
        }
    }

    private void PerformMeleeAttack(GameObject target)
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(tower.damage);
        }

        isAttacking = true; // 공격 시작
        attackCooldown = 1f; // 쿨타임 설정
        towerAnim.SetTrigger("Attack"); // 애니메이션 실행

        Instantiate(meleeEffectPrefab, target.transform.position, Quaternion.identity);
    }

    private void PerformRangeAttack(GameObject target)
    {
        if (projectilePrefabs == null || projectilePrefabs.Length == 0)
        {
            return;
        }

        isAttacking = true; // 공격 시작
        attackCooldown = 1f; // 쿨타임 설정
        towerAnim.SetTrigger("Attack"); // 애니메이션 실행

        int projectileIndex = 0; // 사용할 프리팹 인덱스
        GameObject projectile = Instantiate(projectilePrefabs[projectileIndex], firePoint.position, Quaternion.identity);

        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Init(tower, target); // 목표 설정
        }
    }
}
