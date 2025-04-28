using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Repository : MonoBehaviour
{
    private string currentPlayer;
    private JsonStorage<UpgradeData> storage;
    private List<UpgradeData> upgrades;

    private void Awake()
    {
        currentPlayer = PlayerPrefs.GetString("CurrentPlayer", "default");
        string fileName = currentPlayer + "_upgrades.json";
        storage = new JsonStorage<UpgradeData>(fileName);
        upgrades = storage.GetAll();
    }

    public int GetUpgradeLevel(string statName)
    {
        var upgrade = upgrades.FirstOrDefault(u => u.StatName == statName);
        if (upgrade != null)
        {
            Debug.Log($"📥 Loaded {statName} = {upgrade.Level} for {currentPlayer}");
            return upgrade.Level;
        }
        else
        {
            Debug.Log($"📥 No upgrade found for {statName}, default 0");
            return 0;
        }
    }

    public void SaveUpgradeLevel(string statName, int level)
    {
        var upgrade = upgrades.FirstOrDefault(u => u.StatName == statName);
        if (upgrade != null)
        {
            upgrade.Level = level;
        }
        else
        {
            upgrades.Add(new UpgradeData { StatName = statName, Level = level });
        }
        SaveAll();
    }

    public void ResetAllUpgrades(List<string> statNames)
    {
        upgrades.RemoveAll(u => statNames.Contains(u.StatName));
        SaveAll();
        Debug.Log($"🧹 All upgrades reset for {currentPlayer}");
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
}
