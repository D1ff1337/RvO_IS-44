using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private Toggle androidToggle;

    private void Start()
    {
        androidToggle = FindObjectOfType<Toggle>();
    }
    public void PlayGame(int sceneIndex)
    {
        bool isAndroid = androidToggle.isOn;
        PlayerPrefs.SetInt("Android", isAndroid ? 1 : 0);
        SceneManager.LoadScene(sceneIndex);
        Debug.Log(PlayerPrefs.GetInt("Android"));
    }
    public void ExitGame() 
    {
        Application.Quit();
        Debug.Log("Quit");
    }

}
