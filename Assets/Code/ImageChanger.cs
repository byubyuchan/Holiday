using UnityEngine;
using UnityEngine.UI;

public class ImageChanger : MonoBehaviour
{
    static public ImageChanger instance;
    private Image Image;
    public Sprite BossImage;

    private void Awake()
    {
        instance = this;
        Image = GetComponent<Image>();
    }

    public void ChangeMonsterImage()
    {
        Image.sprite = BossImage;
    }
}
