using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;
    public float speed;
    public float hp;
    public float maxHp;
    public int exp;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

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
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position; // 가장 가까운 NavMesh 위로 이동
            }
            else
            {
                Debug.LogError("❌ 주변에 NavMesh가 없습니다. 씬 확인 필요!");
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
    void FixedUpdate()
    {
        if (!GameManager.instance.isLive || !isLive) return;

        if (agent.isOnNavMesh && target != null)
        {
            agent.SetDestination(target.position);
        }

        if (agent.speed == 0)
        {
            agent.speed = speed;
        }

        // Vector2 dirVec = target.position - rigid.position;
        // Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        // rigid.MovePosition(rigid.position + nextVec);
        // rigid.linearVelocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive) return;
        if (!isLive) return;
        spriter.flipX = target.position.x < rigid.position.x;
    }

    void OnEnable()
    {
        target = GameManager.instance.goal.GetComponent<Rigidbody2D>();
        isLive = true;
        // maxHp로 해주는이유 = 풀링 시 되살아난다?
        hp = maxHp;

        isLive = true;
        col.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
    }

    public void Init(SpawnData data)
    {
        // 애니메이션의 타입을 animCon에 저장한 스프라이트의 타입으로 변경한다.
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        agent.speed = speed;
        maxHp = data.hp;
        hp = data.hp;
        exp = data.exp;
        IsBoss = data.spawnTime >= 5;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!collision.CompareTag("Bullet") || !isLive) return;

    //    //hp -= collision.GetComponent<Bullet>().damage;
    //    StartCoroutine(KnockBack());

    //    if (hp > 0)
    //    {
    //        anim.SetTrigger("Hit");
    //        AudioManager.instance.PlaySFX(AudioManager.SFX.Hit);
    //    }
    //    else
    //    {
    //        isLive = false;
    //        col.enabled = false;
    //        rigid.simulated = false;
    //        spriter.sortingOrder = 1;
    //        anim.SetBool("Dead", true);
    //        if (GameManager.instance.isLive) AudioManager.instance.PlaySFX(AudioManager.SFX.Dead);
    //    }
    //}

    IEnumerator KnockBack()
    {
        //yield return new WaitForSeconds(2); // 2초간 휴식
        yield return wait; // 1 물리 프레임 휴식
                           //Vector3 playerPos = GameManager.instance.player.transform.position;
                           //Vector3 dirVec = transform.position - playerPos;
                           //rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);

    }

    void Dead()
    {
        GameManager.instance.kill++;
        GameManager.instance.GetExp(exp);

        //if (IsBoss)
        //{
        //    int randomIndex = Random.Range(0, Drop.instance.itemPrefabs.Length);
        //    GameObject item = Instantiate(
        //        Drop.instance.itemPrefabs[randomIndex],
        //        transform.position,
        //        Quaternion.identity
        //    );
        //    item.transform.parent = Spawner.instance.transform;
        //}
        if (Random.value < 0.01f) // 1% 확률로 아이템 스폰
        {
            int itemIndex = Random.Range(0, Spawner.instance.itemPrefabs.Length);
            GameObject item = Instantiate(Spawner.instance.itemPrefabs[itemIndex]);
            item.transform.position = this.transform.position;
        }
        gameObject.SetActive(false);
    }
}
