using UnityEngine;

public class TowerMaker : MonoBehaviour
{
    [SerializeField] private GameObject meleeTowerPrefab;
    [SerializeField] private GameObject rangedTowerPrefab;
    [SerializeField] private GameObject tankTowerPrefab;

    private GameObject selectedTowerPrefab;

    private void Start()
    {
        selectedTowerPrefab = meleeTowerPrefab;  // 기본 선택 타워
    }

    public void SelectMeleeTower()
    {
        selectedTowerPrefab = meleeTowerPrefab;
        Debug.Log("근거리 타워 선택");
    }

    public void SelectRangedTower()
    {
        selectedTowerPrefab = rangedTowerPrefab;
        Debug.Log("원거리 타워 선택");
    }

    public void SelectTankTower()
    {
        selectedTowerPrefab = tankTowerPrefab;
        Debug.Log("탱커 타워 선택");
    }

    public void SpawnTower(Transform tile)
    {
        if (selectedTowerPrefab == null) return;
        Tile tileComponent = tile.GetComponent<Tile>();
        // 중복 배치 방지
        if (tileComponent.IsBuildTower)
        {
            Debug.Log("이미 타워가 설치된 타일입니다!");
            return;
        }
        Instantiate(selectedTowerPrefab, tile.position, Quaternion.identity);
        tileComponent.IsBuildTower = true;
        Debug.Log($"{selectedTowerPrefab.name} 배치됨!");
    }
}
