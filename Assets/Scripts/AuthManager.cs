using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

#region Класи ролей

public class Player
{
    public string Nickname { get; protected set; }

    public Player(string nickname)
    {
        Nickname = nickname;
    }

    public virtual void EnterGame()
    {
        Debug.Log($"🔓 {Nickname} зайшов у гру.");
    }
}

public class Admin : Player
{
    public Admin(string nickname) : base(nickname) { }

    public override void EnterGame()
    {
        Debug.Log($"👑 Адміністратор {Nickname} має повний доступ.");
        SceneManager.LoadScene("AdminScene");
    }
}

public class User : Player
{
    public User(string nickname) : base(nickname) { }

    public override void EnterGame()
    {
        Debug.Log($"👤 Користувач {Nickname} грає у звичайному режимі.");
        SceneManager.LoadScene("GameScene");
    }
}

#endregion

public class AuthManager : MonoBehaviour
{
    public InputField nicknameInput;
    public InputField passwordInput;
    public Button submitButton;
    public Button playButton;
    public Text statusText;
    public Toggle adminToggle;




    private Dictionary<string, string> userRoles = new Dictionary<string, string>();

    void Start()
    {



        submitButton.onClick.AddListener(SubmitNickname);
        playButton.onClick.AddListener(OnPlayClicked);
        playButton.interactable = false;

        if (PlayerPrefs.HasKey("CurrentPlayer"))
        {
            string nickname = PlayerPrefs.GetString("CurrentPlayer");
            nicknameInput.text = nickname;
            playButton.interactable = true;
            statusText.text = $"✅ Welcome back, {nickname}!";
        }
    }

    void SubmitNickname()
    {
        string nickname = nicknameInput.text.Trim();
        string password = passwordInput.text;

        if (!PlayerPrefs.HasKey(nickname + "_password"))
        {
            string hash = Hash(password);
            PlayerPrefs.SetString(nickname + "_password", hash);

            
            string role = adminToggle.isOn ? "admin" : "user";
            PlayerPrefs.SetString(nickname + "_role", role);

            Debug.Log($"🆕 Користувач зареєстрований з роллю: {role}");
        }

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(password))
        {
            statusText.text = "❌ Введіть нікнейм і пароль!";
            return;
        }

        PlayerPrefs.SetString("CurrentPlayer", nickname);

        if (PlayerPrefs.HasKey(nickname + "_password"))
        {
            string storedHash = PlayerPrefs.GetString(nickname + "_password");
            if (Verify(password, storedHash))
            {
                statusText.text = $"✅ Вхід виконано. Вітаємо, {nickname}!";
                playButton.interactable = true;
            }
            else
            {
                statusText.text = "❌ Неправильний пароль!";
                return;
            }
        }
        else
        {
            string hash = Hash(password);
            PlayerPrefs.SetString(nickname + "_password", hash);
            PlayerPrefs.SetString(nickname + "_role", "user");

            string[] stats = { "Speed", "Strength", "Regeneration", "Max-hp", "Armor", "Vampirism", "Crits" };
            foreach (string stat in stats)
            {
                PlayerPrefs.SetInt(nickname + "_Upgrade_" + stat, 0);
            }
            PlayerPrefs.SetInt(nickname + "_UpgradePoints", 0);
            PlayerPrefs.SetFloat(nickname + "_Health", 2f);
            PlayerPrefs.SetInt(nickname + "_initialized", 1);

            statusText.text = $"🎉 Реєстрація завершена. Вітаємо, {nickname}!";
            playButton.interactable = true;
        }

        PlayerPrefs.Save();
    }

    void OnPlayClicked()
    {
        string nickname = PlayerPrefs.GetString("CurrentPlayer", "default");
        string role = PlayerPrefs.GetString(nickname + "_role", "user");

        Player player;
        if (role == "admin")
        {
            player = new Admin(nickname);
        }
        else
        {
            player = new User(nickname);
        }

        player.EnterGame();
    }

    public void ResetProgress()
    {
        string nickname = PlayerPrefs.GetString("CurrentPlayer", "default");
        string[] stats = { "Speed", "Strength", "Regeneration", "Max-hp", "Armor", "Vampirism", "Crits" };
        foreach (string stat in stats)
        {
            PlayerPrefs.DeleteKey(nickname + "_Upgrade_" + stat);
        }
        PlayerPrefs.DeleteKey(nickname + "_UpgradePoints");
        PlayerPrefs.DeleteKey(nickname + "_Health");
        PlayerPrefs.DeleteKey(nickname + "_initialized");
        PlayerPrefs.Save();
        Debug.Log("🗑️ Прогрес користувача очищено!");

        Boosts boosts = FindObjectOfType<Boosts>();
        if (boosts != null)
        {
            boosts.ResetBoosts();
        }
    }

    public static void SavePlayerProgress(float health, int upgradePoints)
    {
        string nickname = PlayerPrefs.GetString("CurrentPlayer", "default");
        PlayerPrefs.SetFloat(nickname + "_Health", health);
        PlayerPrefs.SetInt(nickname + "_UpgradePoints", upgradePoints);
        PlayerPrefs.Save();
        Debug.Log("💾 Прогрес збережено для: " + nickname);
    }

    public static float LoadPlayerHealth()
    {
        string nickname = PlayerPrefs.GetString("CurrentPlayer", "default");
        return PlayerPrefs.GetFloat(nickname + "_Health", 2f);
    }

    public static int LoadUpgradePoints()
    {
        string nickname = PlayerPrefs.GetString("CurrentPlayer", "default");
        return PlayerPrefs.GetInt(nickname + "_UpgradePoints", 0);
    }

    public static string Hash(string input)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] data = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    public static bool Verify(string input, string storedHash)
    {
        string inputHash = Hash(input);
        return inputHash == storedHash;
    }
}
