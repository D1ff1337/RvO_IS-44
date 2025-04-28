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
            amplifier.level = repository.GetUpgradeLevel(amplifier.Name); 
        }


        foreach (var amplifier in amplifiers)
        {
            Amplifier localAmplifier = amplifier;
            localAmplifier.button.onClick.AddListener(() => ApplyUpgrade(localAmplifier));
        }

        UpdateAllLevelsText();
    }

    public void ApplyUpgrade(Amplifier amplifier)
    {
        amplifier.level++;
        Debug.Log($"⏫ {amplifier.Name} прокачан до {amplifier.level}");

        player.UpgradeStat(amplifier.Name);

        repository.SaveUpgradeLevel(amplifier.Name, amplifier.level); 

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
            repository.SaveUpgradeLevel(statName, amplifier.level);
            Debug.Log($"🧠 Уровень {statName} увеличен вручную до {amplifier.level}");
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

        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
            if (player == null)
            {
                Debug.LogError("❌ PlayerController не найден. Убедись, что у игрока стоит тег Player!");
                return;
            }
        }
        if (player.hpSlider == null)
        {
            Debug.LogWarning("⚠️ hpSlider у игрока не назначен, пропускаем UI-сброс HP");
        }
        else
        {
            player.hpSlider.maxValue = player.playerHealth;
            player.hpSlider.value = player.playerHealth;
        }
        if (repository == null)
        {
            repository = FindObjectOfType<Repository>();
            if (repository == null)
            {
                Debug.LogError("❌ Repository не найден в сцене!");
                return;
            }
        }

        
        var statNames = amplifiers.Select(a => a.Name).ToList();
        repository.ResetAllUpgrades(statNames);

        
        foreach (var amplifier in amplifiers)
        {
            amplifier.level = 0;
        }

       
        if (player != null)
        {
            player.playerHealth = 2f;
            player.vampirismHeal = 0f;
            player.critChance = 0f;
            player.regenAmount = 0f;
            player.animationSpeed = 1f;
            player.jumpForce = 6f;
            player.ignoreHitTime = 2.5f;
            player.upgradePoints = 0;
            player.maxHpLevel = 0;

     
            player.hpSlider.maxValue = player.playerHealth;
            player.hpSlider.value = player.playerHealth;

            Debug.Log("🧬 Характеристики игрока сброшены");
        }

        
        UpdateAllLevelsText();

        Debug.Log("🔄 Все данные сброшены");
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
        repository.SaveUpgradeLevel(amp.Name, amp.level);
    }

    if (player != null)
    {
        AuthManager.SavePlayerProgress(player.playerHealth, player.upgradePoints);
    }

    Debug.Log("💾 Прогрес збережено вручну.");
}


}
