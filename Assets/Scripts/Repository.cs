using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Repository : MonoBehaviour
{
    private string currentPlayer;
    private JsonStorage<UpgradeData> storage;
    private List<UpgradeData> upgrades;
    private List<string> validStats = new List<string> { "Speed", "Strength", "Regeneration", "Max-hp", "Armor", "Vampirism", "Crits" };

    private void Awake()
    {
        currentPlayer = PlayerPrefs.GetString("CurrentPlayer", "default");
        string fileName = currentPlayer + "_upgrades.json";
        storage = new JsonStorage<UpgradeData>(fileName);

        // список апгрейдов
        upgrades = storage.GetAll();
    }

    public bool PlayerExists(string playerName)
    {
        return upgrades.Any(u => u.PlayerName == playerName);
    }

    public bool IsStatValid(string statName)
    {
        return validStats.Contains(statName);
    }

    public int GetUpgradeLevel(string playerName, string statName)
    {
        var upgrade = upgrades.FirstOrDefault(u => u.PlayerName == playerName && u.StatName == statName);
        if (upgrade != null)
        {
            Debug.Log($"📥 Loaded {statName} = {upgrade.Level} for {playerName}");
            return upgrade.Level;
        }
        else
        {
            Debug.Log($"📥 No upgrade found for {statName}, default 0");
            return 0;
        }
    }


    public void AddUpgrade(string playerName, string statName, int level)
    {
        var upgrade = new UpgradeData(playerName, statName, level);
        upgrades.Add(upgrade);
        SaveAll();
        Debug.Log($"📥 Додано апгрейд: {upgrade}");
    }

    public List<UpgradeData> GetUpgrades(string playerName)
    {
        return upgrades.Where(u => u.PlayerName == playerName).ToList();
    }

    public void ResetAllUpgrades()
    {
        upgrades.Clear();
        SaveAll();
        Debug.Log("🧹 Всі апгрейди скинуто!");
    }

    private void SaveAll()
    {
        storage.ClearAll();
        foreach (var upg in upgrades)
        {
            storage.Add(upg);
        }
        storage.Save();
    }

    public void SaveUpgradeLevel(string playerName, string statName, int level)
    {
        var upgrade = upgrades.FirstOrDefault(u => u.PlayerName == playerName && u.StatName == statName);
        if (upgrade != null)
        {
            upgrade.Level = level;
        }
        else
        {
            upgrades.Add(new UpgradeData(playerName, statName, level));
        }

        SaveAll();
        Debug.Log($"💾 Збережено апгрейд: {statName} (Lv.{level}) для гравця {playerName}");
    }

}
