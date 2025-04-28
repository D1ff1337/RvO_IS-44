using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeUIManager : MonoBehaviour
{
    public Button addButton;
    public Button showButton;
    public Button clearButton;
    public Text upgradesText;

    private IDataStorage<UpgradeData> storage;

    private void Start()
    {
        storage = new JsonStorage<UpgradeData>("upgrades.json");

        addButton.onClick.AddListener(AddUpgrade);
        showButton.onClick.AddListener(ShowUpgrades);
        clearButton.onClick.AddListener(ClearUpgrades);
    }

    private void AddUpgrade()
    {
        string[] statNames = { "Speed", "Armor", "MaxHp", "Strength", "Regeneration", "Vampirism", "Crits" };
        string randomStat = statNames[Random.Range(0, statNames.Length)];

        string currentPlayerName = PlayerPrefs.GetString("PlayerName", "DefaultPlayer");

        storage.Add(new UpgradeData
        {
            PlayerName = currentPlayerName,
            StatName = randomStat,
            Level = Random.Range(1, 10)
        });
        storage.Save();
        Debug.Log($"➕ Додано новий апгрейд для {currentPlayerName}: {randomStat}");
    }



    private void ShowUpgrades()
    {
        string currentPlayerName = PlayerPrefs.GetString("PlayerName", "DefaultPlayer");

        List<UpgradeData> upgrades = storage.GetAll();
        upgradesText.text = "📋 Прокачки:\n\n";

        foreach (var upg in upgrades)
        {
            if (upg.PlayerName == currentPlayerName)
            {
                upgradesText.text += $"- {upg.StatName.PadRight(12)} : Lv.{upg.Level}\n";
            }
        }
    }




    private void ClearUpgrades()
    {
        (storage as JsonStorage<UpgradeData>).ClearAll();
        storage.Save();
        upgradesText.text = "Всі прокачки видалено!";
        Debug.Log("🧹 Всі прокачки очищено");
    }
}
