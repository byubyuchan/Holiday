using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Talent : MonoBehaviour
{
    [SerializeField] ButtonText healButton;
    [SerializeField] ButtonText meleeButton;
    [SerializeField] ButtonText rangeButton;
    [SerializeField] ButtonText tankButton;
    [SerializeField] ButtonText randomButton;

    RectTransform rect;
    Transform[] talents;

    [SerializeField]
    Transform talentGroup;

    [SerializeField]
    RectTransform layoutRoot;

    HashSet<int> picked = new HashSet<int>();

    private int RandomGold;
    [SerializeField]
    DataBaseConnectingTest dbConnector;

    public int meleePrice = 7; // 가격 조정시 수정해야함.
    public int rangePrice = 7;
    public int tankPrice = 7;
    public int randomPrice = 6;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        if (talentGroup == null)
            talentGroup = transform.Find("Panel/Talent Group");

        int childCount = talentGroup.childCount;
        talents = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
            talents[i] = talentGroup.GetChild(i);
        Show();
    }

    public void Show()
    {
        Select();
        rect.localScale = Vector3.one;
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
    }

    void Select()
    {
        if (talents == null || talents.Length < 2) return;

        foreach (var t in talents) t.gameObject.SetActive(false);

        List<int> candidates = new List<int>();
        for (int i = 1; i < talents.Length; i++)
            if (!picked.Contains(i)) candidates.Add(i);

        for (int i = 0; i < candidates.Count; i++)
        {
            int j = Random.Range(i, candidates.Count);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        int toPick = Mathf.Min(3, candidates.Count);
        for (int k = 0; k < toPick; k++)
            talents[candidates[k]].gameObject.SetActive(true);

        var t0 = talents[0];
        t0.gameObject.SetActive(true);
        t0.SetSiblingIndex(t0.parent.childCount - 1);

        var root = layoutRoot != null ? layoutRoot : (RectTransform)talentGroup;
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }

    void OnPicked(int idx)
    {
        if (idx > 0) picked.Add(idx);
    }

    public void Talent1()
    {
        RandomGold = Random.Range(0, 151);
        GameManager.instance.Gold += RandomGold;
        GameManager.instance.ShowMessage($"{RandomGold}만큼 골드를 획득했습니다!");
        dbConnector.saveTalent($"Talent1_{RandomGold}만큼 골드 획득");
        OnPicked(1);
    }

    public void Talent2()
    {
        TowerManager.instance.sale = true;
        healButton.PriceChange(3);
        GameManager.instance.ShowMessage("회복 골드가 3이 됩니다.");
        dbConnector.saveTalent("회복 골드가 3골드가 됩니다.");
        OnPicked(2);
    }

    public void Talent3()
    {
        TowerMaker.instance.MeleePay = 5;
        meleePrice = 5;
        TowerMaker.instance.selectedTowerPrefab = null;
        meleeButton.PriceChange(5);
        GameManager.instance.ShowMessage($"전사 타워의 골드가  {TowerMaker.instance.MeleePay}이 됩니다.");
        dbConnector.saveTalent("전사의 가격이 5이됩니다.");
        OnPicked(3);
    }

    public void Talent4()
    {
        TowerMaker.instance.RangedPay = 5;
        rangePrice = 5;
        TowerMaker.instance.selectedTowerPrefab = null;
        rangeButton.PriceChange(5);
        GameManager.instance.ShowMessage($"마법사 타워의 골드가  {TowerMaker.instance.RangedPay}이 됩니다.");
        dbConnector.saveTalent("마법사의 가격이 5이됩니다.");
        OnPicked(4);
    }

    public void Talent5()
    {
        TowerMaker.instance.TankPay = 5;
        meleePrice = 5;
        TowerMaker.instance.selectedTowerPrefab = null;
        tankButton.PriceChange(5);
        GameManager.instance.ShowMessage($"전위 타워의 골드가  {TowerMaker.instance.TankPay}이 됩니다.");
        dbConnector.saveTalent("전위 가격이 5이 됩니다.");
        OnPicked(5);
    }

    public void Talent6()
    {
        RandomGold = Random.Range(3, 9);
        TowerMaker.instance.RandomPay = RandomGold;
        randomPrice = RandomGold;
        TowerMaker.instance.selectedTowerPrefab = null;
        randomButton.PriceChange(RandomGold);
        if(TowerManager.instance.buttonRandom)
        {
            meleeButton.PriceChange(randomPrice);
            rangeButton.PriceChange(randomPrice);
            tankButton.PriceChange(randomPrice);
            randomButton.PriceChange(randomPrice);
        }
        GameManager.instance.ShowMessage($"랜덤 타워의 골드가  {RandomGold}G로 변경되었습니다!");
        dbConnector.saveTalent($"'랜덤의 랜덤_랜덤' 타워의 가격이 {RandomGold}G로 변경되었습니다!");
        OnPicked(6);
    }

    public void Talent7()
    {

        TowerMaker.instance.up_B = true;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"B급 타워의 확률이 증가했습니다!");
        dbConnector.saveTalent($"B급 타워의 확률이 증가했습니다!");
        OnPicked(7);
    }

    public void Talent8()
    {
        TowerMaker.instance.up_A = true;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"A급 타워의 확률이 증가했습니다!");
        dbConnector.saveTalent($"A급 타워의 확률이 증가했습니다!");
        OnPicked(8);
    }

    public void Talent9()
    {
        TowerMaker.instance.only_C = true;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.Gold += 100;
        GameManager.instance.ShowMessage($"더 이상 높은 등급의 용사가 등장하지 않습니다!");
        dbConnector.saveTalent($"더 이상 높은 등급의 용사가 등장하지 않습니다!");
        OnPicked(9);
    }

    public void Talent10()
    {
        TowerManager.instance.DestroyAllTower();
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.Gold += 100;
        GameManager.instance.ShowMessage($"임무를 재시작합니다!");
        dbConnector.saveTalent($"임무를 재시작합니다!");
        OnPicked(10);
    }

    public void Talent11()
    {
        TowerMaker.instance.upgradeVal += 0.1f;
        TowerManager.instance.UpgradeAllTower(TowerMaker.instance.upgradeVal);
        //TowerMaker.instance.Isupgrade = true;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"모든 영웅의 능력치가 10% 향상됩니다!");
        dbConnector.saveTalent($"모든 영웅의 능력치가 10% 향상됩니다!");
        OnPicked(11);
    }
    public void Talent12()
    {
        TowerManager.instance.reverse = true;
        TowerMaker.instance.selectedTowerPrefab = null;
        healButton.PriceChange(TowerManager.instance.sale? 3:5);
        GameManager.instance.ShowMessage($"회복 마법이 파괴 마법으로 변경됩니다!");
        dbConnector.saveTalent($"회복 마법이 파괴 마법으로 변경됩니다!!");
        OnPicked(12);
    }

    public void Talent13()
    {
        TowerMaker.instance.upgradeVal += 0.2f;
        TowerManager.instance.UpgradeAllTower(TowerMaker.instance.upgradeVal);
        Spawner.instance.bossHpUp = true;
        //TowerMaker.instance.Isupgrade = true;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"능력치가 증가하고 보스몬스터의 체력이 증가합니다!");
        dbConnector.saveTalent($"능력치가 증가하고 보스몬스터의 체력이 증가합니다!");
        OnPicked(13);
    }

    public void Talent14()
    {
        TowerManager.instance.ChangeAttackFormToWarrior();
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"전사들의 공격이 광역으로 변경됩니다!");
        dbConnector.saveTalent($"전사들의 공격이 광역으로 변경됩니다!");
        OnPicked(14);
    }

    public void Talent15()
    {
        TowerMaker.instance.upgradeVal += 0.2f;
        TowerManager.instance.UpgradeAllTower(TowerMaker.instance.upgradeVal);
        TowerMaker.instance.cantMove = true;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"능력치가 증가하고 더 이상 배치를 변경할 수 없습니다!");
        dbConnector.saveTalent($"능력치가 증가하고 더 이상 배치를 변경할 수 없습니다!");
        OnPicked(15);
    }

    public void Talent16()
    {
        TowerManager.instance.ActivateBurningTalent();
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"전투 중 광전사 모드가 발동됩니다!");
        dbConnector.saveTalent($"전투 중 광전사 모드가 발동됩니다!");
        OnPicked(16);
    }

    public void Talent17()
    {
        TowerManager.instance.buttonRandom = true;
        RandomGold = Random.Range(50, 101);
        GameManager.instance.Gold += RandomGold;
        TowerMaker.instance.selectedTowerPrefab = null;
        meleeButton.PriceChange(randomPrice);
        rangeButton.PriceChange(randomPrice);
        tankButton.PriceChange(randomPrice);
        randomButton.PriceChange(randomPrice);
        GameManager.instance.ShowMessage($"어떤 클래스를 선택하더라도 랜덤한 영웅이 소환됩니다! {RandomGold}만큼 골드를 획득했습니다!");
        dbConnector.saveTalent($"어떤 클래스를 선택하더라도 랜덤한 영웅이 소환됩니다! {RandomGold}만큼 골드를 획득했습니다!");
        OnPicked(17);
    }

    public void Talent18()
    {
        TowerManager.instance.cantSell= true;
        TowerManager.instance.ActivateHealingTalent();
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"더 이상 타워 판매가 불가능합니다! 5초에 1번씩 체력을 50씩 회복시킵니다.");
        dbConnector.saveTalent($"더 이상 타워 판매가 불가능합니다! 5초에 1번씩 체력을 50씩 회복시킵니다.");
        OnPicked(18);
    }

    public void Talent19()
    {
        TowerManager.instance.bigProjectile = true;
        TowerManager.instance.UpgradeAllTower(TowerMaker.instance.upgradeVal);
        //TowerMaker.instance.Isupgrade = true;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"투사체의 크기가 2배, 데미지가 2배 그리고 공격속도도 2배로 증가합니다.");
        dbConnector.saveTalent($"투사체의 크기가 2배, 데미지가 2배 그리고 공격속도도 2배로 증가합니다.");
        OnPicked(19);
    }

    public void Talent20()
    {
        TowerManager.instance.smallProjectile = true;
        TowerManager.instance.UpgradeAllTower(TowerMaker.instance.upgradeVal);
        //TowerMaker.instance.Isupgrade = true;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"투사체의 크기가 2배, 데미지가 2배 그리고 공격속도도 2배로 낮아집니다.");
        dbConnector.saveTalent($"투사체의 크기가 2배, 데미지가 2배 그리고 공격속도도 2배로 낮아집니다.");
        OnPicked(20);
    }

    public void Talent21()
    {
        TowerManager.instance.IsAllC = true;
        TowerManager.instance.UpgradeAllTower(TowerMaker.instance.upgradeVal);
        //TowerMaker.instance.Isupgrade = true;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"C급 영웅들만 있다면 능력치가 50% 상승합니다!");
        dbConnector.saveTalent($"C급 영웅들만 있다면 능력치가 50% 상승합니다!");
        OnPicked(21);
    }

    public void Talent22()
    {
        TowerManager.instance.reSell = true;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"현재 타워들의 판매 골드가 2배로 증가합니다. 살아남을 때마다 2배로 증가합니다!");
        dbConnector.saveTalent($"현재 타워들의 판매 골드가 2배로 증가합니다. 살아남을 때마다 2배로 증가합니다!");
        OnPicked(22);
    }
    public void Talent23()
    {
        TowerManager.instance.forcedSale = true;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"현재 타워들의 판매 골드가 2배로 증가합니다. 살아남을 때마다 2배로 증가합니다!");
        dbConnector.saveTalent($"현재 타워들의 판매 골드가 2배로 증가합니다. 살아남을 때마다 2배로 증가합니다!");
        OnPicked(23);
    }




    //public void Talent13()
    //{
    //    List<int> candidates = new List<int>();
    //    for (int i = 1; i < talents.Length; i++)
    //        if (!picked.Contains(i) && i != 13) // 자기 자신은 제외
    //            candidates.Add(i);

    //    // 랜덤 섞기
    //    for (int i = 0; i < candidates.Count; i++)
    //    {
    //        int j = Random.Range(i, candidates.Count);
    //        (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
    //    }

    //    // 2개 지급
    //    for (int k = 0; k < 2; k++)
    //    {
    //        int idx = candidates[k];
    //        switch (idx)
    //        {
    //            case 1: Talent1(); break;
    //            case 2: Talent2(); break;
    //            case 3: Talent3(); break;
    //            case 4: Talent4(); break;
    //            case 5: Talent5(); break;
    //            case 6: Talent6(); break;
    //            case 7: Talent7(); break;
    //            case 8: Talent8(); break;
    //            case 9: Talent9(); break;
    //            case 10: Talent10(); break;
    //            case 11: Talent11(); break;
    //            case 12: Talent12(); break;
    //        }
    //    }

    //    GameManager.instance.ShowMessage("랜덤한 특성 2개를 획득했습니다!");
    //    dbConnector.saveTalent("Talent13: 랜덤 특성 2개 자동 지급");
    //    OnPicked(13);
    //}
}
