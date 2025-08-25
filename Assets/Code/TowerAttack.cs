
using System.Collections;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    public Transform firePoint; // 투사체 발사 위치

    public Tower tower;

    private Animator towerAnim;  // 타워의 애니메이터
    private SpriteRenderer spriteRenderer;

    public int meleeEffectIndex;
    private bool isAttacking = false;
    public float attackCooldown; // 쿨타임 시간

    public float skillCooldown = 20f;   // 전체 쿨타임
    private float currentSkillCooldown; // 현재 남은 쿨타임

    private void Awake()
    {
        firePoint = this.transform;
        towerAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        tower = GetComponent<Tower>();
    }

    private void Start()
    {
        // A+ 등급 타워일 경우에만 쿨타임 초기화
        if (tower.cost == "A+")
        {
            currentSkillCooldown = skillCooldown;
        }
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

        if (tower.cost != "A+" || GameManager.instance.isCutsceneActive) return;

        // 스킬이 준비되지 않았고, 전투 중일 때만 쿨타임 감소
        if (currentSkillCooldown > 0 && GameManager.instance.isStart)
        {
            currentSkillCooldown -= Time.deltaTime;
        }

        // 쿨타임이 다 되면 스킬 발동
        if (currentSkillCooldown <= 0)
        {
            ActivateSpecialSkill();
        }
    }

    private void ActivateSpecialSkill()
    {
        // 쿨타임 초기화 (중복 실행 방지)
        currentSkillCooldown = skillCooldown;

        GameManager.instance.StartCutsceneMode();

        if (towerAnim != null)
        {
            towerAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        towerAnim.SetTrigger("Jump");
        AudioManager.instance.PlaySFX("P_Heal");
        CutsceneManager.instance.PlayTowerCutscene(transform, "스킬 발동!", 0.35f, 1, ApplySkillEffect);

        
    }

    private void ApplySkillEffect()
    {
        
        Debug.Log(gameObject.name + "의 스킬 효과가 발동됩니다!");

        if (towerAnim != null)
        {
            towerAnim.updateMode = AnimatorUpdateMode.Normal;
        }

        GameManager.instance.EndCutsceneMode();
    }

    public void Attack(GameObject target)
    {
        if (target == null || isAttacking)
        {
            return;
        }

        Vector2 direction = (target.transform.position - transform.position).normalized;

        // 타워가 왼쪽을 바라보도록 flipX 설정
        if (tower.flipX == false) spriteRenderer.flipX = direction.x > 0;
        else spriteRenderer.flipX = !(direction.x > 0);


        switch (tower.towerType)
        {
            case "Melee":
                if (tower.IsAttackChange) PerformRoundAttack();
                else PerformMeleeAttack(target);
                break;

            case "Range":
                PerformRangeAttack(target);
                break;

            case "Round":
                PerformRoundAttack();
                break;

            case "Tank" :
                PerformRoundAttack();
                break;

            case "Heal":
                PerformHeal();
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
        attackCooldown = tower.speed; // 쿨타임 설정
        towerAnim.SetTrigger("Attack"); // 애니메이션 실행

        string[] attackKeys = { "P_Attack1", "P_Attack2", "P_Attack3" };
        string randomKey = attackKeys[Random.Range(0, attackKeys.Length)];
        AudioManager.instance.PlaySFX(randomKey);

        GameObject effectInstance = GameManager.instance.pool.Get(meleeEffectIndex);
        effectInstance.transform.position = target.transform.position;
        effectInstance.SetActive(true); // 이펙트 활성화
    }

    public void PerformRangeAttack(GameObject target)
    {
        GameObject projectileObject = GameManager.instance.pool.Get(tower.projectileIndex); // 풀링된 투사체 가져오기
        projectileObject.transform.position = firePoint.position; // 발사 위치 설정
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Init(tower, target); // 초기화 및 목표 설정
        }
        isAttacking = true; // 공격 시작
        attackCooldown = tower.speed; // 쿨타임 설정
        towerAnim.SetTrigger("Attack"); // 애니메이션 실행

        if (projectile.effectIndex == 5 || projectile.effectIndex == 9)
        {
            CameraShakeComponent.instance.StartShake(0.3f, 0.3f);
        }
    }

    private void PerformRoundAttack()
    {
        // 타워 주변 범위 내 적 감지
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, tower.range);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null || enemy.hp > 0)
                {
                    enemy.TakeDamage(tower.damage); // 적에게 데미지 적용
                    GameObject effectInstance = GameManager.instance.pool.Get(meleeEffectIndex);
                    effectInstance.transform.position = enemy.transform.position;
                    effectInstance.SetActive(true);
                }
            }
        }

        string[] attackKeys = { "P_Attack1", "P_Attack2", "P_Attack3" };
        string randomKey = attackKeys[Random.Range(0, attackKeys.Length)];
        AudioManager.instance.PlaySFX(randomKey);

        isAttacking = true; // 공격 시작
        attackCooldown = tower.speed; // 쿨타임 설정
        towerAnim.SetTrigger("Attack"); // 애니메이션 실행
    }

    private void PerformHeal()
    {
        // 주변 타워 감지
        Collider2D[] nearTowers = Physics2D.OverlapCircleAll(transform.position, tower.range);

        foreach (Collider2D towerCollider in nearTowers)
        {
            if (towerCollider.CompareTag("Tower"))
            {
                Tower nearTower = towerCollider.GetComponent<Tower>();
                if (nearTower != null)
                {
                    nearTower.hp += tower.damage; // 타워 회복
                    if (nearTower.hp >= nearTower.maxHp) nearTower.hp = nearTower.maxHp;
                    GameObject effectInstance = GameManager.instance.pool.Get(15);
                    effectInstance.transform.position = nearTower.transform.position;
                    effectInstance.SetActive(true);
                }
            }
        }
        AudioManager.instance.PlaySFX("P_Heal");
        isAttacking = true; // 힐 시작
        attackCooldown = tower.speed; // 쿨타임 설정
        towerAnim.SetTrigger("Attack"); // 힐 애니메이션 실행
    }
}
