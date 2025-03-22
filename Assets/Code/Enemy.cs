using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;
    public float speed; // 이동 속도
    public float hp; // 현재 체력
    public float maxHp; // 최대 체력
    public float damage; // 공격력
    public int exp; // 경험치
    public RuntimeAnimatorController[] animCon; // 애니메이션 컨트롤러 배열

    public Transform target; // 현재 목표 (타워 또는 Goal)
    private Vector2 lastTargetPosition;

    public float detectionRange = 10f; // 탐지 범위
    public float destinationUpdateInterval = 0.5f; // 목표 위치 갱신 주기
    private float lastUpdateTime = 0f;

    public float repelForce = 5.0f; // 겹친 몬스터를 밀어내는 힘
    public float detectionRadius = 1.0f; // 주변 몬스터를 감지하는 반경

    private bool isLive;
    private Animator anim;

    private Rigidbody2D rigid;
    private Collider2D col;
    private SpriteRenderer spriter;

    private WaitForFixedUpdate wait;
    public RuntimeAnimatorController deathAnimatorController; // 사망 애니메이션 컨트롤러

    void Start()
    {
        target = GameManager.instance.goal.transform; // 기본 목표 설정
    }

    void Awake()
    {
        instance = this;
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
    }

    void Update()
    {
        if (!GameManager.instance.isLive || !isLive) return;

        if (hp <= 0) Dead();

        if (Time.time - lastUpdateTime >= destinationUpdateInterval)
        {
            FindClosestTower(); // 가장 가까운 타워 탐색
            lastUpdateTime = Time.time; // 시간 갱신
        }

        MoveTowardsTarget(); // 목표를 향해 이동
        PreventOverlap();

    }
    private void PreventOverlap()
    {
        // 주변 몬스터 감지
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != this.gameObject && collider.CompareTag("Enemy"))
            {
                // 밀어내는 방향 계산
                Vector3 repelDirection = (transform.position - collider.transform.position).normalized;
                rigid.AddForce(repelDirection * repelForce, ForceMode2D.Impulse);
            }
        }
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
            Debug.Log($"탐지된 가장 가까운 타워: {target.name}");
        }
        else
        {
            target = GameManager.instance.goal.transform; // 기본 목표 설정
            Debug.Log("탐지된 타워가 없습니다. Goal로 이동합니다.");
        }
    }

    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rigid.linearVelocity = direction * speed;

            Debug.Log($"목표 {target.name}을(를) 향해 이동 중");
        }
        else
        {
            rigid.linearVelocity = Vector2.zero; // 목표가 없으면 멈춤
            Debug.LogWarning("목표가 설정되지 않았습니다!");
        }
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive || !isLive) return;

        spriter.flipX = target != null && target.position.x < transform.position.x;
    }

    void OnEnable()
    {
        isLive = true;
        hp = maxHp;

        isLive = true;
        col.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 5;
        anim.SetBool("Dead", false);

        transform.position = new Vector3(transform.position.x, transform.position.y, 0); // Z 축 고정

        FindClosestTower(); // 활성화 시 가장 가까운 타워 탐색
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHp = data.hp;
        hp = data.hp;
        exp = data.exp;
        damage = data.damage;
    }

    IEnumerator KnockBack()
    {
        yield return wait;
    }

    void Dead()
    {
        if (deathAnimatorController != null)
        {
            anim.runtimeAnimatorController = deathAnimatorController; // 사망 애니메이션 컨트롤러 변경
        }

        rigid.linearVelocity = Vector2.zero; // 이동 멈춤
        col.enabled = false; // 충돌 비활성화
        rigid.simulated = false; // 물리 계산 비활성화

        StartCoroutine(RemoveAfterDeath()); // 일정 시간 후 삭제
    }

    private IEnumerator RemoveAfterDeath()
    {
        yield return new WaitForSeconds(0.5f); // 사망 애니메이션 길이에 맞게 대기
        Destroy(gameObject); // 오브젝트 삭제
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
        {
            collision.GetComponent<Tower>().hp -= damage; // 타워에 데미지 적용
            Debug.Log($"타워 {collision.name}에 {damage} 데미지를 입힘");
        }

        if (collision.CompareTag("GUARD"))
        {
            Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized + new Vector3(1f, 1f, 0).normalized;
            rigid.AddForce(knockbackDirection * 1f, ForceMode2D.Impulse);
        }
    }
}
