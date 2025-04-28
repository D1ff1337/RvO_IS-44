using UnityEngine;

public class AdminPanelController : MonoBehaviour
{
    private PlayerController player;

    private void Start()
    {
        GameObject adminPanel = GameObject.FindGameObjectWithTag("AdminPanel");

        if (adminPanel == null)
        {
            Debug.LogError("AdminPanel с тегом 'AdminPanel' не найдена!");
            return;
        }

        // ⬇️ ДОБАВЬ ЭТО
        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("❌ PlayerController не найден на сцене!");
        }

        string nickname = PlayerPrefs.GetString("CurrentPlayer", "default");
        string role = PlayerPrefs.GetString(nickname + "_role", "user");

        if (role != "admin")
        {
            adminPanel.SetActive(false);
            Debug.Log("🔒 AdminPanel скрыта — обычный пользователь");
        }
        else
        {
            adminPanel.SetActive(true);
            Debug.Log("👑 Админ вошёл — AdminPanel активна");
        }
    }


public void AddPoints()
    {
        if (player != null)
        {
            player.upgradePoints += 1; 
            Debug.Log($"👑 Админ добавил себе поинты! Теперь: {player.upgradePoints}");
        }
    }
}
