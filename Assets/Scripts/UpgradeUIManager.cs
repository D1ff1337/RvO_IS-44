using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIManager : MonoBehaviour
{
    public Text upgradeListText;
    public InputField playerNameInput;
    public InputField statNameInput;
    public InputField levelInput;
    public Button addButton;
    public Button resetButton;
    public Button showButton;
    public Text errorMessageText;

    private Repository repository;
    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = new PlayerStats("Maxim");
        repository = FindObjectOfType<Repository>();

        
        addButton.onClick.AddListener(AddUpgrade);
        resetButton.onClick.AddListener(ResetAllUpgrades);
        showButton.onClick.AddListener(ShowUpgrades);

        
        UpdateUpgradeList();
    }

    private void AddUpgrade()
    {
        string playerName = playerNameInput.text.Trim();
        string statName = statNameInput.text.Trim();
        int level;

        
        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(statName) || !int.TryParse(levelInput.text, out level))
        {
            errorMessageText.text = "❌ Введіть коректні значення для всіх полів!";
            return;
        }

        
        var upgrade = new UpgradeData(playerName, statName, level);
        if (!upgrade.IsValid(out string errorMessage))
        {
            errorMessageText.text = $"❌ Помилка валідації:\n{errorMessage}";
            return;
        }

        
        if (!repository.PlayerExists(playerName))
        {
            errorMessageText.text = $"❌ Гравець '{playerName}' не знайдений!";
            return;
        }

        
        if (!repository.IsStatValid(statName))
        {
            errorMessageText.text = $"❌ Стат '{statName}' не знайдений!";
            return;
        }

        
        repository.AddUpgrade(playerName, statName, level);
        UpdateUpgradeList();
        errorMessageText.text = "";
        Debug.Log($"✅ Додано апгрейд: {statName} (Lv.{level}) для {playerName}");
    }

    private void ResetAllUpgrades()
    {
        repository.ResetAllUpgrades();
        UpdateUpgradeList();
        errorMessageText.text = "🧹 Всі апгрейди скинуто!";
    }

    private void ShowUpgrades()
    {
        string playerName = playerNameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            errorMessageText.text = "❌ Введіть нікнейм гравця!";
            return;
        }

        if (!repository.PlayerExists(playerName))
        {
            errorMessageText.text = $"❌ Гравець '{playerName}' не знайдений!";
            return;
        }

        UpdateUpgradeList(playerName);
        errorMessageText.text = "";
    }

    private void UpdateUpgradeList(string playerName = null)
    {
        upgradeListText.text = "";
        List<UpgradeData> upgrades = repository.GetUpgrades(playerName ?? playerStats.PlayerName);

        if (upgrades.Count == 0)
        {
            upgradeListText.text = "❌ Апгрейдів не знайдено!";
            return;
        }

        foreach (var upgrade in upgrades)
        {
            upgradeListText.text += $"{upgrade.StatName} (Lv.{upgrade.Level})\n";
        }
    }
}
