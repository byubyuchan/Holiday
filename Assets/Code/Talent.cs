using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Talent : MonoBehaviour
{
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
        dbConnector.saveTalent("선택하지 않습니다.");
        OnPicked(2);
    }

    public void Talent3()
    {
        TowerMaker.instance.RandomPay = 4;
        TowerMaker.instance.selectedTowerPrefab = null;
        dbConnector.saveTalent("랜덤가격이 4가됩니다.");
        OnPicked(3);
    }

    public void Talent4()
    {
        TowerMaker.instance.MeleePay = 6;
        TowerMaker.instance.selectedTowerPrefab = null;
        dbConnector.saveTalent("전사의 가격이 6이됩니다.");
        OnPicked(4);
    }

    public void Talent5()
    {
        TowerMaker.instance.RangedPay = 6;
        TowerMaker.instance.selectedTowerPrefab = null;
        dbConnector.saveTalent("마법사의 가격이 6이됩니다.");
        OnPicked(5);
    }

    public void Talent6()
    {
        TowerMaker.instance.TankPay = 6;
        TowerMaker.instance.selectedTowerPrefab = null;
        dbConnector.saveTalent("전위 가격이 6이 됩니다.");
        OnPicked(6);
    }

    public void Talent7()
    {
        RandomGold = Random.Range(2, 11);
        TowerMaker.instance.RandomPay = RandomGold;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"랜덤 타워의 가격이 {RandomGold}G로 변경되었습니다!");
        dbConnector.saveTalent($"'랜덤의 랜덤_랜덤' 타워의 가격이 {RandomGold}G로 변경되었습니다!");
        OnPicked(7);
    }
}
