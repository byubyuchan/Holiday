using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public GameObject[] itemPrefabs;
    public int EnemyIndex;
    float timer;
    public int level;

    public int[] maxEnemies;  // 최대 몬스터 수
    public int currentEnemyCount = 0;  // 현재 몬스터 수
    public int enemyCount = 0;

    void Awake()
    {
        instance = this;
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
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
        for (int i = Mathf.Max(level-2, 0); i <= level; i++)
        {
            if (enemyCount >= maxEnemies[level])
            {
                return;
            }
            GameObject enemy = GameManager.instance.pool.Get(EnemyIndex); // 풀링될 프리팹 선택
            if (enemy == null || currentEnemyCount >= 150)
            {
                continue;
            }

            Transform randomSpawnPoint = spawnPoint[Random.Range(1, spawnPoint.Length)];
            enemy.transform.position = randomSpawnPoint.position;
            enemy.GetComponent<Enemy>().Init(spawnData[i]);
            enemyCount++;
            currentEnemyCount++;
        }
    }

    public void EnemyDefeated()
    {
        currentEnemyCount--;
        if (currentEnemyCount <= 0 && enemyCount >= maxEnemies[level])
        {
            currentEnemyCount = 0;
            enemyCount = 0;
            GameManager.instance.EndRound();
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
