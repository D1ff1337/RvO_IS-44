using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Boosts : MonoBehaviour
{
    [System.Serializable]
    public class Amplifier
    {
        public string Name;
        public Button button;
        public Text levelText;
        public int level = 0;
    }

    public List<Amplifier> amplifiers;
    private PlayerController player;
    private Repository repository;
    private bool preventApplyUpgrades = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
        repository = FindObjectOfType<Repository>();

        if (repository == null)
        {
            Debug.LogError("❌ Repository не найден в сцене!");
            return;
        }

        foreach (var amplifier in amplifiers)
        {
            amplifier.level = repository.GetUpgradeLevel(PlayerPrefs.GetString("CurrentPlayer", "default"), amplifier.Name);
        }

        foreach (var amplifier in amplifiers)
        {
            Amplifier localAmplifier = amplifier;
            localAmplifier.button.onClick.AddListener(() => ApplyUpgrade(localAmplifier));
        }

        ApplyAllUpgradesToPlayer();
        UpdateAllLevelsText();
    }

    public void ApplyUpgrade(Amplifier amplifier)
    {
        if (player == null)
        {
            Debug.LogError("❌ PlayerController не найден в сцене!");
            return;
        }

        amplifier.level++;
        player.UpgradeStat(amplifier.Name);
        repository.SaveUpgradeLevel(PlayerPrefs.GetString("CurrentPlayer", "default"), amplifier.Name, amplifier.level);
        UpdateAllLevelsText();
    }

    public void ApplyUpgradeByName(string statName)
    {
        var amp = amplifiers.FirstOrDefault(a => a.Name == statName);
        if (amp != null)
        {
            ApplyUpgrade(amp);
        }
        else
        {
            Debug.LogWarning($"❌ Стат {statName} не знайдено у списку апгрейдів.");
        }
    }

    public void IncrementLevel(string statName)
    {
        var amplifier = amplifiers.FirstOrDefault(a => a.Name == statName);
        if (amplifier != null)
        {
            amplifier.level++;
            Debug.Log($"🧠 Уровень {statName} увеличен вручную до {amplifier.level}");

            string playerName = PlayerPrefs.GetString("CurrentPlayer", "default");
            repository.SaveUpgradeLevel(playerName, amplifier.Name, amplifier.level);
            UpdateAllLevelsText();
        }
    }

    public void UpdateAllLevelsText()
    {
        amplifiers.ForEach(a =>
        {
            if (a.levelText != null)
                a.levelText.text = $"(Lv.{a.level})";
        });
    }

    public void ResetBoosts()
    {
        preventApplyUpgrades = true;

        if (repository == null)
        {
            repository = FindObjectOfType<Repository>();
            if (repository == null)
            {
                Debug.LogError("❌ Repository не найден в сцене!");
                return;
            }
        }

        repository.ResetAllUpgrades();

        foreach (var amplifier in amplifiers)
        {
            amplifier.level = 0;
        }

        if (player != null)
        {
            player.ResetStats();
        }

        ApplyAllUpgradesToPlayer();
        UpdateAllLevelsText();

        Debug.Log("🔄 Всі дані скинуто");
    }

    public void ApplyAllUpgradesToPlayer()
    {
        if (preventApplyUpgrades || player == null) return;

        foreach (var amp in amplifiers)
        {
            for (int i = 0; i < amp.level; i++)
            {
                player.UpgradeStat(amp.Name);
            }
        }
    }

    public void SaveAllBoosts()
    {
        if (repository == null) return;

        foreach (var amp in amplifiers)
        {
            repository.SaveUpgradeLevel(PlayerPrefs.GetString("CurrentPlayer", "default"), amp.Name, amp.level);
        }

        if (player != null)
        {
            AuthManager.SavePlayerProgress(player.playerHealth, player.upgradePoints);
        }

        Debug.Log("💾 Прогрес збережено вручну.");
    }
}
