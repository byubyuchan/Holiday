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
        GameManager.instance.ShowMessage($"{RandomGold}¸¸Å­ °ñµå¸¦ È¹µæÇß½À´Ï´Ù!");
    }
}
