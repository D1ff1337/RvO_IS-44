using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.IO;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;
    private IDbConnection dbConnection;

    private void Awake()
    {
        string fileName = "player_upgrades.db";
        dbPath = "URI=file:" + Path.Combine(Application.persistentDataPath, fileName);
        dbConnection = new SqliteConnection(dbPath);
        dbConnection.Open();

        CreateTableIfNotExists();
    }

    private void CreateTableIfNotExists()
    {
        using (var cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Upgrades (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlayerName TEXT NOT NULL,
                    StatName TEXT NOT NULL,
                    Level INTEGER NOT NULL
                );
            ";
            cmd.ExecuteNonQuery();
            Debug.Log("✅ Таблиця створена або вже існує");
        }
    }

    public void SaveUpgrade(string playerName, string statName, int level)
    {
        using (var cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = @"
                INSERT OR REPLACE INTO Upgrades (Id, PlayerName, StatName, Level)
                VALUES (
                    COALESCE((SELECT Id FROM Upgrades WHERE PlayerName = @player AND StatName = @stat), NULL),
                    @player,
                    @stat,
                    @level
                );";

            AddParameter(cmd, "@player", playerName);
            AddParameter(cmd, "@stat", statName);
            AddParameter(cmd, "@level", level);

            cmd.ExecuteNonQuery();
            Debug.Log($"💾 Збережено апгрейд {statName} (Lv.{level}) для {playerName}");
        }
    }

    public int LoadUpgrade(string playerName, string statName)
    {
        using (var cmd = dbConnection.CreateCommand())
        {
            cmd.CommandText = "SELECT Level FROM Upgrades WHERE PlayerName = @player AND StatName = @stat";
            AddParameter(cmd, "@player", playerName);
            AddParameter(cmd, "@stat", statName);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }

    private void AddParameter(IDbCommand cmd, string name, object value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        cmd.Parameters.Add(param);
    }

    public void OnApplicationQuit()
    {
        dbConnection?.Close();
    }
    public void CloseConnection()
    {
        dbConnection?.Close();
    }

}
