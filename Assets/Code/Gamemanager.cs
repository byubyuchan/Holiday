using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
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
    public GameObject nameInput;
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
    public int currentRound; // ���� ����

    int speedIndex = 0;
    public float[] gameSpeed = { 1f, 1.5f, 2f, 2.5f, 3f };

    [Header("# Player Info")]
    public int playerId;
    public string playerName;
    public float hp;
    public float maxHp;
    public int level;
    public int kill;
    public Enemy bossEnemy;
    public Light2D globalLight;
    public SpriteRenderer Background;

    private void Awake()
    {
        instance = this;
        AudioManager.instance.PlayBGM(true);
        PauseButton.gameObject.SetActive(true);
        StartRoundButton.gameObject.SetActive(true);
        Gold = 50;
        currentRound = 0;
        isLive = true;
        if (globalLight == null)
        {
            globalLight = GetComponent<Light2D>();
        }
        playerId = Random.Range(1,400000000);
        dbConnector.defaultSetting(playerId);

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
        if (Spawner.instance.level == 4)
        {
            Spawner.instance.spriteRenderer.enabled = false;
        }
        currentRound++;
        Spawner.instance.level = currentRound - 1;
        Spawner.instance.currentEnemyCount = 0;
        Spawner.instance.enemyCount = 0;
        Spawner.instance.SpawnCount = 0;
        Spawner.instance.isEnd = false;
        StartRoundButton.gameObject.SetActive(false); // ��ư ��Ȱ��ȭ
        isStart = true;

    }

    public void EndRound()
    {
        if (Spawner.instance.level == 4)
        {
            Spawner.instance.anim.runtimeAnimatorController = Spawner.instance.animCon;
            Spawner.instance.transform.localScale = new Vector3(4, 4, 4);
            ImageChanger.instance.ChangeMonsterImage();
        }
        else
        {
            // 라운드에 따라 컬러 변화
            // Color는 0~1 범위로 설정하므로 255기준을 0~1로 변환
            float originRed = globalLight.color.r;
            float originGreen = globalLight.color.g;
            float originBlue = globalLight.color.b;
            float r = 255f / 255f / 4f;
            float g = 75f / 255f / 4f;
            float b = 75f / 255f / 4f;
            globalLight.color = new Color(originRed+r, originGreen+g, originBlue+b);
            globalLight.intensity -= 0.15f;

            Color targetBGColor = new Color(100f / 255f, 0f, 0f);
            Background.color = Color.Lerp(Background.color, targetBGColor, 0.33f);
        }
        dbConnector.saveValue(playerId, "time", (int)gameTime);
        dbConnector.saveValue(playerId, "stage", currentRound);
        Debug.Log("Time save!");

        if (isLive)
        {
            isStart = false;
            StartRoundButton.gameObject.SetActive(true); // ��ư Ȱ��ȭ
            Gold += 50;
            AudioManager.instance.PlaySFX("Win");
        }
    }

    public void GameStart(int id)
    {
        playerId = id;
        PauseButton.gameObject.SetActive(true);
        StartRoundButton.gameObject.SetActive(true);
        isLive = true;
        Gold = 50;
        currentRound = 0;
    }

    public void GameOver()
    {
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
        //dbConnector.loadValue(playerId);//플레이어의 clear 순위를 가져옴(int로)
        dbConnector.clearCnt();//플레이어의 클리어 순서를 저장하고, 전체 클리어 횟수를 하나 올림
    }

    IEnumerator GameWinRoutine()
    {
        isLive = false;
        //EnemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        //UIResult.gameObject.SetActive(true);
        //UIResult.Win();

        Stop();

        //AudioManager.instance.PlaySFX(AudioManager.SFX.Win);
    }

    //public void GameRetry()
    //{
    //    SceneManager.LoadScene(0);
    //    Gold = dbConnector.LoadValue("gold");
    //    kill = dbConnector.LoadValue("kill");
    //    currentRound = dbConnector.LoadValue("stage");
    //    hp = dbConnector.LoadValue("hp");
    //}

    public void GameQuit()
    {
        Application.Quit();
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
        //UIJoy.localScale = Vector3.zero;
        StartRoundButton.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);
    }

    public void Resume()
    {
        isLive = true;
        // �ð� �ӵ� (����)
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
        // �ð� �ӵ� (����)
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
        // �̹� ���� ���� �޽��� �ڷ�ƾ�� �ִٸ� ����
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }
        messageCoroutine = StartCoroutine(ShowMessageCoroutine(message, time));
    }

    public void ShowRetryButton(float delay, bool win)
    {
        StartCoroutine(ShowButtonAfterDelay(delay, win));
    }

    public IEnumerator ShowMessageCoroutine(string message, float time)
    {
        NoticeText.text = message;
        NoticeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        NoticeText.gameObject.SetActive(false);
        messageCoroutine = null;
    }

    private IEnumerator ShowButtonAfterDelay(float delay,bool win)
    {
        yield return new WaitForSeconds(delay);
        nameInput.SetActive(true);
        if (win)
        {
            AudioManager.instance.PlaySFX("Victory");
            GameWin();
        }
        else
        {
            AudioManager.instance.PlaySFX("Fail");
            GameOver();
        }

    }

    public void GoToIntroScene()
    {
        Destroy(instance.gameObject);
        SceneManager.LoadScene("IntroScene"); // 인트로 씬 이름
    }

    [Header("# DataBase")]
    public UnityEvent<int> OnGoldChanged;
    public DataBaseConnectingTest dbConnector;

    private int _gold;
}