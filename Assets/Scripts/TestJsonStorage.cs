using UnityEngine;
using System.Collections.Generic;

// Мини-класс для теста
[System.Serializable]
public class TestUpgrade
{
    public string StatName;
    public int Level;
}

public class TestJsonStorage : MonoBehaviour
{
    private JsonStorage<TestUpgrade> storage;
    private List<TestUpgrade> upgrades;

    private void Start()
    {
        Debug.Log("🚀 Старт теста JsonStorage");

        // Создаём Storage с файлом
        storage = new JsonStorage<TestUpgrade>("test_upgrades.json");

        // Загружаем старые данные
        upgrades = storage.GetAll();
        Debug.Log($"📥 Загружено {upgrades.Count} записей");

        // Добавляем новую запись
        TestUpgrade newUpgrade = new TestUpgrade
        {
            StatName = "Speed",
            Level = 5
        };
        upgrades.Add(newUpgrade);

        // Сохраняем обратно
        SaveAll();

        // Проверяем содержимое
        foreach (var upg in upgrades)
        {
            Debug.Log($"🛠 Прокачка: {upg.StatName}, уровень: {upg.Level}");
        }

        Debug.Log($"💾 Данные сохранены в файл: {Application.persistentDataPath}/test_upgrades.json");
    }

    private void SaveAll()
    {
        // Переустанавливаем список
        storage.ClearAll();
        foreach (var upg in upgrades)
        {
            storage.Add(upg);
        }
        storage.Save();
    }


   

}
