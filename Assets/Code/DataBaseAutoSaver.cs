using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class AutoSaver : MonoBehaviour
{
    private DataBaseConnectingTest db;

    void Start()
    {
        //DB 연결 설정
        db = new DataBaseConnectingTest();
        db.Connect("127.0.0.1", "holiday_db", "root", "0000");
    }
    //변화된 값 넣기
    public void SaveStage(int stage)
    {
        db.Save("stage", stage);
    }

    public void SaveGold(int gold)
    {
        db.Save("gold", gold);
    }
}