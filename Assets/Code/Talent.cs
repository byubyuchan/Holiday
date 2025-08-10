using UnityEngine;

public class Talent : MonoBehaviour
{
    RectTransform rect;
    Transform[] talents;
    private int RandomGold;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        Transform talentGroup = transform.Find("Panel/TalentGroup");
        if (talentGroup != null)
        {
            talents = new Transform[talentGroup.childCount];
            for (int i = 0; i < talentGroup.childCount; i++)
            {
                talents[i] = talentGroup.GetChild(i);
            }
        }
    }

    public void Show()
    {
        rect.localScale = Vector3.one;
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
    }

    public void Talent1()
    {
        RandomGold = Random.Range(0, 101);
        GameManager.instance.Gold += RandomGold;
        GameManager.instance.ShowMessage($"{RandomGold}만큼 골드를 획득했습니다!");
    }

    public void Talent2()
    {
        TowerManager.instance.sale = true;
    }

    public void Talent3()
    {
        TowerMaker.instance.RandomPay = 4;
        TowerMaker.instance.selectedTowerPrefab = null;
    }

    public void Talent4()
    {
        TowerMaker.instance.MeleePay = 6;
        TowerMaker.instance.selectedTowerPrefab = null;
    }

    public void Talent5()
    {
        TowerMaker.instance.RangedPay = 6;
        TowerMaker.instance.selectedTowerPrefab = null;
    }

    public void Talent6()
    {
        TowerMaker.instance.TankPay = 6;
        TowerMaker.instance.selectedTowerPrefab = null;
    }

    public void Talent7()
    {
        RandomGold = Random.Range(2, 11);
        TowerMaker.instance.RandomPay = RandomGold;
        TowerMaker.instance.selectedTowerPrefab = null;
        GameManager.instance.ShowMessage($"랜덤 타워의 가격이 {RandomGold}G로 변경되었습니다!");
    }
}
