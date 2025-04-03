using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("# Nomal Info")]
    public static Enemy instance;
    public float speed; // 이동 속도
    public float hp; // 현재 체력
    public float maxHp; // 최대 체력
    public float damage; // 공격력
    public int exp; // 경험치
    public float range;
    public float attackCooldown; // 공격 쿨타임
    public RuntimeAnimatorController[] animCon; // 애니메이션 컨트롤러 배열
    private float lastAttackTime = 0f; // 마지막 공격 시간

    [Header("# Check Info")]
    public Transform target; // 현재 목표 (타워 또는 Goal)
    private Vector2 lastTargetPosition;
    public float detectionRange = 10f; // 탐지 범위
    public float destinationUpdateInterval = 0.5f; // 목표 위치 갱신 주기
    private float lastUpdateTime = 0f;

    private Vector3 lastPosition; // 마지막 위치 저장
    private float positionCheckTimer = 0f; // 위치 변경 감지 타이머
    private const float positionCheckInterval = 5f;

    private bool isLive;
    private Animator anim;
    private Rigidbody2D rigid;
    private Collider2D col;
    private SpriteRenderer spriter;

    void Start()
    {
        target = GameManager.instance.goal.transform; // 기본 목표 설정
        lastPosition = transform.position;
    }

    void OnEnable()
    {
        isLive = true;
        hp = maxHp;

        isLive = true;
        col.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 5;

        transform.position = new Vector3(transform.position.x, transform.position.y, 0); // Z 축 고정
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHp = data.hp;
        hp = data.hp;
        exp = data.exp;
        damage = data.damage;
        range = data.Range;
        attackCooldown = data.AttackSpeed;
    }

    void Awake()
    {
        instance = this;
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive || !isLive) return;
        spriter.flipX = target != null && target.position.x < transform.position.x;
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive || !isLive) return;

        if (hp <= 0) Dead();

        if (Time.time - lastUpdateTime >= destinationUpdateInterval)
        {
            FindClosestTower(); // 가장 가까운 타워 탐색
            lastUpdateTime = Time.time; // 시간 갱신
        }

        Attack();

        MoveTowardsTarget(); // 목표를 향해 이동

        CheckPositionStuck();
    }

    private void FindClosestTower()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);

        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Tower"))
            {
                float distanceToTower = Vector3.Distance(transform.position, collider.transform.position);
                if (distanceToTower < closestDistance)
                {
                    closestDistance = distanceToTower;
                    closestTarget = collider.transform;
                }
            }
        }

        // 가장 가까운 타워를 목표로 설정하거나 Goal로 설정
        if (closestTarget != null)
        {
            target = closestTarget; // 가장 가까운 타워를 목표로 설정
        }
        else
        {
            target = GameManager.instance.goal.transform; // 기본 목표 설정
        }
    }

    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rigid.linearVelocity = direction * speed;

            // 벽과 충돌 시 방향 조정
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f);
            if (hit.collider != null && hit.collider.CompareTag("Wall"))
            {
                Vector3 adjustedDirection = Vector3.Reflect(direction, hit.normal);
                rigid.linearVelocity = adjustedDirection * speed;
            }
        }
    }

    void Dead()
    {
        anim.SetTrigger("Death");
        rigid.linearVelocity = Vector2.zero; // 이동 멈춤
        //col.enabled = false; // 충돌 비활성화
        rigid.simulated = false; // 물리 계산 비활성화

        DamageFlashEffect flashEffect = GetComponent<DamageFlashEffect>();

        if (flashEffect != null)
        {
            flashEffect.StopAllCoroutines(); // 모든 깜빡임 코루틴 중지
            spriter.color = flashEffect.originalColor; // 원래 색상으로 복구
        }

        if (gameObject.activeInHierarchy) // 활성 상태 확인
        {
            StartCoroutine(RemoveAfterDeath());
        }
        else
        {
            Debug.LogWarning($"코루틴을 시작할 수 없습니다. {gameObject.name}은 비활성화 상태입니다.");
        }
    }

    private IEnumerator RemoveAfterDeath()
    {
        yield return new WaitForSeconds(0.5f); // 사망 애니메이션 길이에 맞게 대기
        Spawner.instance.EnemyDefeated(); // 스포너에 몬스터 사망 알림
        StopAllCoroutines();
        gameObject.SetActive(false); // 비활성화하여 풀링 시스템으로 반환
    }

    private void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return; // 쿨타임 체크

        Collider2D[] hitTowers = Physics2D.OverlapCircleAll(transform.position, range); // 범위 내 타워 탐지

        foreach (Collider2D towerCollider in hitTowers)
        {
            if (towerCollider.CompareTag("Tower")) // 타워 태그 확인
            {
                Tower tower = towerCollider.GetComponent<Tower>();
                if (tower != null)
                {
                    tower.TakeDamage(damage); // 타워에 데미지 적용
                    anim.SetTrigger("Attack"); // 공격 애니메이션 실행
                    lastAttackTime = Time.time; // 마지막 공격 시간 갱신
                    break; // 한 번 공격 후 종료
                }
            }
        }
    }

    private void CheckPositionStuck()
    {
        if ((transform.position - lastPosition).sqrMagnitude < 0.01f) // 움직임이 거의 없으면 타이머 증가
        {
            positionCheckTimer += Time.fixedDeltaTime;

            if (positionCheckTimer >= positionCheckInterval) // 5초 동안 움직임이 없으면 처리
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z); // Y축을 0으로 설정
                positionCheckTimer = 0f; // 타이머 초기화
            }
        }
        else
        {
            positionCheckTimer = 0f; // 움직임이 있으면 타이머 초기화
            lastPosition = transform.position; // 마지막 위치 갱신
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (!gameObject.activeInHierarchy) // 활성 상태 확인
        {
            return;
        }

        // 데미지 효과 적용
        DamageFlashEffect flashEffect = GetComponent<DamageFlashEffect>();
        if (flashEffect != null)
        {
            flashEffect.Flash();
        }

        if (hp <= 0)
        {
            Dead();
        }
    }
}
