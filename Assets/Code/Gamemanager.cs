using System.Collections;
using System.Threading;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("# Game Object")]
    public static GameManager instance;
    public GameObject EnemyCleaner;
    public Transform UIJoy;
    public Transform UIPause;
    public Button PauseButton;

    [Header("# Game Control")]
    public float gameTime;
    public float maxGameTime = 5 * 60f;
    public bool isLive;
    int speedIndex = 0;
    public float[] gameSpeed = { 1f, 1.5f, 2f, 2.5f, 3f };

    [Header("# Player Info")]
    public int playerId;
    public float hp;
    public float maxHp;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 10, 30, 50, 70, 100, 150, 200, 250, 300 };

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 120;
        //PauseButton.gameObject.SetActive(false);
    }
    public void GameStart(int id)
    {
        playerId = id;
        hp = maxHp;
        isLive = true;
        Resume();

        //PauseButton.gameObject.SetActive(true);
        //AudioManager.instance.PlayBGM(true);
        //AudioManager.instance.PlaySFX(AudioManager.SFX.Select);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        //UIResult.gameObject.SetActive(true);
        //UIResult.Lose();

        Stop();

        //AudioManager.instance.PlayBGM(false);
        //AudioManager.instance.PlaySFX(AudioManager.SFX.Lose);

    }

    public void GameWin()
    {
        StartCoroutine(GameWinRoutine());
    }

    IEnumerator GameWinRoutine()
    {
        isLive = false;
        EnemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        //UIResult.gameObject.SetActive(true);
        //UIResult.Win();

        Stop();

        //AudioManager.instance.PlaySFX(AudioManager.SFX.Win);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
        Application.Quit();
    }
    void Update()
    {
        if (!isLive) return;
        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameWin();
        }
    }

    public void GetExp(int expVal)
    {
        if (!isLive) return;
        exp += expVal;

        if (exp >= nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp -= nextExp[Mathf.Min(level - 1, nextExp.Length - 1)];
            //UILevelUp.show();
        }
    }

    public void Stop()
    {
        isLive = false;
        // 시간 속도 (배율)
        Time.timeScale = 0;
        UIJoy.localScale = Vector3.zero;
        PauseButton.gameObject.SetActive(false);
    }

    public void Resume()
    {
        isLive = true;
        // 시간 속도 (배율)
        Time.timeScale = gameSpeed[speedIndex];
        UIJoy.localScale = Vector3.one;
        PauseButton.gameObject.SetActive(true);
    }

    public void Pause()
    {
        isLive = false;
        Time.timeScale = 0;
        UIPause.localScale = Vector3.one;
    }

    public void Pause_Resume()
    {
        isLive = true;
        // 시간 속도 (배율)
        Time.timeScale = gameSpeed[speedIndex];
        UIPause.localScale = Vector3.zero;
    }

    public void ChangeSpeed()
    {
        speedIndex = (speedIndex + 1) % gameSpeed.Length;
        //GameSpeed.instance.speed = gameSpeed[speedIndex];
        Time.timeScale = gameSpeed[speedIndex];

    }
}
