using Managers;
using Steamworks;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu, settingsMenu;

    private void Start()
    {
        Resume();
    }

    // Start is called before the first frame update
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameState.isPaused)
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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameState.isPaused = false;
    }

    public void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameState.isPaused = true;
    }

    public void Settings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
    
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
