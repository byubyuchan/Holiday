// using System;
// using MySql.Data.MySqlClient;
// using UnityEngine;

// public class DataBaseConnectingTest : MonoBehaviour
// {
//     //DB ������ ���� ������
//     private string connectionString;
//     private MySqlConnection connection;
//     private int playerId = 1; // �⺻ �÷��̾� ID
//     private int num = 0; // talent ����
//     public static DataBaseConnectingTest Instance;

//     private void Awake()
//     {
//         //databse ���� ���ڿ� ����
//         connectionString = "Server=";
//         String sever = "127.0.0.1";
//         String port = "3306";
//         String database = "holiday_db";
//         String user = "root";
// <<<<<<< HEAD
//         String password = "0000";
// =======
//         String password = "******";
// >>>>>>> 56873e3be39bc2280e210fc91b7862f42a8022fa
//         connectionString = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};",
//             sever, port, database, user, password);
//         connection = new MySqlConnection(connectionString);
//         try
//         {
//             connection.Open();
//             Debug.Log("first DB connecting Success!!!");
//             if (Instance == null)
//             {
//                 Instance = this;
//                 DontDestroyOnLoad(gameObject);
//             }
//             else
//             {
//                 Destroy(gameObject);
//             }
//             Close(); 
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError("DB Connecting Fail ! : " + ex.Message);
//             Debug.LogError("DB Connecting Fail ! : " + ex.Message);
//             Debug.LogError("DB Connecting Fail ! : " + ex.Message);
//             Debug.LogError("DB Connecting Fail ! : " + ex.Message);
//             Debug.LogError("DB Connecting Fail ! : " + ex.Message);
//             Debug.LogError("DB Connecting Fail ! : " + ex.Message);
//             Debug.LogError("DB Connecting Fail ! : " + ex.Message);
//         }
//     }
//     //�⺻ ������ ���� �Լ�

//     public void defaultSetting(int playerID)
//     {
//         string query = @"
//             INSERT INTO info (Player, name, gold, stage, clear, time)
//             VALUES (@Player, @name, @gold, @stage, @clear, @time)
//             ON DUPLICATE KEY UPDATE
//                 name = VALUES(name),
//                 gold = VALUES(gold),
//                 stage = VALUES(stage),
//                 clear = VALUES(clear),
//                 time = VALUES(time);";

//         try
//         {
//             using (MySqlConnection connection = new MySqlConnection(connectionString))
//             {
//                 connection.Open();

//                 using (MySqlCommand command = new MySqlCommand(query, connection))
//                 {
//                     Debug.Log("DB Default Setting!");
//                     // SQL Injection ������ ���� �Ķ���� ���ε�
//                     command.Parameters.AddWithValue("@Player", playerID);
//                     command.Parameters.AddWithValue("@name", "none");
//                     command.Parameters.AddWithValue("@gold", 50);
//                     command.Parameters.AddWithValue("@stage", 0);
//                     command.Parameters.AddWithValue("@clear", 0);
//                     command.Parameters.AddWithValue("@time", 0);
//                     playerId = playerID;
//                     int rowsAffected = command.ExecuteNonQuery();
//                     Debug.Log("DB Default Setting! succuesss!!!!");
//                 }
//             }
//         }

//         catch (Exception ex)
//         {
//             Debug.LogError("Error to saving!!: " + ex.Message);
//             Debug.LogError("Error to saving!!: " + ex.Message);
//             Debug.LogError("Error to saving!!: " + ex.Message);
//             Debug.LogError("Error to saving!!: " + ex.Message);
//             Debug.LogError("Error to saving!!: " + ex.Message);
//         }
//     }

//     //DB ���� ������ ���� �Լ�1
//     private void Execute(int playerId, string name, int value)
//     {
//         Debug.Log("excute ����");
//         try
//         {
//             using (MySqlConnection conn = new MySqlConnection(connectionString))
//             {
//                 conn.Open();
//                 //switch (fieldName.ToLower()) // �ҹ��ڷ� ���Ͽ� �Ǽ� ����
//                 //{
//                 //    case "gold":
//                 //        query = "INSERT INTO info (Player, gold) VALUES (@playerID, @value) ON DUPLICATE KEY UPDATE gold = @value";
//                 //        break;
//                 //    case "stage":
//                 //        query = "INSERT INTO info (Player, stage) VALUES (@playerID, @value) ON DUPLICATE KEY UPDATE stage = @value";
//                 //        break;
//                 //    // �߿�: 'name' �÷��� VARCHAR Ÿ���̹Ƿ� ������ �޼ҵ�� �����ϴ� ���� ����.
//                 //    // case "name":
//                 //    //     ...
//                 //    //     break;
//                 //    default: 
//                 //���� �ؾ��ϴ� ���������, �ӵ��� ���̱� ���� �Ʒ��� ���ȿ� ����� ������� �����.

