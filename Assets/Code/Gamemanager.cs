using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//database using

//using UnityEngine;
//using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private Coroutine messageCoroutine;

    [Header("# Game Object")]
    public GameObject goal;
    public PoolManager pool;
    public static GameManager instance;
    public GameObject EnemyCleaner;
    public Transform UIJoy;
    public Transform UIPause;
    public Button PauseButton;
    public Button StartRoundButton;
    public Text NoticeText;

    [Header("# Game Control")]
    public float gameTime;
    public bool isLive;
    public bool isStart;
    public int Life;
    public int Gold
    {
        get => _gold;
        set
        {
            if (_gold != value)
            {
                _gold = value;
                OnGoldChanged?.Invoke(_gold);
            }
        }
    }
    public int currentRound // 현재 라운드
    {
        get => _stage;
        set
        {
            if (_stage != value)
            {
                _stage = value;
                OnStageChanged?.Invoke(_stage);
            }
        }
    } 

    int speedIndex = 0;
    public float[] gameSpeed = { 1f, 1.5f, 2f, 2.5f, 3f };

    [Header("# Player Info")]
    public int playerId;
    public float hp;
    public float maxHp;
    public int level;
    public int kill;

    public Enemy bossEnemy;

    private void Awake()
    {
        instance = this;
        PauseButton.gameObject.SetActive(true);
        StartRoundButton.gameObject.SetActive(true);
        isLive = true;
        Gold = 50;
        currentRound = 0;
        AudioManager.instance.PlayBGM(true);
    }

    void Update()
    {
        if (!isLive) return;
        gameTime += Time.deltaTime;
    }

    public void StartRound()
    {
        if (isStart) return;
        if (CutsceneManager.instance.cutsceneflag == 1) return;

        currentRound++;
        Spawner.instance.level = currentRound - 1;
        StartRoundButton.gameObject.SetActive(false); // 버튼 비활성화
        isStart = true;
    }

    public void EndRound()
    {
        isStart = false;
        StartRoundButton.gameObject.SetActive(true); // 버튼 활성화
        Gold += 50;
        dbConnector.saveTime((int)gameTime);
        Debug.Log("Time save!");
    }

    //public void GameStart(int id)
    //{
    //    playerId = id;
    //    hp = maxHp;
    //    isLive = true;
    //    Resume();
    //    PauseButton.gameObject.SetActive(true);
    //    //AudioManager.instance.PlayBGM(true);
    //    //AudioManager.instance.PlaySFX(AudioManager.SFX.Select);
    //}

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
        GameSpeed.instance.speed = gameSpeed[speedIndex];
        
    }
    public void ShowMessage(string message, float time = 2)
    {
        // 이미 실행 중인 메시지 코루틴이 있다면 중지
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }
        messageCoroutine = StartCoroutine(ShowMessageCoroutine(message, time));
    }

    public IEnumerator ShowMessageCoroutine(string message, float time)
    {
        NoticeText.text = message;
        NoticeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        NoticeText.gameObject.SetActive(false);
        messageCoroutine = null;
    }

    [Header("# DataBase")]
    public UnityEvent<int> OnStageChanged;
    public UnityEvent<int> OnGoldChanged;
    public DataBaseConnectingTest dbConnector;

    private int _stage;
    private int _gold;
}