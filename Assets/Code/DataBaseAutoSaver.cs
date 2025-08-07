using UnityEngine;


public class AutoSaver : MonoBehaviour
{
    public GameManager gameManager;
    public DataBaseConnectingTest dbConnectingT;
    
    void Start()
    {
        // gameManager 인스턴스가 없으면 찾기 없으면 오류가 뜸
        if (gameManager == null)
        {
            gameManager = GameManager.instance;
        }
        Debug.Log("자동저장을 시작함");
        gameManager.OnGoldChanged.AddListener(SaveGold);
        gameManager.OnStageChanged.AddListener(SaveStage);
        gameManager.OnKillChanged.AddListener(SaveKill);
    }
    private void OnDestroy()
    {
        // 게임 오브젝트가 파괴될 때 이벤트 리스너 제거
        if (gameManager != null)
        {
            gameManager.OnGoldChanged.RemoveListener(SaveGold);
            gameManager.OnStageChanged.RemoveListener(SaveStage);
            gameManager.OnKillChanged.RemoveListener(SaveKill);
        }
    }
    private void SaveKill(int kill)
    {
        dbConnectingT?.saveValue("kill", kill); 
    }

    //변화된 값 넣기
    private void SaveStage(int stage)
    {
        dbConnectingT?.saveValue("stage", stage);
    }
    private void SaveGold(int gold)
    {
        dbConnectingT?.saveValue("gold", gold);
    }
}