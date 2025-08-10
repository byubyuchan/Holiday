using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private Image Image;
    private Sprite Idle_Image;
    public Sprite Clicked_Image;
    private void Awake()
    {
        Image = GetComponent<Image>();
        Idle_Image = Image.sprite;
    }

    public void ClickButton()
    {
        Image.sprite = Clicked_Image;
    }

    public void ReturnButton()
    {
        Image.sprite = Idle_Image;
    }
}
