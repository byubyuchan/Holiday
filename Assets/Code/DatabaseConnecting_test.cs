using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

public class DataBaseConnectingTest : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Connecting to database..." + ConnectionTest());
    }

    public bool ConnectionTest()
    {
        string connectionString = string.Format("Server={0};Database={1};Uid={2};Pwd={3};",
            "127.0.0.1", "db_test", "root", "0000");
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                Debug.Log("Database connection successful!");
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Database connection failed: " + ex.Message);
            return false;
        }
    }
}
