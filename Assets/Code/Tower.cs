using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Tower : MonoBehaviour
{
    // 타워의 데이터 클래스
    public Tile tile;
    public TowerData[] towerData;  // 타워 데이터 (각 타워마다 설정)
    private SpriteRenderer spriteRenderer;

    public string towerType;  // 타워 유형
    public float maxHp;
    public float hp;            // 체력
    public float range;       // 사거리
    public float speed;
    public int damage;        // 공격력
    public string cost;
    public int projectileIndex;
    public bool flipX;

    public TowerAttack towerattack;
    public RuntimeAnimatorController[] animCon;
    public Animator anim;

    public int towerindex;
    public float[] probabilities = { 50f, 20f, 15f, 10f, 5f };

    private void Awake()
    {
        towerattack = GetComponent<TowerAttack>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        towerindex = GetRandomIndex();
        // 확률에 맞게 인덱스 번호 설정됨.
        Init(towerData[towerindex]); // 소환 로직
        SortingOrder();
        if (cost == "C")
        {
            CutsceneManager.instance.PlayTowerCutscene(transform, cost + "급 용사 소환!!", 0.9f, 1);
            AudioManager.instance.PlaySFX("Spawn_C");
        }
        if (cost == "B")
        {
            CutsceneManager.instance.PlayTowerCutscene(transform, cost + "급 용사 소환!!", 0.9f, 1);
            AudioManager.instance.PlaySFX("Spawn_B");
        }
        if (cost == "A")
        {
            CutsceneManager.instance.PlayTowerCutscene(transform, cost + "급 용사 소환!!", 0.7f, 2);
            AudioManager.instance.PlaySFX("Spawn_A");
        }
        if (cost == "S")
        {
            CutsceneManager.instance.PlayTowerCutscene(transform, cost + "급 용사 소환!!", 0.3f, 4);
            AudioManager.instance.PlaySFX("Spawn_S");
        }
    }

    public void RemoveTower()
    {
        if (tile != null)
        {
            tile.IsBuildTower = false; // 설치된 타일의 상태를 초기화
        }

        Destroy(gameObject); // 타워 제거
    }

    public void Init(TowerData towerData)
    {
        towerType = towerData.towerType;
        maxHp = towerData.HP;
        hp = towerData.HP;
        range = towerData.Range;
        damage = towerData.Damage;
        speed = towerData.Speed;
        flipX = towerData.FlipX;
        cost = towerData.Cost;
        projectileIndex = towerData.ProjectileIndex;

        spriteRenderer.flipX = flipX;

        // 애니메이터 컨트롤러를 확률적으로 선택
        if (animCon.Length > 0 && anim != null)
        {
            anim.runtimeAnimatorController = animCon[towerindex];
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
            string[] attackKeys = { "P_Hit1", "P_Hit2"};
            string randomKey = attackKeys[Random.Range(0, attackKeys.Length)];
            AudioManager.instance.PlaySFX(randomKey);
        }
        if (hp <= 0)
        {
            string[] attackKeys = { "P_Dead1", "P_Dead2" };
            string randomKey = attackKeys[Random.Range(0, attackKeys.Length)];
            AudioManager.instance.PlaySFX(randomKey);
            CameraShakeComponent.instance.StartShake(0.1f, 1f);
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

    public void MoveToTile(Tile newTile, bool isSwap)
    {
        if (tile != null && isSwap == false)
        {
            tile.IsBuildTower = false;
            tile.currentTower = null;
        }

        newTile.IsBuildTower = true;
        newTile.currentTower = this;
        tile = newTile;
        
        transform.position = newTile.transform.position; // 위치 변경
        SortingOrder();
    }

    public void SwapWithTower(Tower otherTower)
    {
        Tile tempTile = tile;

        // 현재 타워의 상태 변경
        MoveToTile(otherTower.tile, true);
        SortingOrder();

        // 다른 타워의 상태 변경
        otherTower.MoveToTile(tempTile, true);
        otherTower.SortingOrder();
    }

    public void SortingOrder()
    {
        Transform parentTransform = tile.transform.parent; // 부모 오브젝트 가져오기
        float order = (parentTransform.position.y - 2.5f) / 1.5f;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-order);
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
    public string Cost;          // 비용
    public int Star;          // 별 등급
    public int ProjectileIndex;
    public bool FlipX;
    [TextArea] public string Desc;  // 설명
}