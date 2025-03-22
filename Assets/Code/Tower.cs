using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Tower : MonoBehaviour
{
    // 타워의 데이터 클래스
    public TowerData[] towerData;  // 타워 데이터 (각 타워마다 설정)

    public string towerType;  // 타워 유형
    public int hp;            // 체력
    public float range;       // 사거리
    public int damage;        // 공격력

    public TowerAttack towerattack;
    public RuntimeAnimatorController[] animCon;
    public Animator anim;

    private void Awake()
    {
        towerattack = GetComponent<TowerAttack>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        ApplyTowerData();
        // 확률에 맞게 인덱스 번호 설정됨.
        Init(towerData[0]);
    }

    void ApplyTowerData()
    {

    }

    public void Init(TowerData towerData)
    {


        towerType = towerData.towerType;
        hp = towerData.HP;
        range = towerData.Range;
        damage = towerData.Damage;

        // 애니메이터 컨트롤러를 확률적으로 선택
        if (animCon.Length > 0 && anim != null)
        {
            int randomIndex = Random.Range(0, animCon.Length);
            anim.runtimeAnimatorController = animCon[randomIndex];
        }
    }
}

[System.Serializable]
public class TowerData
{
    public string towerType;  // 타워 유형
    public int HP;            // 체력
    public float Speed;       // 공격 속도
    public float Range;       // 사거리
    public int Damage;        // 공격력
    public int Cost;          // 비용
    public int Star;          // 별 등급
    [TextArea] public string Desc;  // 설명
}