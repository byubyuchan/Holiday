using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour, IPointerClickHandler
{
    private Image image;
    private Sprite idle_Image;
    public Sprite clicked_Image;

    private TowerButtonGroup buttonGroup;

    // 어두워지는 정도를 조절하는 변수 (1f = 원본, 0f = 완전 검은색)
    private Color pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    private Color normalColor = Color.white;

    private void Awake()
    {
        image = GetComponent<Image>();
        idle_Image = image.sprite;
    }

    public void SetGroup(TowerButtonGroup group)
    {
        buttonGroup = group;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buttonGroup != null)
        {
            buttonGroup.SelectButton(this);
        }
        else
        {
            ClickButton();
        }
    }

    public void ClickButton()
    {
        // 클릭 전용 이미지가 있다면 변경
        if (clicked_Image != null) image.sprite = clicked_Image;

        // 이미지를 약간 어둡게 변경
        image.color = pressedColor;
    }

    public void ReturnButton()
    {
        // 원래 이미지로 복구
        if (idle_Image != null) image.sprite = idle_Image;

        // 원래 밝기로 복구 (Color.white는 원본 이미지 색상을 그대로 보여줌)
        image.color = normalColor;
    }
}