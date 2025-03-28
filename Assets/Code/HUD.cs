using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Round, CurrentEnemy, Life, Gold, Time }
    public InfoType type;

    Text myText;
    Slider mySlider;

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    void LateUpdate()
    {
        int curLevel = GameManager.instance.level;
        switch (type)
        {
            case InfoType.Round:
                myText.text = string.Format("라운드 = {0:D0}", GameManager.instance.currentRound);
                break;
            case InfoType.CurrentEnemy:
                myText.text = string.Format("남은 몬스터 수 = {0:D0}", Spawner.instance.currentEnemyCount);
                break;
            case InfoType.Life:
                myText.text = string.Format("라이프 = {0:D0}", GameManager.instance.Life);
                break;
            case InfoType.Gold:
                myText.text = string.Format("GOLD = {0:D0}", GameManager.instance.Gold);
                break;
            case InfoType.Time:
                float Time = GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(Time / 60);
                int sec = Mathf.FloorToInt(Time % 60);
                // 자리수가 2개인 정수 표현
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
          //case 라운드 current / 6 으로 계산한 슬라이드 (보스몬스터까지 알림)
        }
    }


}
