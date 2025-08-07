using System;
using MySql.Data.MySqlClient;
using UnityEngine;

public class DataBaseConnectingTest : MonoBehaviour
{
    //DB 연결을 위한 생성자
    private string connectionString;
    private MySqlConnection connection;
    //
    public static DataBaseConnectingTest Instance;

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
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            Close();
        }
        catch (Exception ex)
        {
            Debug.LogError("DB Connecting Fail ! : " + ex.Message);
            Debug.LogError("DB Connecting Fail ! : " + ex.Message);
            Debug.LogError("DB Connecting Fail ! : " + ex.Message);
            Debug.LogError("DB Connecting Fail ! : " + ex.Message);
            Debug.LogError("DB Connecting Fail ! : " + ex.Message);
            Debug.LogError("DB Connecting Fail ! : " + ex.Message);
            Debug.LogError("DB Connecting Fail ! : " + ex.Message);
        }
    }
    //기본 데이터 저장 함수

    public void defaultSetting()
    {
        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                Debug.Log("DB connecting Success!");

                string query = "INSERT INTO information (name, value) VALUES " +
                               "('gold', 50), " +
                               "('stage', 0), " +
                               "('time', 0) " +
                               "ON DUPLICATE KEY UPDATE value = VALUES(value);";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    int result = cmd.ExecuteNonQuery();
                    Debug.Log($"Default values inserted or updated. Rows affected: {result}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error to saving!!: " + ex.Message);
            Debug.LogError("Error to saving!!: " + ex.Message);
            Debug.LogError("Error to saving!!: " + ex.Message);
            Debug.LogError("Error to saving!!: " + ex.Message);
            Debug.LogError("Error to saving!!: " + ex.Message);
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
                    compareStage(value,LoadValue("maxStage"));
                }

                
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error to saving: " + ex.Message);
        }
    }

    //DB 퀴리 실행을 위한 함수2
    public void saveValue(string name, int value) => Execute(name, value);

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
            saveValue("maxStage", currentstage);
        }
        else
        {
            Debug.Log("Stage is not updated");
        }
    }

    //DB에서 maxStage 값을 불러오는 함수
    public int LoadValue(string name)
    {
        int stageValue = 0; // 기본값 (불러오지 못했을 경우 대비)
        string query = $"SELECT value FROM information WHERE name = '{name}'";

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
                        Debug.Log($"{name} 값을 불러옴: " +stageValue);
                    }
                    else
                    {
                        Debug.LogWarning( $"{name} is null.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading: " + ex.Message);
        }
        return stageValue;
    }

    public void SaveTowerData(string towerName, int towerType, string areaName, string tileName)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {

                Debug.Log($"[DB] 타워 저장 시작: {towerName}, {towerType}, {areaName}, {tileName}");
                conn.Open();

                string query = @"
                    INSERT INTO information (name, tower_type, area_name, tile_name)
                    VALUES (@name, @type, @area, @tile)
                    ON DUPLICATE KEY UPDATE
                        tower_type = VALUES(tower_type),
                        area_name = VALUES(area_name),
                        tile_name = VALUES(tile_name);";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", towerName);
                    cmd.Parameters.AddWithValue("@type", towerType);
                    cmd.Parameters.AddWithValue("@area", areaName);
                    cmd.Parameters.AddWithValue("@tile", tileName);

                    cmd.ExecuteNonQuery();
                }

                Debug.Log($"[DB] 타워 저장 성공: {towerName}");
            }
            catch (Exception ex)
            {
                Debug.LogError("[DB] 타워 저장 실패: " + ex.Message);
            }
        }
    }

}
