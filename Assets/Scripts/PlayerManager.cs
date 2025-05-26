using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerStats playerStats;
    private List<UpgradeData> upgrades = new List<UpgradeData>();

    private void Start()
    {
        playerStats = new PlayerStats("Maxim");

        
        upgrades.Add(new SpeedUpgrade(playerStats.PlayerName, 3));
        upgrades.Add(new ArmorUpgrade(playerStats.PlayerName, 2));
        upgrades.Add(new StrengthUpgrade(playerStats.PlayerName, 4));

        
        foreach (var upgrade in upgrades)
        {
            playerStats.ApplyUpgrade(upgrade);
        }

        
        playerStats.ShowStats();
    }
}
