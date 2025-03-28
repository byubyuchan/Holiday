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
        if (enemyCount >= maxEnemies[level])
        {
            return;
        }
        for (int i = 0; i <= level; i++)
        {
            GameObject enemy = GameManager.instance.pool.Get(EnemyIndex);
            if (enemy == null)
            {
                continue;
            }

            Transform randomSpawnPoint = spawnPoint[Random.Range(1, spawnPoint.Length)];
            enemy.transform.position = randomSpawnPoint.position;
            enemy.GetComponent<Enemy>().Init(spawnData[Mathf.Min(i, spawnData.Length - 1)]);
            enemyCount++;
            currentEnemyCount++;
        }
    }

    public void EnemyDefeated()
    {
        currentEnemyCount--;
        if (currentEnemyCount <= 0)
        {
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
}
