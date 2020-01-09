using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject playerUI;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                Resume();                
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        playerUI.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Pause()
    {
        playerUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }
    public void LoadOptions()
    {
        Debug.Log("Load Options Menu...");
    }
    public void Quit()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
