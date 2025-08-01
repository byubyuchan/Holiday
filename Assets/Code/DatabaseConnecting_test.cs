using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEditor.MemoryProfiler;
using UnityEngine;



public class DataBaseConnectingTest : MonoBehaviour
{
    //DB 연결을 위한 생성자
    private MySqlConnection connection;
    
    
    //unity와 db의 연결을 확인하는 함수

    public void Connect(string server, string database, string user, string password)
    {
        string connectionString = string.Format("Server={0};Database={1};Uid={2};Pwd={3};",
            "127.0.0.1", "holiday_db", "root", "0000");
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();
            Debug.Log("DB connecting Success!");
        }
        catch (Exception ex)
        {
            Debug.LogError("DB Connecting Fail ! : " + ex.Message);
        }
    }


    //DB 퀴리 실행을 위한 함수
    //아래의 save와 이것 중 무엇을 쓸지 고민중입니다.
    public void Execute(string query, Dictionary<string, object> parameters)
    {
        using (var cmd = new MySqlCommand(query, connection))
        {
            foreach (var pair in parameters)
                cmd.Parameters.AddWithValue(pair.Key, pair.Value);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.LogError("Query error: " + ex.Message);
            }
        }
    }
    public void Save(string column, int value)
    {
        Debug.Log($"Saving to DB → {column} = {value}");

        // 저장 로직을 작성해햐함
        //내일 할 예정입니다 ㅠㅠ
    }

    public void Close()
    {
        if (connection != null)
        {
            connection.Close();
            Debug.Log("DB connection end");
        }
    }
}
