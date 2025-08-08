using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


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
    //{
    //    get => _kill;
    //    set
    //    {
    //         if (_kill != value)
    //        {
    //            _kill = value;
    //            OnKillChanged?.Invoke(_kill);
    //        }
    //    }
    //}

    public Enemy bossEnemy;

    private void Awake() // 씬이 변경될 때, 인트로 씬 -> 메인 씬으로 갈 때 생성되기에 불러오기가 불가능함.
    {
        instance = this;
        AudioManager.instance.PlayBGM(true);
        PauseButton.gameObject.SetActive(true);
        StartRoundButton.gameObject.SetActive(true);
        Gold = 50;
        currentRound = 0;
        isLive = true;
    }

    void Update()
    {
        if (!isLive) return;
        gameTime += Time.deltaTime;
        if(gameTime > 0 && gameTime < 1) dbConnector.defaultSetting();
    }

    public void StartRound()
    {
        if (isStart) return;
        if (CutsceneManager.instance.cutsceneflag == 1) return;
        if (Spawner.instance.level == 4)
        {
            Spawner.instance.spriteRenderer.enabled = false;
        }
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
        if (Spawner.instance.level == 4)
        {
            Spawner.instance.anim.runtimeAnimatorController = Spawner.instance.animCon;
            Spawner.instance.transform.localScale = new Vector3(4, 4, 4);
        }
        dbConnector.saveValue("time", (int)gameTime);
        Debug.Log("Time save!");
    }

    public void GameStart(int id)
    {
        playerId = id;
        PauseButton.gameObject.SetActive(true);
        StartRoundButton.gameObject.SetActive(true);
        isLive = true;
        Gold = 50;
        currentRound = 0;
        dbConnector.defaultSetting();
    }

    public void GameOver()
    {
        dbConnector.saveValue("time", (int)gameTime);
        dbConnector.saveValue("kill", kill);
        StartCoroutine(GameOverRoutine());
        dbConnector.Close();
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
        Gold = dbConnector.LoadValue("gold");
        kill = dbConnector.LoadValue("kill");
        currentRound = dbConnector.LoadValue("stage");
        hp = dbConnector.LoadValue("hp");
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
    public UnityEvent<int> OnKillChanged;
    public DataBaseConnectingTest dbConnector;

    private int _stage;
    private int _gold;
    private int _kill;
}