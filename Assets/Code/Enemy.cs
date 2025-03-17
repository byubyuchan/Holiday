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
    public Rigidbody2D target;
    private Vector2 lastTargetPosition;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isLive || !isLive) return;

        if (target != null && agent.isOnNavMesh)
        {
            // 일정 시간마다만 목표 위치를 갱신
            if (Time.time - lastUpdateTime >= destinationUpdateInterval)
            {
                // 목표 위치가 변경되었을 때만 SetDestination 호출
                if (target.position != lastTargetPosition)
                {
                    agent.SetDestination(target.position);
                    lastTargetPosition = target.position;
                    lastUpdateTime = Time.time; // 시간 갱신
                    //Debug.LogError("이동 호출");
                }
            }
        }
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive) return;
        if (!isLive) return;
        spriter.flipX = target.position.x < rigid.position.x;
    }

    void OnEnable()
    {
        target = GameManager.instance.goal.GetComponent<Rigidbody2D>(); // 기본 목표 설정
        isLive = true;
        hp = maxHp;

        isLive = true;
        col.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower")) // Tower 태그를 가진 오브젝트 감지
        {
            target = collision.GetComponent<Rigidbody2D>(); // Tower 오브젝트를 목표로 설정
        }
        // Tower가 없을 경우 goal로 target 변경돼야함.
    }

    IEnumerator KnockBack()
    {
        yield return wait;
    }

    void Dead()
    {
    }
}
