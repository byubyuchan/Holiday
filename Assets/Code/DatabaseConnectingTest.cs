using System;
using MySql.Data.MySqlClient;
using UnityEngine;

public class DataBaseConnectingTest : MonoBehaviour
{
    //DB 연결을 위한 생성자
    private string connectionString;
    private MySqlConnection connection;

    private void Awake()
    {
        //databse 연결 문자열 설정
        connectionString = "Server=";
        String sever = "175.197.161.15";
        String port = "31242";
        String database = "holiday_db";
        String user = "root";
        String password = "thth1234*";
        connectionString = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};",
            sever, port, database, user, password);
        connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();
            Debug.Log("DB connecting Success!");
            saveGold(50);
            saveStage(0);
            saveTime(0);
            Debug.Log("DB Setting Success!");
            Close();
        }
        catch (Exception ex)
        {
            Debug.LogError("DB Connecting Fail ! : " + ex.Message);
        }
    }

    //DB 퀴리 실행을 위한 함수1
    public void Execute(string name, int value)
    {
        Debug.Log("excute 시작");
        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE information SET value = @value WHERE name = @name";
                Debug.Log($"Saving to DB → {name} = {value}");

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@value", value);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.ExecuteNonQuery();
                }
                Debug.Log($"Saved '{name}' = {value} to DB");

                if(name == "stage")
                {
                    compareStage(value,LoadMaxStage());
                }

                
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error to saving: " + ex.Message);
        }
    }

    //DB 퀴리 실행을 위한 함수2
    public void saveGold(int gold) => Execute("gold", gold);
    public void saveStage(int stage) => Execute("stage", stage);
    public void saveTime(int gameTime) => Execute("time", gameTime);
    public void saveMaxStage(int mStage) => Execute("maxStage", mStage);

    //닫는 함수
    public void Close()
    {
        Debug.Log("닫음");
        if (connection != null)
        {
            connection.Close();
            Debug.Log("DB connection end");
        }
    }

    //DB에서 maxStage 값을 비교하는 함수
    public void compareStage(int currentstage, int savedstage)
    {
        if (currentstage > savedstage)
        {
            Debug.Log("New Record is updated!!");
            saveMaxStage(currentstage);

        }
        else
        {
            Debug.Log("Stage is not updated");
        }
    }

    //DB에서 maxStage 값을 불러오는 함수
    private int LoadMaxStage()
    {
        int stageValue = 0; // 기본값 (불러오지 못했을 경우 대비)
        string query = "SELECT value FROM information WHERE name = 'maxStage'";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            { 
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stageValue = reader.GetInt32("value");
                        Debug.Log("maxStage 값을 불러옴: " +stageValue);
                    }
                    else
                    {
                        Debug.LogWarning( "maxStage is null.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading stage: " + ex.Message);
        }
        return stageValue;
    }

}
