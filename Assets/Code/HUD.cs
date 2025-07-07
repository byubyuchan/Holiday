using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Round, CurrentEnemy, Life, Gold, Time, ProgressSlider, ProgressText }
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
        float killCount = GameManager.instance.kill;
        float maxEnemyCount = -1;
        foreach (int count in Spawner.instance.maxEnemies)
            maxEnemyCount += count;

        float progress = maxEnemyCount > 0 ? killCount / maxEnemyCount : 0f;

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
            case InfoType.ProgressSlider:
                mySlider.value = progress;
                break;
            case InfoType.ProgressText:
                int progressPercent = Mathf.FloorToInt(progress * 100);
                myText.text = string.Format("Progress {0:D0}%", progressPercent);
                break;
        }
    }
}
