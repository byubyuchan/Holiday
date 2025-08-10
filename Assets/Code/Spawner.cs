using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public GameObject[] itemPrefabs;
    public int EnemyIndex;
    public int BossIndex;
    float timer;
    public int level;

    public int[] maxEnemies;  // 최대 몬스터 수
    public int currentEnemyCount = 0;  // 현재 몬스터 수
    public int enemyCount = 0;
    public int SpawnCount = 1;
    public bool isEnd = false;

    public GameObject enemy;

    public SpriteRenderer spriteRenderer;
    public RuntimeAnimatorController animCon;
    public Animator anim;

    [Header("# Game Object")]
    public Talent RoundUp;

    void Awake()
    {
        instance = this;
        spawnPoint = GetComponentsInChildren<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {

        if (enemyCount >= maxEnemies[level]) return;

        if (isEnd) return;

        if (!GameManager.instance.isStart) return;

        timer += Time.deltaTime;

        if (timer > spawnData[level].spawnTime)
        {
            Spawn();
            timer = 0;
        }
    }

    void Spawn()
    {
        if (level == 5) // 보스몬스터 소환
        {
            if (enemyCount >= maxEnemies[level])
            {
                return;
            }

            enemy = GameManager.instance.pool.Get(BossIndex); // 풀링될 프리팹 선택
            Transform randomSpawnPoint = spawnPoint[Random.Range(1, spawnPoint.Length)];
            enemy.transform.position = randomSpawnPoint.position;
            enemy.GetComponent<Enemy>().Init(spawnData[level]);
            GameManager.instance.bossEnemy = enemy.GetComponent<Enemy>();
            enemyCount++;
            currentEnemyCount++;

            CutsceneManager.instance.PlayBossCutscene(enemy.transform, "까마귀 왕", 0.5f);
        }
        else
        {
            for (int i = Mathf.Max(level - 2, 0); i <= level; i++)
            {
                if (enemyCount >= maxEnemies[level])
                {
                    break;
                }
                GameObject enemy = GameManager.instance.pool.Get(EnemyIndex); // 풀링될 프리팹 선택

                    // 문제 되는 부분 !! //
                //if (enemy == null || currentEnemyCount >= 150)
                //{
                //    break;
                //}

                Transform randomSpawnPoint = spawnPoint[Random.Range(1, spawnPoint.Length)];
                enemy.transform.position = randomSpawnPoint.position;
                enemy.GetComponent<Enemy>().Init(spawnData[i]);
                enemy.GetComponent<Enemy>().number = SpawnCount;
                if (enemy.GetComponent<Enemy>().number >= maxEnemies[level])
                {
                    // 현재 스포너 카운트 먼저 보정
                    enemy.GetComponent<Enemy>().Dead();
                    return;
                }
                enemyCount++;
                currentEnemyCount++;
                SpawnCount++;
            }
        }
    }

    public void EnemyDefeated()
    {
        GameManager.instance.kill++;
        currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
        if (currentEnemyCount <= 0 && enemyCount >= maxEnemies[level])
        {
            if (isEnd) return;
            isEnd = true;
            GameManager.instance.EndRound();
            if (level<=4) RoundUp.Show();
            PoolManager.instance.DeactivateAll();
        }
    }
}
// 직렬화 필요 (속성 부여)
[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int hp;
    public float damage;
    public float speed;
    public int exp;
    public float Range;
    public float AttackSpeed;
    public string AttackType;
    public int projectileIndex;
}
