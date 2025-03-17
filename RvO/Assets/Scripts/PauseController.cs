using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PauseController : MonoBehaviour
{
    private Button restartBut, menuBut, pauseBut;
    private GameObject pausePanel;
    private bool isPause = false;
    void Start()
    {
        restartBut = GameObject.Find("RestartButton").GetComponent<Button>();
        menuBut = GameObject.Find("MenuButton").GetComponent<Button>();
        pauseBut = GameObject.Find("PauseButton").GetComponent<Button>();
        pausePanel = GameObject.Find("PausePanel");
        pausePanel.SetActive(false);

        restartBut.onClick.AddListener(RestartLevel);
        menuBut.onClick.AddListener(GoToMenu);
        pauseBut.onClick.AddListener(SetPause);
        Time.timeScale = 1.0f;
    }
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            SetPause();
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SetPause()
    {
        isPause = !isPause;
        Time.timeScale = isPause ? 0 : 1;
        Cursor.visible = isPause;
        Cursor.lockState = isPause ? CursorLockMode.None : CursorLockMode.Locked;
        pausePanel.SetActive(isPause);
    }
    private void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }


}
