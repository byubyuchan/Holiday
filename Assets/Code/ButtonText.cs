using UnityEngine;
using UnityEngine.UI;
using static HUD;

public class ButtonText : MonoBehaviour
{
    public enum InfoType { Heal, Melee, Range, Tank, Random }
    public InfoType type;   // 버튼의 종류 (Inspector에서 지정)
    public Text BText;      // 이 버튼에 표시될 Text

    public void PriceChange(int price)
    {
        // 타입에 맞는 가격표시 갱신
        switch (type)
        {
            case InfoType.Heal:
                if (TowerManager.instance.reverse) BText.text = $"파괴({price}G)";
                else BText.text = $"회복({price}G)";
                break;
            case InfoType.Melee:
                BText.text = $"전사({price}G)";
                break;
            case InfoType.Range:
                BText.text = $"마법사({price}G)";
                break;
            case InfoType.Tank:
                BText.text = $"전위({price}G)";
                break;
            case InfoType.Random:
                BText.text = $"랜덤({price}G)";
                break;
        }
    }
}
