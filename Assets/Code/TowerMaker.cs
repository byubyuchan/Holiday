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

    public void SpawnTower(Tile tile)
    {
        if (selectedTowerPrefab == null) return;

        if (tile.IsBuildTower)
        {
            Debug.Log("이미 타워가 설치된 타일입니다!");
            return;
        }

        GameObject towerObject = Instantiate(selectedTowerPrefab, tile.transform.position, Quaternion.identity);

        Tower tower = towerObject.GetComponent<Tower>();
        if (tower != null)
        {
            tower.tile = tile; // 설치된 타일을 참조하도록 설정
            tile.IsBuildTower = true; // 설치된 상태로 변경
        }
    }
}
