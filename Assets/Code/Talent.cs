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
        RandomGold = Random.Range(0, 101);
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
        TowerMaker.instance.selectedTowerPrefab = null;
        meleeButton.PriceChange(5);
        GameManager.instance.ShowMessage($"전사 타워의 골드가  {TowerMaker.instance.MeleePay}이 됩니다.");
        dbConnector.saveTalent("전사의 가격이 5이됩니다.");
        OnPicked(3);
    }

    public void Talent4()
    {
        TowerMaker.instance.RangedPay = 5;
        TowerMaker.instance.selectedTowerPrefab = null;
        rangeButton.PriceChange(5);
        GameManager.instance.ShowMessage($"마법사 타워의 골드가  {TowerMaker.instance.RangedPay}이 됩니다.");
        dbConnector.saveTalent("마법사의 가격이 5이됩니다.");
        OnPicked(4);
    }

    public void Talent5()
    {
        TowerMaker.instance.TankPay = 5;
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
        TowerMaker.instance.selectedTowerPrefab = null;
        randomButton.PriceChange(RandomGold);
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
        TowerManager.instance.UpgradeAllTower(0.1f);
        TowerMaker.instance.Isupgrade = true;
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
}
