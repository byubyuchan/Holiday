using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public GameObject[] itemPrefabs;
    public float levelTime;
    public int EnemyIndex;
    float timer;
    public int level;

    public int maxEnemies = 150;  // 최대 몬스터 수
    public int currentEnemyCount = 0;  // 현재 몬스터 수

    void Awake()
    {
        // Components = 다수의 자식 선택 + 자기 자신도 포함
        instance = this;
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    void Spawn()
    {
        if (currentEnemyCount < maxEnemies)
        {
            for (int i = 0; i <= level; i++)
            {
                GameObject enemy = GameManager.instance.pool.Get(EnemyIndex);
                enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
                enemy.GetComponent<Enemy>().Init(spawnData[i]);
                currentEnemyCount++;
            }
        }
    }

    void Update()
    {
        if (!GameManager.instance.isLive) return;
        timer += Time.deltaTime;
        // 정수형으로 반내림 
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

        if (timer > spawnData[level].spawnTime)
        {
            Spawn();
            timer = 0;
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
