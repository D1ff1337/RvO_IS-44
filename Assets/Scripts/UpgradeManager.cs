using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager
{
    private PlayerStats playerStats;
    private List<UpgradeData> upgrades = new List<UpgradeData>();

    public UpgradeManager(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    // UpgradeManager.cs
    public void ApplyUpgrade(PlayerStats player, string statName, int level)
    {
        var upgrade = new UpgradeData(player.PlayerName, statName, level);
        player.ApplyUpgrade(upgrade);
    }


    public void ShowUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            Debug.Log($"{upgrade.PlayerName} - {upgrade.StatName} (Lv.{upgrade.Level})");
        }
    }
}
