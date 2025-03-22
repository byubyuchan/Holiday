using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;
    public float speed;
    public float hp;
    public float maxHp;
    public int exp;
    public RuntimeAnimatorController[] animCon;

    public Rigidbody2D target; // 현재 목표
    private Vector2 lastTargetPosition;

    public float detectionRange = 10f; // 탐지 범위

    public float destinationUpdateInterval = 0.5f; // 목표 위치 갱신 주기
    private float lastUpdateTime = 0f;

    bool isLive;
    Animator anim;

    Rigidbody2D rigid;
    Collider2D col;
    SpriteRenderer spriter;

    WaitForFixedUpdate wait;

    NavMeshAgent agent;

    bool IsBoss;

    void Start()
    {
        // NavMesh 초기화 코드
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position; // 가장 가까운 NavMesh 위로 이동
            }
        }
    }

    void Awake()
    {
        instance = this;
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;  // 2D에서는 회전 필요 없음
        agent.updateUpAxis = false;    // Up Axis 비활성화
        agent.stoppingDistance = 0.2f; // 목표에 가까워지면 멈추는 거리 설정
        target = GameManager.instance.goal.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!GameManager.instance.isLive || !isLive) return;

        if (hp <= 0) Dead();

        if (Time.time - lastUpdateTime >= destinationUpdateInterval)
        {
            FindClosestTower(); // 가장 가까운 타워 탐색
            UpdateDestination(); // 목표 위치 갱신
            lastUpdateTime = Time.time; // 시간 갱신
        }
    }

    private void FindClosestTower()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);

        float closestDistance = Mathf.Infinity;
        Rigidbody2D closestTarget = null;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Tower"))
            {
                float distanceToTower = Vector3.Distance(transform.position, collider.transform.position);
                if (distanceToTower < closestDistance)
                {
                    closestDistance = distanceToTower;
                    closestTarget = collider.GetComponent<Rigidbody2D>();
                }
            }
        }
        if (closestTarget != null)
        {
            target = closestTarget; // 가장 가까운 타워를 목표로 설정
        }
        else target = GameManager.instance.goal.GetComponent<Rigidbody2D>();
    }


    private void UpdateDestination()
    {
        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position); // 목표 위치로 이동
            lastTargetPosition = target.position;  // 마지막 목표 위치 갱신
        }
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive || !isLive) return;

        spriter.flipX = target != null && target.position.x < rigid.position.x;
    }

    void OnEnable()
    {
        isLive = true;
        hp = maxHp;

        isLive = true;
        col.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);

        FindClosestTower(); // 활성화 시 가장 가까운 타워 탐색
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        agent.speed = speed;
        maxHp = data.hp;
        hp = data.hp;
        exp = data.exp;
        IsBoss = data.spawnTime >= 5;
    }

    IEnumerator KnockBack()
    {
        yield return wait;
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
