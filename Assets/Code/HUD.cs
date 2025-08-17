using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    static public HUD instance;
    public enum InfoType { Round, CurrentEnemy, Life, Gold, Time, ProgressSlider, ProgressText}
    public InfoType type;

    Text myText;
    Slider mySlider;

    void Awake()
    {
        instance = this;
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
                if (Spawner.instance.level != 5)
                    mySlider.value = progress;
                else {
                    Enemy boss = GameManager.instance.bossEnemy;
                    mySlider.maxValue = boss.maxHp;
                    mySlider.value = boss.hp;
                    mySlider.fillRect.GetComponent<Image>().color = Color.red;
                }
                break;
            case InfoType.ProgressText:
                if (Spawner.instance.level != 5) {
                    int progressPercent = Mathf.FloorToInt(progress * 100);
                    myText.text = string.Format("Progress {0:D0}%", progressPercent);
                }
                else {
                    Enemy bossHP = GameManager.instance.bossEnemy;
                    float hp = Mathf.Max(0, bossHP.hp);
                    // 보스 HP 비율로 표시
                    //myText.text = string.Format("Boss HP {0:D0}%", Mathf.FloorToInt((hp / bossHP.maxHp) * 100));
                    // 보스 HP 숫자로 표시
                    myText.text = string.Format("Boss HP {0:D0}/{1:D0}", Mathf.FloorToInt(hp), Mathf.FloorToInt(bossHP.maxHp));
                }
                break;
        }
    }
}
