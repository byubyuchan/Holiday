using Unity.VisualScripting;
using UnityEngine;

public class TowerMaker : MonoBehaviour
{
    [SerializeField] private GameObject meleeTowerPrefab;
    [SerializeField] private GameObject rangedTowerPrefab;
    [SerializeField] private GameObject tankTowerPrefab;
    [SerializeField] private Transform towerParent;

    private GameObject selectedTowerPrefab;
    private int Pay;

    public void SelectMeleeTower()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;
        selectedTowerPrefab = meleeTowerPrefab;
        GameManager.instance.ShowMessage("전사 영웅을 모집합니다!");
        Pay = 8;
    }

    public void SelectRangedTower()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;
        selectedTowerPrefab = rangedTowerPrefab;
        GameManager.instance.ShowMessage("마법사 영웅을 모집합니다!");
        Pay = 8;
    }

    public void SelectTankTower()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;
        selectedTowerPrefab = tankTowerPrefab;
        GameManager.instance.ShowMessage("전위 영웅을 모집합니다!");
        Pay = 8;
    }

    public void SelevtRandomTower()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;
        GameObject[] TowerArr = { meleeTowerPrefab, rangedTowerPrefab, tankTowerPrefab};
        GameObject RandomPrefab = TowerArr[Random.Range(0, TowerArr.Length)];
        selectedTowerPrefab = RandomPrefab;
        GameManager.instance.ShowMessage("무작위 영웅을 모집합니다!");
        Pay = 5;
    }

    public void SpawnTower(Tile tile)
    {
        if (tile.IsBuildTower) // 이미 타워가 설치된 타일인지 확인
        {
            GameManager.instance.ShowMessage("이미 용사가 소환된 지역입니다!");
            AudioManager.instance.PlaySFX("Cant");
            CameraShakeComponent.instance.StartShake();
            return;
        }
        else if (GameManager.instance.isStart)
        {
            GameManager.instance.ShowMessage("전투 중엔 용사를 고용할 수 없습니다!");
            AudioManager.instance.PlaySFX("Cant");
            CameraShakeComponent.instance.StartShake();
            return;
        }
        else if (selectedTowerPrefab == null)
        {
            GameManager.instance.ShowMessage("고용할 용사가 결정되지 않았습니다!");
            AudioManager.instance.PlaySFX("Cant2");
            CameraShakeComponent.instance.StartShake();
            return;
        }
        else if (GameManager.instance.Gold < Pay)
        {
            GameManager.instance.ShowMessage("용사를 고용할 골드가 모자랍니다!");
            AudioManager.instance.PlaySFX("Cant2");
            CameraShakeComponent.instance.StartShake();
            return;
        }
        else if (CutsceneManager.instance.cutsceneflag == 1)
        {
            return;
        }

        GameObject towerObject = Instantiate(selectedTowerPrefab, tile.transform.position, Quaternion.identity);
        towerObject.transform.SetParent(towerParent);
        Tower tower = towerObject.GetComponent<Tower>();
        GameManager.instance.Gold -= Pay;

        if (tower != null)
        {
            tower.tile = tile; // 설치된 타일을 참조하도록 설정
            tile.IsBuildTower = true; // 설치된 상태로 변경ㅒ
            tile.currentTower = tower;
        }

        if (Pay == 5)
        {
            GameObject[] TowerArr = { meleeTowerPrefab, rangedTowerPrefab, tankTowerPrefab };
            GameObject RandomPrefab = TowerArr[Random.Range(0, TowerArr.Length)];
            selectedTowerPrefab = RandomPrefab;
        }
    }
}
