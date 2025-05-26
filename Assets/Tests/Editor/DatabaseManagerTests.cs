using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


using NUnit.Framework;
using UnityEngine;
using System.IO;

public class DatabaseManagerTests
{
    private DatabaseManager dbManager;

    [SetUp]
    public void Setup()
    {
        // ������� ����� ������ � ����� �����������
        var go = new GameObject("DBManagerTestObject");
        dbManager = go.AddComponent<DatabaseManager>();

        // ��� ����� ����� �������� ��� �������������� ���� � ����, ���� �����
        string testDbPath = Path.Combine(Application.persistentDataPath, "test_player_upgrades.db");
        if (File.Exists(testDbPath))
            File.Delete(testDbPath);

       
    }

    [Test]
    public void SaveAndLoadUpgrade_WorksCorrectly()
    {
        string player = "TestPlayer";
        string stat = "Strength";
        int level = 5;

        // ��������� �������
        dbManager.SaveUpgrade(player, stat, level);

        // ��������� �������
        int loadedLevel = dbManager.LoadUpgrade(player, stat);

        Assert.AreEqual(level, loadedLevel, "����������� ������� �������� ������ ��������� � ����������");
    }

    [TearDown]
    public void TearDown()
    {
        // ��������� ���������� � ������� ������
        dbManager.OnApplicationQuit();
        GameObject.DestroyImmediate(dbManager.gameObject);
    }
}
