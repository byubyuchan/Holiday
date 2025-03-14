using UnityEngine;

public class Tower : MonoBehaviour
{
    // 타워의 데이터 클래스
    [System.Serializable]
    public class TowerData
    {
        public string towerType;  // 타워 유형
        public int HP;            // 체력
        public float Speed;       // 공격 속도
        public int Damage;        // 공격력
        public int Cost;          // 비용
        public int Star;          // 별 등급
        [TextArea] public string Desc;  // 설명
    }

    public TowerData towerData;  // 타워 데이터 (각 타워마다 설정)
    public int damage;
    public static Tower instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ApplyTowerData();
        Init(towerData);
    }

    void ApplyTowerData()
    {

    }

    void Init(TowerData towerData)
    {
        damage = towerData.Damage;
    }
}