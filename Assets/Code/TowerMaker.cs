using Unity.VisualScripting;
using UnityEngine;
using System;

public class TowerMaker : MonoBehaviour
{
    [SerializeField] private GameObject meleeTowerPrefab;
    [SerializeField] private GameObject rangedTowerPrefab;
    [SerializeField] private GameObject tankTowerPrefab;
    [SerializeField] private Transform towerParent;

    public GameObject selectedTowerPrefab;
    public static TowerMaker instance;
    private int Pay;
    public int MeleePay = 8;
    public int RangedPay = 8;
    public int TankPay = 8;
    public int RandomPay = 5;
    private Tile loadtile;
    private bool isRandom;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectMeleeTower();       
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectRangedTower();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectTankTower();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectRandomTower();
        }
    }
    private void Awake()
    {
        instance = this;
    }

    public void SelectMeleeTower()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;
        selectedTowerPrefab = meleeTowerPrefab;
        GameManager.instance.ShowMessage("전사 영웅을 모집합니다!");
        AudioManager.instance.PlaySFX("Select");
        Pay = MeleePay;
        isRandom = false;
    }

    public void SelectRangedTower()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;
        selectedTowerPrefab = rangedTowerPrefab;
        GameManager.instance.ShowMessage("마법사 영웅을 모집합니다!");
        AudioManager.instance.PlaySFX("Select");
        Pay = RangedPay;
        isRandom = false;
    }

    public void SelectTankTower()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;
        selectedTowerPrefab = tankTowerPrefab;
        GameManager.instance.ShowMessage("전위 영웅을 모집합니다!");
        AudioManager.instance.PlaySFX("Select");
        Pay = TankPay;
        isRandom = false;
    }

    public void SelectRandomTower()
    {
        if (CutsceneManager.instance.cutsceneflag == 1) return;
        GameObject[] TowerArr = { meleeTowerPrefab, rangedTowerPrefab, tankTowerPrefab};
        GameObject RandomPrefab = TowerArr[UnityEngine.Random.Range(0, TowerArr.Length)];
        selectedTowerPrefab = RandomPrefab;
        GameManager.instance.ShowMessage("무작위 영웅을 모집합니다!");
        AudioManager.instance.PlaySFX("Select");
        Pay = RandomPay;
        isRandom = true;
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

        loadtile = tile;
        LoadTower();
        GameManager.instance.Gold -= Pay;
        
        if (isRandom)
        {
            GameObject[] TowerArr = { meleeTowerPrefab, rangedTowerPrefab, tankTowerPrefab };
            GameObject RandomPrefab = TowerArr[UnityEngine.Random.Range(0, TowerArr.Length)];
            selectedTowerPrefab = RandomPrefab;
        }
    }

    public void LoadTower()
    {
        GameObject towerObject = Instantiate(selectedTowerPrefab, loadtile.transform.position, Quaternion.identity);
        towerObject.transform.SetParent(towerParent);
        Tower tower = towerObject.GetComponent<Tower>();

        if (tower != null)
        {
            tower.tile = loadtile; // 설치된 타일을 참조하도록 설정
            loadtile.IsBuildTower = true; // 설치된 상태로 변경
            loadtile.currentTower = tower;
        }
        string towerName = "Tower_" + Guid.NewGuid().ToString("N"); // PK
        int towerType = tower.towerindex;
        string tileName = loadtile.name;
        string areaName = loadtile.transform.parent.name;
    }
}
