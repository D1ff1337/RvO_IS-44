using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UnityEngine;

[Serializable]
public class UpgradeData
{
    [Required(ErrorMessage = "PlayerName is required")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "PlayerName must be between 3 and 20 characters")]
    public string PlayerName;

    [Required(ErrorMessage = "StatName is required")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "StatName must be between 3 and 20 characters")]
    public string StatName;

    [System.ComponentModel.DataAnnotations.Range(1, 100, ErrorMessage = "Level must be between 1 and 100")]
    public int Level;

    public UpgradeData(string playerName, string statName, int level)
    {
        PlayerName = playerName;
        StatName = statName;
        Level = level;
    }

    public bool IsValid(out string errorMessage)
    {
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateObject(this, context, results, true))
        {
            errorMessage = string.Join("\n", results.ConvertAll(r => r.ErrorMessage));
            return false;
        }

        errorMessage = "";
        return true;
    }

    public override string ToString()
    {
        return $"{StatName} (Lv.{Level})";
    }
}

public class SpeedUpgrade : UpgradeData
{
    public SpeedUpgrade(string playerName, int level)
        : base(playerName, "Speed", level) { }
}

public class ArmorUpgrade : UpgradeData
{
    public ArmorUpgrade(string playerName, int level)
        : base(playerName, "Armor", level) { }
}

public class StrengthUpgrade : UpgradeData
{
    public StrengthUpgrade(string playerName, int level)
        : base(playerName, "Strength", level) { }
}
