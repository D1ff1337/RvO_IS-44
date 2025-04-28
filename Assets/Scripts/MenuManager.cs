using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class MenuManager : MonoBehaviour
{

    [SerializeField] private InputField nameInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button playButton;


    [SerializeField] private Toggle androidToggle;
    [SerializeField] private GameObject pcCanvas; 

    private void Start()
    {
        if (nameInputField == null || submitButton == null || playButton == null)
        {
            Debug.LogError("❌ Не все UI элементы назначены в инспекторе!");
        }

       
        playButton.interactable = false;

        
        submitButton.onClick.AddListener(() =>
        {
            string nickname = nameInputField.text.Trim();

            if (!string.IsNullOrEmpty(nickname))
            {
                PlayerPrefs.SetString("PlayerName", nickname);
                playButton.interactable = true;
                Debug.Log($"🎮 Ник сохранён: {nickname}");
            }
            else
            {
                Debug.LogWarning("❗ Введите никнейм!");
            }
        });


        if (androidToggle == null)
            Debug.LogError("Android Toggle не назначен в инспекторе!");

        if (pcCanvas == null)
            Debug.LogError("PC Canvas не назначен в инспекторе!");
    }



    public void PlayGame(int sceneIndex)
    {
        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            Debug.LogWarning("❌ Нельзя начать игру без ника!");
            return;
        }

        bool isAndroid = androidToggle.isOn;
        PlayerPrefs.SetInt("Android", isAndroid ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("▶ Запуск игры...");
        SceneManager.LoadScene(sceneIndex);
    }


    private void Update()
    {
        
        if (androidToggle != null && pcCanvas != null)
            pcCanvas.SetActive(!androidToggle.isOn);
    }

    public void ExitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }




}
