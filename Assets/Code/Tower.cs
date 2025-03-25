using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Tower : MonoBehaviour
{
    // 타워의 데이터 클래스
    public Tile tile;
    public TowerData[] towerData;  // 타워 데이터 (각 타워마다 설정)

    public string towerType;  // 타워 유형
    public float maxHp;
    public float hp;            // 체력
    public float range;       // 사거리
    public int damage;        // 공격력

    public TowerAttack towerattack;
    public RuntimeAnimatorController[] animCon;
    public Animator anim;

    public float[] probabilities = { 50f, 20f, 15f, 10f, 5f };

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

    private void Update()
    {
    }

    private void RemoveTower()
    {
        if (tile != null)
        {
            tile.IsBuildTower = false; // 설치된 타일의 상태를 초기화
            Debug.Log($"타워가 제거되었습니다. {tile.name}의 IsBuildTower가 false로 변경되었습니다.");
        }

        Destroy(gameObject); // 타워 제거
    }

    void ApplyTowerData()
    {

    }

    public void Init(TowerData towerData)
    {
        towerType = towerData.towerType;
        maxHp = towerData.HP;
        hp = towerData.HP;
        range = towerData.Range;
        damage = towerData.Damage;

        // 애니메이터 컨트롤러를 확률적으로 선택
        if (animCon.Length > 0 && anim != null)
        {
            anim.runtimeAnimatorController = animCon[GetRandomIndex()];
        }
    }
    public void TakeDamage(float damage)
    {
        hp -= damage;

        // 데미지 효과 적용
        DamageFlashEffect flashEffect = GetComponent<DamageFlashEffect>();
        if (flashEffect != null)
        {
            flashEffect.Flash();
        }
        if (hp <= 0)
        {
            RemoveTower();
        }
    }
    private int GetRandomIndex()
    {
        float randomValue = Random.Range(0f, 100f); // 0~100 사이의 랜덤 값 생성
        float cumulativeProbability = 0f;

        for (int i = 0; i < probabilities.Length; i++) // 배열의 첫 번째 요소부터 순회
        {
            cumulativeProbability += probabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                Debug.Log(randomValue);
                return i; // 랜덤 값이 누적 확률에 도달하면 해당 인덱스 반환
            }
        }

        return probabilities.Length - 1; // 기본적으로 마지막 인덱스 반환
    }

}

[System.Serializable]
public class TowerData
{
    public string towerType;  // 타워 유형
    public float HP;            // 체력
    public float Speed;       // 공격 속도
    public float Range;       // 사거리
    public int Damage;        // 공격력
    public int Cost;          // 비용
    public int Star;          // 별 등급
    [TextArea] public string Desc;  // 설명
}