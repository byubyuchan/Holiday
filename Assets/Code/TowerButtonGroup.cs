using UnityEngine;

public class TowerButtonGroup : MonoBehaviour
{
    [SerializeField] private ButtonManager[] towerButtons;

    private void Awake()
    {
        // 게임 시작 시, 배열에 등록된 4개의 버튼에게 이 그룹 스크립트를 연결해줍니다.
        foreach (ButtonManager btn in towerButtons)
        {
            if (btn != null) btn.SetGroup(this);
        }
    }

    // 특정 버튼이 눌렸을 때 호출되는 함수
    public void SelectButton(ButtonManager selectedButton)
    {
        if (TowerManager.instance.buttonRandom)
        {
            selectedButton = TowerMaker.instance.btnRandom;
        }

        foreach (ButtonManager btn in towerButtons)
        {
            if (btn == selectedButton)
            {
                btn.ClickButton(); // 눌린 버튼은 활성화 이미지로
            }
            else
            {
                btn.ReturnButton(); // 나머지는 기본 이미지로 복구
            }
        }
    }
    
    public void ResetButtons()
    {
        foreach (ButtonManager btn in towerButtons)
        {
            btn.ReturnButton();
        }
    }
}