//                 string query = $"INSERT INTO info (Player, {name}) VALUES (@playerID, @value) ON DUPLICATE KEY UPDATE {name} = @value";
//                 Debug.Log($"Saving to DB �� {name} = {value}");

//                 using (MySqlCommand cmd = new MySqlCommand(query, conn))
//                 {
//                     cmd.Parameters.AddWithValue("@value", value);
//                     cmd.Parameters.AddWithValue("@playerID", playerId);
//                     cmd.ExecuteNonQuery();
//                 }
//                 Debug.Log($"Saved {playerId}, '{name}' = {value} to DB");                
//             }
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError("Error to saving: " + ex.Message);
//         }
//     }

//     private void ExecuteTalent(int playerId, string name, string value)
//     {
//         Debug.Log("excute ����");
//         try
//         {
//             using (MySqlConnection conn = new MySqlConnection(connectionString))
//             {
//                 conn.Open();
//                 string query = $"INSERT INTO info (Player, {name}) VALUES (@playerID, @value) ON DUPLICATE KEY UPDATE {name} = @value";
//                 Debug.Log($"Saving to DB �� {name} = {value}");

//                 using (MySqlCommand cmd = new MySqlCommand(query, conn))
//                 {
//                     cmd.Parameters.AddWithValue("@value", value);
//                     cmd.Parameters.AddWithValue("@playerID", playerId);
//                     cmd.ExecuteNonQuery();
//                 }
//                 Debug.Log($"Saved {playerId}, '{name}' = {value} to DB");
//                 num++;
//             }
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError("Error to saving: " + ex.Message);
//         }
//     }

//     //DB ���� ������ ���� �Լ�2
//     public void saveValue(int playerId, string name, int value) => Execute(playerId, name, value);

//     public void saveTalent(string talent)
//     {
//         ExecuteTalent(playerId, "talent_"+num, talent);
//     }
//     //�ݴ� �Լ�
//     public void Close()
//     {
//         Debug.Log("����");
//         if (connection != null)
//         {
//             connection.Close();
//             Debug.Log("DB connection end");
//         }
//     }

//     public void clearCnt()
//     {
//         Execute(playerId, "clear", LoadValue(0));
//         Execute(0, "clear", LoadValue(0)+1);
//     }
//     public int LoadValue(int playerID)
//     {
//         int defalutValue = -1; // �⺻�� (�ҷ����� ������ ��� ���)
//         string query = $"SELECT clear FROM info WHERE Player = @playerID";

//         try
//         {
//             using (MySqlConnection conn = new MySqlConnection(connectionString))
//             { 
//                 conn.Open();
//                 using (MySqlCommand cmd = new MySqlCommand(query, conn))
//                 {
//                     cmd.Parameters.AddWithValue("@playerID", playerID);

//                     // ExecuteScalar: ���� ��(ù ��° ���� ù ��° �÷�)�� ������ �� �����մϴ�.
//                     object result = cmd.ExecuteScalar();

//                     // ����� null�� �ƴϰ� DBNull�� �ƴ� ���
//                     if (result != null && result != DBNull.Value)
//                     {
//                         defalutValue = Convert.ToInt32(result);
//                         Debug.Log("[DB Success] clear ���� �ҷ���:");
//                     }
//                     else
//                     {
//                         Debug.LogWarning($"[DB Info] Player {playerID}�� clear���� �������� �ʰų� NULL�Դϴ�.");
//                     }
//                 }
//             }
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError("DB Error loading to clear" + ex.Message);
//         }
//         return defalutValue;
//     }

//     public void saveGold(int gold)
//     {
//         Execute(playerId,"gold",gold);

//     }
//     public void saveName(string name)
//     {
//         ExecuteTalent(playerId, "name", name);
//     }
// }
