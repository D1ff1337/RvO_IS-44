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
        // Создаем новый объект с твоим компонентом
        var go = new GameObject("DBManagerTestObject");
        dbManager = go.AddComponent<DatabaseManager>();

        // Для теста можно очистить или переопределить путь к базе, если нужно
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

        // Сохраняем апгрейд
        dbManager.SaveUpgrade(player, stat, level);

        // Загружаем апгрейд
        int loadedLevel = dbManager.LoadUpgrade(player, stat);

        Assert.AreEqual(level, loadedLevel, "Загруженный уровень апгрейда должен совпадать с сохранённым");
    }

    [TearDown]
    public void TearDown()
    {
        // Закрываем соединение и удаляем объект
        dbManager.OnApplicationQuit();
        GameObject.DestroyImmediate(dbManager.gameObject);
    }
}
