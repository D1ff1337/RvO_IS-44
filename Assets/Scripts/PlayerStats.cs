using System;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    public string PlayerName;
    public int Health = 100;
    public int Speed = 10;
    public int Armor = 5;
    public int Strength = 15;
    public int CriticalChance = 5;
    public int Vampirism = 0;

    public PlayerStats(string playerName)
    {
        PlayerName = playerName;
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.StatName)
        {
            case "Speed":
                Speed += upgrade.Level;
                break;
            case "Armor":
                Armor += upgrade.Level * 2;
                break;
            case "Strength":
                Strength += upgrade.Level * 3;
                break;
            case "CriticalChance":
                CriticalChance += upgrade.Level;
                break;
            case "Vampirism":
                Vampirism += upgrade.Level;
                break;
            default:
                Debug.Log("Unknown upgrade type: " + upgrade.StatName);
                break;
        }

        Debug.Log($"Applied {upgrade.StatName} (Lv.{upgrade.Level}) to {PlayerName}");
    }

    public void ShowStats()
    {
        Debug.Log($"🧍 Гравець: {PlayerName}");
        Debug.Log($"❤️ Здоров'я: {Health}");
        Debug.Log($"💨 Швидкість: {Speed}");
        Debug.Log($"🛡️ Броня: {Armor}");
        Debug.Log($"💥 Сила: {Strength}");
        Debug.Log($"🎯 Шанс криту: {CriticalChance}%");
        Debug.Log($"🩸 Вампіризм: {Vampirism}");
    }
}
