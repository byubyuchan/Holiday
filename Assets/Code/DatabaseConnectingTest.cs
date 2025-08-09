using System;
using MySql.Data.MySqlClient;
using UnityEngine;

public class DataBaseConnectingTest : MonoBehaviour
{
    //DB 연결을 위한 생성자
    private string connectionString;
    private MySqlConnection connection;
    private int playerId = 1; // 기본 플레이어 ID
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
            Debug.Log("first DB connecting Success!!!");
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

    public void defaultSetting(int playerID)
    {
        string query = @"
            INSERT INTO info (Player, name, gold, stage, kill_cnt, clear, time)
            VALUES (@Player, @name, @gold, @stage, @kill_cnt, @clear, @time)
            ON DUPLICATE KEY UPDATE
                name = VALUES(name),
                gold = VALUES(gold),
                stage = VALUES(stage),
                kill_cnt = VALUES(kill_cnt),
                clear = VALUES(clear),
                time = VALUES(time);";

        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    Debug.Log("DB Default Setting!");
                    // SQL Injection 방지를 위한 파라미터 바인딩
                    command.Parameters.AddWithValue("@Player", playerID);
                    command.Parameters.AddWithValue("@name", "none");
                    command.Parameters.AddWithValue("@gold", 50);
                    command.Parameters.AddWithValue("@stage", 0);
                    command.Parameters.AddWithValue("@kill_cnt", 0);
                    command.Parameters.AddWithValue("@clear", 0);
                    command.Parameters.AddWithValue("@time", 0);
                    playerId = playerID;
                    int rowsAffected = command.ExecuteNonQuery();
                    Debug.Log("DB Default Setting! succuesss!!!!");
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
    private void Execute(int playerId, string name, int value)
    {
        Debug.Log("excute 시작");
        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                //switch (fieldName.ToLower()) // 소문자로 비교하여 실수 방지
                //{
                //    case "gold":
                //        query = "INSERT INTO info (Player, gold) VALUES (@playerID, @value) ON DUPLICATE KEY UPDATE gold = @value";
                //        break;
                //    case "stage":
                //        query = "INSERT INTO info (Player, stage) VALUES (@playerID, @value) ON DUPLICATE KEY UPDATE stage = @value";
                //        break;
                //    case "kill_cnt":
                //        query = "INSERT INTO info (Player, kill_cnt) VALUES (@playerID, @value) ON DUPLICATE KEY UPDATE kill_cnt = @value";
                //        break;
                //    case "clear":
                //        query = "INSERT INTO info (Player, clear) VALUES (@playerID, @value) ON DUPLICATE KEY UPDATE clear = @value";
                //        break;
                //    case "time":
                //        query = "INSERT INTO info (Player, time) VALUES (@playerID, @value) ON DUPLICATE KEY UPDATE time = @value";
                //        break;
                //    // 중요: 'name' 컬럼은 VARCHAR 타입이므로 별도의 메소드로 관리하는 것이 좋습니다.
                //    // case "name":
                //    //     ...
                //    //     break;
                //    default: 
                //원래 해야하는 방식이지만, 속도를 높이기 위해 아래의 보안에 취약한 방식으로 사용함.

                string query = $"INSERT INTO info (Player, {name}) VALUES (@playerID, @value) ON DUPLICATE KEY UPDATE {name} = @value";
                Debug.Log($"Saving to DB → {name} = {value}");

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@value", value);
                    cmd.Parameters.AddWithValue("@playerID", playerId);
                    cmd.ExecuteNonQuery();
                }
                Debug.Log($"Saved {playerId}, '{name}' = {value} to DB");                
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error to saving: " + ex.Message);
        }
    }

    //DB 퀴리 실행을 위한 함수2
    public void saveValue(int playerId, string name, int value) => Execute(playerId, name, value);

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
    
    public void saveGold(int gold)
    {
        Execute(playerId,"gold",gold);

    }

    //public void SaveTowerData(string towerName, int towerType, string areaName, string tileName)
    //{
    //    using (MySqlConnection conn = new MySqlConnection(connectionString))
    //    {
    //        try
    //        {

    //            Debug.Log($"[DB] 타워 저장 시작: {towerName}, {towerType}, {areaName}, {tileName}");
    //            conn.Open();

    //            string query = @"
    //                INSERT INTO information (name, tower_type, area_name, tile_name)
    //                VALUES (@name, @type, @area, @tile)
    //                ON DUPLICATE KEY UPDATE
    //                    tower_type = VALUES(tower_type),
    //                    area_name = VALUES(area_name),
    //                    tile_name = VALUES(tile_name);";

    //            using (MySqlCommand cmd = new MySqlCommand(query, conn))
    //            {
    //                cmd.Parameters.AddWithValue("@name", towerName);
    //                cmd.Parameters.AddWithValue("@type", towerType);
    //                cmd.Parameters.AddWithValue("@area", areaName);
    //                cmd.Parameters.AddWithValue("@tile", tileName);

    //                cmd.ExecuteNonQuery();
    //            }

    //            Debug.Log($"[DB] 타워 저장 성공: {towerName}");
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.LogError("[DB] 타워 저장 실패: " + ex.Message);
    //        }
    //    }
    //}

}
