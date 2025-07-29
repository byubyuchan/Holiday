using Unity.VisualScripting;
using UnityEngine;

public class TowerMaker : MonoBehaviour
{
    [SerializeField] private GameObject meleeTowerPrefab;
    [SerializeField] private GameObject rangedTowerPrefab;
    [SerializeField] private GameObject tankTowerPrefab;
    [SerializeField] private Transform towerParent;

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
        if (tile.IsBuildTower) // 이미 타워가 설치된 타일인지 확인
        {
            StartCoroutine(GameManager.instance.ShowMessage("이미 용사가 소환된 지역입니다."));
            return;
        }
        else if (GameManager.instance.isStart)
        {
            StartCoroutine(GameManager.instance.ShowMessage("전투 중엔 용사를 부를 수 없습니다!"));
            return;
        }
        else if (selectedTowerPrefab == null)
        {
            StartCoroutine(GameManager.instance.ShowMessage("용사가 결정되지 않았습니다!"));
            return;
        }
        else if (GameManager.instance.Gold < 5)
        {
            StartCoroutine(GameManager.instance.ShowMessage("용사를 고용할 골드가 부족합니다."));
            return;
        }
        else if (CutsceneManager.instance.cutsceneflag == 1) 
        {
            return;
        }

        GameObject towerObject = Instantiate(selectedTowerPrefab, tile.transform.position, Quaternion.identity);
        towerObject.transform.SetParent(towerParent);
        Tower tower = towerObject.GetComponent<Tower>();
        GameManager.instance.Gold -= 5;

        if (tower != null)
        {
            tower.tile = tile; // 설치된 타일을 참조하도록 설정
            tile.IsBuildTower = true; // 설치된 상태로 변경ㅒ
            tile.currentTower = tower;
        }
    }
}
