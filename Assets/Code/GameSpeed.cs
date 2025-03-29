using UnityEngine;
using UnityEngine.UI;

public class GameSpeed : MonoBehaviour
{
    public static GameSpeed instance;
    Text speedText;
    public float speed = 1.0f;

    void Awake()
    {
        instance = this;
        speedText = GetComponentInChildren<Text>();
    }

    void Update()
    {
        speedText.text = "게임속도x" + speed.ToString("F1");
    }

}
